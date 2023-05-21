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
        //メインウィンドウが呼び出すメソッド
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
                //Cv2.ImWrite("" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png", captureMat);
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
        private static void DrawingRectangle(Mat mat, Windows.Foundation.Rect lineRect)
        {
            Cv2.Rectangle(mat, new OpenCvSharp.Point((int)lineRect.X, (int)lineRect.Y), new OpenCvSharp.Point((int)(lineRect.X + lineRect.Width), (int)(lineRect.Y + lineRect.Height)), new Scalar(0, 255, 0), -1);
        }

        //画像の文字列の位置に翻訳した文字を重ねるメソッド
        private static void DrawingText(Mat mat, string text, Windows.Foundation.Rect lineRect)
        {
            //テキスト画像用のBitmapを用意
            var textBitmap = new Bitmap((int)lineRect.Width, (int)lineRect.Height);
            Graphics g = Graphics.FromImage(textBitmap);
            //文字のフォント、サイズ、色を指定
            g.DrawString(text, new Font("MS UI Gothic", 11), Brushes.Black, 0, 0);
            var img2 = BitmapConverter.ToMat(textBitmap);
            //matの指定した位置にテキスト画像を重ねる
            mat[(int)lineRect.X, (int)(lineRect.X +lineRect.Width), (int)lineRect.Y, (int)(lineRect.Y + lineRect.Height)] = img2;
            textBitmap.Dispose();
            g.Dispose();
            img2.Dispose();
        }
    }
}
