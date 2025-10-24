using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class Imagen
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Ruta { get; set; } = null!;

    public DateTime FechaSubida { get; set; }

    public int? Ancho { get; set; }

    public int? Alto { get; set; }

    public virtual ICollection<ResultadoAnalisi> ResultadoAnalisis { get; set; } = new List<ResultadoAnalisi>();

    public virtual Usuario Usuario { get; set; } = null!;
}
