using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Pms.Timesheets.ServiceLayer.TimeSystem.Adapter
{
    public class TimeDownloaderAdapter
    {
        private readonly HttpClient _client = new();
        private readonly TimeDownloaderParameter _parameter;

        public TimeDownloaderAdapter(TimeDownloaderParameter parameter)
        {
            _client = new HttpClient { Timeout = TimeSpan.FromMinutes(2d) };
            _parameter = parameter;
        }

        public async Task<T?> GetPageContent<T>(DateTime date_from, DateTime date_to, int page, string payroll_code, string site)
        {
            _parameter.PostData.PayrollCode = payroll_code;
            _parameter.PostData.Page = page.ToString();
            _parameter.PostData.DateFrom = date_from.ToString("yyyy-MM-dd");
            _parameter.PostData.DateTo = date_to.ToString("yyyy-MM-dd");

            var dicc = new Dictionary<string, string>
            {
                { "postData", JsonConvert.SerializeObject(_parameter.PostData) }
            };

            var res = await _client.PostAsync(string.Format("{0}", _parameter.Urls[site]), new FormUrlEncodedContent(dicc));
            var resdeserialized = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());

            return resdeserialized;
        }

        public async Task<T?> GetSummary<T>(DateTime date_from, DateTime date_to, string payroll_code, string site)
        {
            _parameter.PostData.PayrollCode = payroll_code;
            _parameter.PostData.Page = "-1";
            _parameter.PostData.DateFrom = date_from.ToString("yyyy-MM-dd");
            _parameter.PostData.DateTo = date_to.ToString("yyyy-MM-dd");

            var dicc = new Dictionary<string, string>
            {
                { "postData", JsonConvert.SerializeObject(_parameter.PostData) }
            };

            var content = new FormUrlEncodedContent(dicc);

            var responseMessage = await _client.PostAsync(string.Format("{0}", _parameter.Urls[site]), content);
            string responseMessageContent = await responseMessage.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<T>(responseMessageContent);

            return responseDeserialized;
        }
    }
}
