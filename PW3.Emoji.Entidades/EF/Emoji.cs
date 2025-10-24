using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class Emoji
{
    public int Id { get; set; }

    public string CodigoUnicode { get; set; } = null!;

    public string? Alias { get; set; }

    public virtual ICollection<MapeoEmocionEmoji> MapeoEmocionEmojis { get; set; } = new List<MapeoEmocionEmoji>();
}
