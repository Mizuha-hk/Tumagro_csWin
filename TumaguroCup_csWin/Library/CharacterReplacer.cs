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
            byte[] bytes = await ConvertSoftwareBitmapToByte(softwareBitmap);
            Array.Reverse(bytes);
            string postStr = Convert.ToBase64String(bytes);

            JsonData jsonData = new JsonData
            {
                text = CharacterRecognizer.GetTextList(),
                x = CharacterRecognizer.GetXList(),
                y = CharacterRecognizer.GetYList(),
                width = CharacterRecognizer.GetWidthList(),
                height = CharacterRecognizer.GetHeightList(),
                base64ByteImage = postStr
            };
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            File.WriteAllText(@"C:\Users\kouse\Videos\JsonData.json", json);

            string resultStr;
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:8080/api", content);
                resultStr = response.Content.ReadAsStringAsync().Result;
            }

            outSBitmap = await ConvertByteToSoftwareBitmap(Convert.FromBase64String(resultStr));
            return outSBitmap;
        }

        //SoftwareBitmapをbyte[]に変換するメソッド
        private static async Task<byte[]> ConvertSoftwareBitmapToByte(SoftwareBitmap softwareBitmap)
        {
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

                // IRandomAccessStreamをSoftwareBitmapに変換
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                return softwareBitmap;
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
        public string base64ByteImage { get; set; }
    }
}
