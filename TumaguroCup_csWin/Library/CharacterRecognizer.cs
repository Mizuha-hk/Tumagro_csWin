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
        private static List<Rect> rectList = new List<Rect>();
        public static async Task<string> RunOcr(SoftwareBitmap sbitmap, string lang)
        {                     
            //OCRを実行する
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language(lang));
            if(engine != null ) 
            {
                string str = "";
                var result = await engine.RecognizeAsync(sbitmap);
                foreach( var line in result.Lines )
                {
                    str += line.Text + "\n";
                    double startX = line.Words[0].BoundingRect.X;
                    double endX = line.Words[-1].BoundingRect.X + line.Words[-1].BoundingRect.Width;
                    rectList.Add(new Rect(startX, line.Words[0].BoundingRect.Y, line.Words[0].BoundingRect.Height, endX - startX));
                }

                return str;
            }
            return null;
        }

        public static List<Rect> GetRectList()
        {
            return rectList;
        }
    }
}
