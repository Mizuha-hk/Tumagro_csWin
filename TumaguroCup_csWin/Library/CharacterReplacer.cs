using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Newtonsoft.Json;

namespace TumaguroCup_csWin.Library
{
    internal class CharacterReplacer
    {
        //メインウィンドウが呼び出すメソッド
        public static async Task<SoftwareBitmap> ReplaceCharacterAsync(string text, SoftwareBitmap softwareBitmap)
        {
            SoftwareBitmap outSBitmap;
            //SoftwareBitmapからpng形式でエンコードされたbyte[]を取得
            byte[] bytes = await ConvertSoftwareBitmapToByte(softwareBitmap);
            //byte[]をBase64でエンコードし文字列化
            string postStr = Convert.ToBase64String(bytes);

            //APIにPOSTするjsonの用意
            JsonData jsonData = new JsonData
            {
                text = CharacterRecognizer.GetTextList(),
                x = CharacterRecognizer.GetXList(),
                y = CharacterRecognizer.GetYList(),
                width = CharacterRecognizer.GetWidthList(),
                height = CharacterRecognizer.GetHeightList(),
                imageWidth = softwareBitmap.PixelWidth,
                imageHeight = softwareBitmap.PixelHeight,
                base64ByteImage = postStr
            };
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            //POST&Response
            string resultStr;
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:8080/api", content);
                resultStr = response.Content.ReadAsStringAsync().Result;
            }

            //受け取った文字列をBase64でデコードし、取得したbyte[]をpng形式でデコードしてSoftwareBitmapを取得
            Debug.WriteLine("response:\n" + resultStr);
            outSBitmap = await ConvertByteToSoftwareBitmap(Convert.FromBase64String(resultStr));
            return outSBitmap;
        }

        //SoftwareBitmapをbyte[]に変換するメソッド
        private static async Task<byte[]> ConvertSoftwareBitmapToByte(SoftwareBitmap softwareBitmap)
        {
            softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight);
            byte[] byteArray;
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();
                byteArray = new byte[stream.Size];
                await stream.AsStream().ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        //byte[]をSoftwareBitmapに変換するメソッド
        public static async Task<SoftwareBitmap> ConvertByteToSoftwareBitmap(byte[] bytes)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                try
                {
                    // IRandomAccessStreamをSoftwareBitmapに変換
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, stream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                    return softwareBitmap;
                }
                catch (Exception)
                {
                    Debug.WriteLine("decode image error!");
                    return null;
                }
            }
        }
    }

    internal class JsonData
    {
        public List<string> text { get; set; }
        public List<int> x { get; set; }
        public List<int> y { get; set; }
        public List<int> width { get; set; }
        public List<int> height { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string base64ByteImage { get; set; }
    }
}
