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
        public const string JP = "ja-JP";
        public const string EN = "en-US";
    }
  
    internal class CharacterRecognizer
    {      
        public static async Task<string> RunOcr(SoftwareBitmap sbitmap, string lang)
        {                     
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language(lang));
            if(engine != null ) 
            {
                string str = "";
                var result = await engine.RecognizeAsync(sbitmap);
                foreach( var line in result.Lines )
                {
                    str += line.Text + "\n";                    
                }
                return str;
            }
            return null;
        }
    }
}
