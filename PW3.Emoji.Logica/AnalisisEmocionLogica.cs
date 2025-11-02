using MLModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PW3.Emoji.Logica.Utils;

namespace PW3.Emoji.Logica;
public interface IAnalisisEmocionLogica
{
    string ObtenerEmocionDesdeImagen(string ruta);
}
public class AnalisisEmocionLogica : IAnalisisEmocionLogica
{
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
}
