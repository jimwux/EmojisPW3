using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class Emocion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<AnalisisResultado> AnalisisResultado { get; set; } = new List<AnalisisResultado>();

    public virtual MapeoEmocionEmoji? MapeoEmocionEmoji { get; set; }
}
