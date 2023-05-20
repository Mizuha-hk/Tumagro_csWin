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
    internal class CharacterRecognizer
    {
        /*
        private async Task<SoftwareBitmap> ConvertBitmapImageToSoftwareBitmap(BitmapImage bitmapImage)
        {
            SoftwareBitmap sbitmap = null;

            using (var stream = await bitmapImage.OpenR)
            {

                // Create the decoder from the stream
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // Get the SoftwareBitmap representation of the file
                sbitmap = await decoder.GetSoftwareBitmapAsync();
            }
            return sbitmap;
        }
        */

        private static async Task<string> RunOcr(SoftwareBitmap sbitmap)
        {
            //var sbitmap = await ConvertBitmapImageToSoftwareBitmap(bimage);
            //OCRを実行する
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("ja-JP"));
            var result = await engine.RecognizeAsync(sbitmap);
            return result.Text;
        }
    }
}
