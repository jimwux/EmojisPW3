namespace PW3.Emoji.Logica.Utils
{
    public static class EmotionTraduction
    {
        private static readonly Dictionary<string, string> _traductions = new()
        {
            { "happy", "FELICIDAD" },
            { "sad", "TRISTEZA" },
            { "angry", "ENOJO" },
            { "neutral", "NEUTRAL" },
            { "fear", "MIEDO" },
            { "disgust", "DISGUSTO" },
            { "surprise", "SORPRESA" },
        };

        public static string Traduct(string textEmotionEnglish)
        {
            return _traductions.TryGetValue(textEmotionEnglish.ToLower(), out var result)
                ? result
                : "Desconocido";
        }
        public static string getImageSvg(string textEmotionEnglish)
        {
            return textEmotionEnglish + ".svg";
        }


    }
}
