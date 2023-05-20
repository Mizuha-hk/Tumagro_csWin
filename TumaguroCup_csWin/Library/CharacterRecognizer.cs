using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace TumaguroCup_csWin.Library
{
    public readonly struct Language
    {
        public static string JP = "ja-JP";
        public static string EN = "en-US";
    }
  
    internal class CharacterRecognizer
    {      
        public static async Task<string> RunOcr(SoftwareBitmap sbitmap, string lang)
        {                     
            //OCRを実行する
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language(lang));
            if(engine != null ) 
            {
                var result = await engine.RecognizeAsync(sbitmap);
                return result.Text;
            }
            return null;
        }
    }
}
