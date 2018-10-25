using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public class WebClient : IWebClient, IDisposable
    {
        private readonly CookieContainer cookies;
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;

        public WebClient(Uri baseUri)
        {
            cookies  = new CookieContainer();
            handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            client = new HttpClient(handler)
            {
                BaseAddress = baseUri
            };
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        }

        public async Task<bool> DeleteAsync(Uri uri)
        {
            var responce = await client.DeleteAsync(uri);
            return responce.IsSuccessStatusCode;
        }

        public async Task<string> GetPageDataAsync(string url)
        {
            return await client.GetStringAsync(url);
        }

        public async Task<string> GetPageDataAsync(Uri uri)
        {
            return await client.GetStringAsync(uri);
        }

        public async Task<bool> TryPostFormDataAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> data)
        {
            var responce = await client.PostAsync(uri, new FormUrlEncodedContent(data));
            return responce.StatusCode != HttpStatusCode.BadRequest;
        }

        public async Task<string> PostFormDataAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> data)
        {
            var responce = await client.PostAsync(uri, new FormUrlEncodedContent(data));
            return await responce.Content.ReadAsStringAsync();
        }

        public async Task<string> PostFormMultipartDataAsync(Uri uri, IEnumerable<KeyValuePair<string, object>> data)
        {
            var boundary = $"--------boundary{Guid.NewGuid()}";
            using (var content = new MultipartFormDataContent(boundary))
            {
                foreach (var item in data)
                {
                    switch (item.Value)
                    {
                        case ValueTuple<byte[], string> fileData:
                            var fileContent = new ByteArrayContent(fileData.Item1);
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                            content.Add(fileContent, item.Key, fileData.Item2);
                            break;
                        case string s:
                            content.Add(new StringContent(s), item.Key);
                            break;
                        default:
                            throw new ArgumentException(nameof(data));
                    }
                }

                var responce = await client.PostAsync(uri, content);
                return await responce.Content.ReadAsStringAsync();
            }
        }

        public void Dispose()
        {
            client.Dispose();
            handler.Dispose();
        }
    }
}
