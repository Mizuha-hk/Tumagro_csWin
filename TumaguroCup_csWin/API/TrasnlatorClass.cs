using System.Text.Json;

namespace Translator
{
    static class Translator
    {
        /// <summary>
        /// 言語を翻訳します。
        /// 第三引数の原文の言語は省略可能です。
        /// </summary>
        /// <param name="target_lang">翻訳文の言語</param>
        /// <param name="sentence">原文</param>
        /// <param name="source_lang">原文の言語</param>
        /// <returns>翻訳文</returns>
        static public string Translate(string target_lang, string sentence, string source_lang = "")
        {
            var result = CallDeeplAPI.Post(target_lang, sentence, source_lang).Result;
            string resultStr = result.Content.ReadAsStringAsync().Result;

            // JSON文字列をパースしてJsonDocumentオブジェクトを作成
            JsonDocument jsonDocument = JsonDocument.Parse(resultStr);

            // 必要なデータにアクセス
            JsonElement rootElement = jsonDocument.RootElement;
            JsonElement translationsElement = rootElement.GetProperty("translations");
            JsonElement firstTranslationElement = translationsElement[0];
            string? text = firstTranslationElement.GetProperty("text").GetString();

            if(text == null)
            {
                return "Error: could't translate.";
            }
            else
            {
                return text;
            }
        }
    }

    static class CallDeeplAPI
    {
        static HttpClient httpClient = new HttpClient();
        static public async Task<HttpResponseMessage> Post(string target_lang, string sentence, string source_lang = "")
        {
            var multiForm = new MultipartFormDataContent();

            
            string apiUrl = "https://api-free.deepl.com/v2/translate";

            multiForm.Add(new StringContent("5ec95d97-cd5c-222b-8684-17eefdf88e7c:fx"), "auth_key");
            multiForm.Add(new StringContent(sentence), "text");
            multiForm.Add(new StringContent(target_lang), "target_lang");
            if(source_lang != "")
            {
                multiForm.Add(new StringContent(source_lang), "source_lang");
            }

            return await httpClient.PostAsync(apiUrl, multiForm);
        }

        
    }
}

namespace Language
{
    readonly struct Language
    {
        /// <summary>
        /// Japanese 日本語
        /// </summary>
        public static string JA = "JA";

        /// <summary>
        /// English 英語
        /// </summary>
        public static string EN = "EN";

        /// <summary>
        /// Germany ドイツ語
        /// </summary>
        public static string DE = "DE";

        /// <summary>
        /// French フランス語
        /// </summary>
        public static string FR = "ES";

        /// <summary>
        /// Italy イタリア語
        /// </summary>
        public static string IT = "IT";

        /// <summary>
        /// Polish ポーランド語
        /// </summary>
        public static string PL = "PL";

        /// <summary>
        /// Dutch オランダ語
        /// </summary>
        public static string NL = "NL";
    }
}
