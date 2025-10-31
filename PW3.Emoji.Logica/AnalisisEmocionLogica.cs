using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Extensions.Logging;
using MLModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PW3.Emoji.Logica
{
    public interface IAnalisisEmocionLogica
    {
        List<Rectangle> DetectFaces(byte[] imageData);
        byte[] ExtractFace(byte[] originalImage, Rectangle faceRect, int padding = 20);
        Task ProcessFacesAsync(List<Rectangle> faces, byte[] imageBytes, float CONFIDENCE_THRESHOLD, List<EmocionResult> results);
    }

    public class AnalisisEmocionLogica : IAnalisisEmocionLogica
    {
        private readonly CascadeClassifier _faceCascade;
        private readonly ILogger<AnalisisEmocionLogica> _logger;

        public AnalisisEmocionLogica(ILogger<AnalisisEmocionLogica> logger)
        {
            _logger = logger;
            string haarCascadePath = Path.Combine(AppContext.BaseDirectory, "Models", "haarcascade_frontalface_default.xml");
            string? haarCascadeDir = Path.GetDirectoryName(haarCascadePath);
            if (!File.Exists(haarCascadePath))
            {
                if (!string.IsNullOrEmpty(haarCascadeDir))
                    Directory.CreateDirectory(haarCascadeDir);
                DownloadHaarCascade(haarCascadePath).Wait();
            }
            _faceCascade = new CascadeClassifier(haarCascadePath);
        }

        private async Task DownloadHaarCascade(string filePath)
        {
            string url = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_default.xml";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }

        public List<Rectangle> DetectFaces(byte[] imageData)
        {
            try
            {
                Mat mat = new Mat();
                CvInvoke.Imdecode(imageData, ImreadModes.AnyColor, mat);
                Mat grayMat = new Mat();
                CvInvoke.CvtColor(mat, grayMat, ColorConversion.Bgr2Gray);
                Rectangle[] faces = _faceCascade.DetectMultiScale(grayMat, 1.1, 5, new Size(30, 30));
                mat.Dispose();
                grayMat.Dispose();
                return faces.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detectar caras en la imagen");
                throw;
            }
        }

        public byte[] ExtractFace(byte[] originalImage, Rectangle faceRect, int padding = 20)
        {
            try
            {
                using var memoryStream = new MemoryStream(originalImage);
                using var bitmap = new Bitmap(memoryStream);
                var x = Math.Max(0, faceRect.X - padding);
                var y = Math.Max(0, faceRect.Y - padding);
                var width = Math.Min(bitmap.Width - x, faceRect.Width + padding * 2);
                var height = Math.Min(bitmap.Height - y, faceRect.Height + padding * 2);
                var paddedRect = new Rectangle(x, y, width, height);
                using var faceBitmap = bitmap.Clone(paddedRect, bitmap.PixelFormat);
                int targetWidth = 224;
                int targetHeight = 224;
                using var resizedBitmap = new Bitmap(targetWidth, targetHeight);
                using var graphics = Graphics.FromImage(resizedBitmap);
                graphics.DrawImage(faceBitmap, 0, 0, targetWidth, targetHeight);
                using var faceStream = new MemoryStream();
                resizedBitmap.Save(faceStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return faceStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer y redimensionar la cara");
                throw;
            }
        }

        public async Task ProcessFacesAsync(List<Rectangle> faces, byte[] imageBytes, float CONFIDENCE_THRESHOLD, List<EmocionResult> results)
        {
            foreach (var faceRect in faces)
            {
                try
                {
                    byte[] faceImageBytes = ExtractFace(imageBytes, faceRect);
                    var input = new MLModel1.ModelInput
                    {
                        ImageSource = faceImageBytes,
                        Label = string.Empty
                    };
                    var prediction = MLModel1.Predict(input);
                    var allPredictions = MLModel1.PredictAllLabels(input)
                        .Take(3)
                        .ToDictionary(kv => kv.Key, kv => kv.Value);
                    float maxConfidence = prediction.Score.Max();
                    string emocion = prediction.PredictedLabel;
                    var result = new EmocionResult
                    {
                        Emocion = emocion,
                        Confidence = maxConfidence,
                        TopPredictions = allPredictions,
                        ImageData = Convert.ToBase64String(faceImageBytes),
                        FaceRectangle = new FaceRectangle
                        {
                            X = faceRect.X,
                            Y = faceRect.Y,
                            Width = faceRect.Width,
                            Height = faceRect.Height
                        },
                        IsRecognized = maxConfidence >= CONFIDENCE_THRESHOLD
                    };
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al procesar la cara en posición {faceRect}");
                }
            }
        }
    }

    public class EmocionResult
    {
        public string Emocion { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public Dictionary<string, float> TopPredictions { get; set; } = new();
        public string ImageData { get; set; } = string.Empty;
        public FaceRectangle FaceRectangle { get; set; } = new FaceRectangle();
        public bool IsRecognized { get; set; }
    }

    public class FaceRectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
