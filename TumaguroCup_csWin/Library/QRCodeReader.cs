using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using ZXing;

namespace TumaguroCup_csWin.Library
{
    internal class QRCodeReader
    {
        public static async Task<string> QRCodeRead(SoftwareBitmap softwareBitmap)
        {
            /*
            // Image コントロールは、BGRA8 エンコードを使用し、プリマルチプライ処理済みまたはアルファ チャネルなしの画像しか受け取れない
            // 異なるフォーマットの場合は変換する☟
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8
                || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
            */

            // バーコード読み取り
            var _reader = new BarcodeReaderGeneric
            {
                AutoRotate = true,
                Options = { TryHarder = true }
            };
            // UWPではSoftwareBitmapかWriteableBitmapを渡す
            //ZXing.Result result = _reader.Decode(softwareBitmap);
            // ☟別スレッドでやるときも、作成済みのSoftwareBitmapインスタンスを渡してよい
            byte[] bytedata = await ConvertSoftwareBitmapToByte(softwareBitmap);
            Result result = _reader.Decode(bytedata, softwareBitmap.PixelWidth, softwareBitmap.PixelHeight, RGBLuminanceSource.BitmapFormat.Unknown);
            if(result != null)
            {
                return result.Text;
            }
            else
            {
                return null;
            }
        }

        //SoftwareBitmapからbyte[]への変換
        private static async Task<byte[]> ConvertSoftwareBitmapToByte(SoftwareBitmap softwareBitmap) {
            BitmapEncoder encoder;
            InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
            encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, ras);
            encoder.SetSoftwareBitmap(softwareBitmap);
            try
            {
                await encoder.FlushAsync();
            }
            catch(Exception e) { 
                Debug.WriteLine(e.Message);
            }
            ras.Seek(0);
            BitmapDecoder decoder;
            decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.BmpDecoderId, ras);
            PixelDataProvider provider = await decoder.GetPixelDataAsync();
            return provider.DetachPixelData();
        }
    }
}
