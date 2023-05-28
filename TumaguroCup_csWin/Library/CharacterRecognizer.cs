using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Foundation;

namespace TumaguroCup_csWin.Library
{
    public readonly struct Language
    {
        public static string JP = "ja-JP";
        public static string EN = "en-US";
    }
  
    internal class CharacterRecognizer
    {
        private static List<string> textList = new List<string>();
        private static List<int> xList = new List<int>();
        private static List<int> yList = new List<int>();
        private static List<int> widthList = new List<int>();
        private static List<int> heightList = new List<int>();
        public static async Task<string> RunOcr(SoftwareBitmap sbitmap, string lang)
        {
            //OCRを実行する
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language(lang));
            if (engine != null)
            {
                string str = "";
                var result = await engine.RecognizeAsync(sbitmap);
                foreach (var line in result.Lines)
                {
                    str += line.Text + "\n";
                    textList.Add(line.Text);
                    double startX = line.Words[0].BoundingRect.X;
                    double endX = line.Words[line.Words.Count - 1].BoundingRect.X + line.Words[line.Words.Count - 1].BoundingRect.Width;
                    xList.Add((int)startX);
                    yList.Add((int)line.Words[0].BoundingRect.Y);
                    widthList.Add((int)(endX - startX));
                    heightList.Add((int)line.Words[0].BoundingRect.Height);
                }

                return str;
            }
            return null;
        }

        public static List<string> GetTextList()
        {
            return textList;
        }

        public static List<int> GetXList()
        {
            return xList;
        }

        public static List<int> GetYList()
        {
            return yList;
        }

        public static List<int> GetWidthList()
        {
            return widthList;
        }

        public static List <int> GetHeightList()
        {
            return heightList;
        }
    }
}
