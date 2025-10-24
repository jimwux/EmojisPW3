using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class Rol
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
