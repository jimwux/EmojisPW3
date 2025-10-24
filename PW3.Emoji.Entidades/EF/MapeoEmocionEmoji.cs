using System;
using System.Collections.Generic;

namespace PW3.Emoji.Entidades.EF;

public partial class MapeoEmocionEmoji
{
    public int Id { get; set; }

    public int EmocionId { get; set; }

    public int EmojiId { get; set; }

    public virtual Emocion Emocion { get; set; } = null!;

    public virtual Emoji Emoji { get; set; } = null!;
}
