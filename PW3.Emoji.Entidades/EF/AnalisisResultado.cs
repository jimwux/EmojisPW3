using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class AnalisisResultado
{
    public int Id { get; set; }

    public int ImagenId { get; set; }

    public int EmocionId { get; set; }

    public int UsuarioId { get; set; }

    public double Confianza { get; set; }

    public string? VectorJson { get; set; }

    public DateTime FechaAnalisis { get; set; }

    public virtual Emocion Emocion { get; set; } = null!;

    public virtual Imagen Imagen { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
