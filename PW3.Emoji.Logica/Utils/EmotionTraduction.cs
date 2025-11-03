namespace PW3.Emoji.Logica.Utils
{
    public static class EmotionTraduction
    {
        private static readonly Dictionary<string, string> _traductions = new()
        {
            { "happy", "Feliz 😄" },
            { "sad", "Triste 😢" },
            { "angry", "Enojado 😠" },
            { "neutral", "Neutral 😐" },
            { "fear", "Miedo 😨" },
            { "disgust", "Disgusto 🤢" },
            { "surprise", "Sorpresa 😮" },
        };

        public static string Traduct(string textEmotionEnglish)
        {
            return _traductions.TryGetValue(textEmotionEnglish.ToLower(), out var result)
                ? result
                : "Desconocido";
        }

    }
}
