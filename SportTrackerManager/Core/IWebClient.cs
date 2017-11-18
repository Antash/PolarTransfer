using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrackerManager.Core
{
    public interface IWebClient
    {
        Task<string> GetPageDataAsync(Uri uri);

        Task<bool> PostFormDataAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> data);

        Task<string> PostFormMultipartDataAsync(Uri uri, IEnumerable<KeyValuePair<string, object>> data);
    }
}
