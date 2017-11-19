using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public interface IWebClient
    {
        Task<bool> DeleteAsync(Uri uri);

        Task<string> GetPageDataAsync(string url);

        Task<string> GetPageDataAsync(Uri uri);

        Task<bool> TryPostFormDataAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> data);

        Task<string> PostFormDataAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> data);

        Task<string> PostFormMultipartDataAsync(Uri uri, IEnumerable<KeyValuePair<string, object>> data);
    }
}
