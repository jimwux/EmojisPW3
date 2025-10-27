using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string HashPassword { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int RolId { get; set; }

    public virtual ICollection<AnalisisResultado> AnalisisResultado { get; set; } = new List<AnalisisResultado>();

    public virtual ICollection<Imagen> Imagen { get; set; } = new List<Imagen>();

    public virtual Rol Rol { get; set; } = null!;
}
