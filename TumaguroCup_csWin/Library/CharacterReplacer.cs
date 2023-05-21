using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using System.Drawing;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using OpenCvSharp.Extensions;

namespace TumaguroCup_csWin.Library
{
    internal class CharacterReplacer
    {
        public static async Task<SoftwareBitmap> ReplaceCharacterAsync(string text,SoftwareBitmap softwareBitmap)
        {
            SoftwareBitmap outSBitmap;
            using (Mat captureMat = await ConvertSoftwareBitmapToMatAsync(softwareBitmap))
            {
                List<Windows.Foundation.Rect> rectList = CharacterRecognizer.GetRectList();
                foreach (Windows.Foundation.Rect rect in rectList)
                {
                    DrawingRectangle(captureMat, rect);
                    DrawingText(captureMat, text, rect);
                }
                outSBitmap = await ConvertMatToSoftwareBitmapAsync(captureMat);
            }
            return outSBitmap;
        }

        //SoftwareBitmapをMatに変換するメソッド
        private static async Task<Mat> ConvertSoftwareBitmapToMatAsync(SoftwareBitmap softwareBitmap)
        {
            Mat mat = null;
            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();

                mat = Mat.FromStream(stream.AsStreamForRead(), ImreadModes.Color);
            }
            return mat;
        }

        //MatをSoftwareBitmapに変換するメソッド
        private static async Task<SoftwareBitmap> ConvertMatToSoftwareBitmapAsync(Mat mat)
        {
            SoftwareBitmap softwareBitmap = null;
            using (var stream = new MemoryStream())
            {
                mat.WriteToStream(stream);
                using (var raStream = stream.AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(raStream);
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync(
                        BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
            }
            return softwareBitmap;
        }

        //画像の文字列の位置を緑の長方形で塗りつぶすメソッド
        private static void DrawingRectangle(Mat mat, Windows.Foundation.Rect rect)
        {
            Cv2.Rectangle(mat, new OpenCvSharp.Point((int)rect.X, (int)rect.Y), new OpenCvSharp.Point((int)(rect.X + rect.Width), (int)(rect.Y + rect.Height)), new Scalar(0, 255, 0), -1);
        }

        //画像の文字列の位置に翻訳した文字を重ねるメソッド
        private static void DrawingText(Mat mat, string text, Windows.Foundation.Rect wordRect)
        {
            //テキスト画像用のBitmapを用意
            var textBitmap = new Bitmap((int)wordRect.Width, (int)wordRect.Y);
            Graphics g = Graphics.FromImage(textBitmap);
            g.DrawString(text, new Font("MS UI Gothic", 20), Brushes.Black, 0, 0);
            var img2 = BitmapConverter.ToMat(textBitmap);
            //matの指定した位置にテキスト画像を重ねる
            mat[(int)wordRect.X, (int)wordRect.Y, (int)wordRect.Width, (int)wordRect.Height] = img2;
            textBitmap.Dispose();
            g.Dispose();
            img2.Dispose();
        }
    }
}
