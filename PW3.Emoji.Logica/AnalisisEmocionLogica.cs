using MLModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PW3.Emoji.Entidades.EF; 
using Microsoft.EntityFrameworkCore;
using PW3.Emoji.Logica.Utils;

namespace PW3.Emoji.Logica;
public interface IAnalisisEmocionLogica
{
    string ObtenerEmocionDesdeImagen(string ruta);
    // Recibe la emocionn, el ID del usuario y la ruta de la imagen
    Task<AnalisisResultado> GuardarAnalisisAsync(string emocionNombre, int usuarioId, string rutaImagen);
}
public class AnalisisEmocionLogica : IAnalisisEmocionLogica
{
        // 1. Campo privado para el DbContext
        private readonly PW3_EmojiContext _context;

        // 2. Inyección de Dependencias en el Constructor
        public AnalisisEmocionLogica(PW3_EmojiContext context)
        {
            _context = context;
        }

        public string ObtenerEmocionDesdeImagen(string ruta)
        {
            var bytes = File.ReadAllBytes(ruta);
            var input = new MLModel1.ModelInput { ImageSource = bytes };
            var result = MLModel1.PredictAllLabels(input);

            var emocionTop = result?
                .OrderByDescending(r => r.Value)
                .FirstOrDefault().Key ?? "Desconocida";

            return EmotionTraduction.Traduct(emocionTop);
        }

    // 3. Implementación del nuevo método para guardar
    public async Task<AnalisisResultado> GuardarAnalisisAsync(string emocionNombre, int usuarioId, string rutaImagen)
    {
            
            // A. Buscamos el ID de la emoción en la tabla Emocion
            var emocion = await _context.Emocion
                .FirstOrDefaultAsync(e => e.Nombre.ToLower() == emocionNombre.ToLower());

            // (Opcional) Manejar si la emoción no existe en tu tabla Emocion
            if (emocion == null)
                {
                    throw new Exception($"La emoción '{emocionNombre}' devuelta por el modelo no fue encontrada en la tabla 'Emocion'. Verifica que coincidan los nombres.");
                }

            // B. Creamos la entidad Imagen (porque AnalisisResultado depende de ella)
            var nuevaImagen = new Imagen
            {
                Ruta = rutaImagen,
                FechaSubida = DateTime.UtcNow,
                UsuarioId = usuarioId 
            };
            
            // C. Creamos la entidad AnalisisResultado
            var nuevoResultado = new AnalisisResultado
            {
                UsuarioId = usuarioId,
                EmocionId = emocion.Id,
                Imagen = nuevaImagen, // Asignamos la nueva imagen
                FechaAnalisis = DateTime.UtcNow
            };

            // D. Agregamos las nuevas entidades al contexto
            _context.Imagen.Add(nuevaImagen);
            _context.AnalisisResultados.Add(nuevoResultado);

            // E. Guardamos los cambios en la BD (esto guarda ambas tablas en una transacción)
            await _context.SaveChangesAsync();

            return nuevoResultado;
    }

}
