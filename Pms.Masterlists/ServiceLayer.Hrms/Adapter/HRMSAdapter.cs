using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pms.Masterlists.ServiceLayer.Hrms.Exceptions;

namespace Pms.Masterlists.ServiceLayer.Hrms.Adapter
{
    public class HRMSAdapter
    {
        private readonly HttpClient _client;
        private readonly HRMSParameter _parameter;

        public HRMSAdapter(HRMSParameter hrmsParameter)
        {
            _client = new();
            _client.Timeout = TimeSpan.FromSeconds(30d);
            _parameter = hrmsParameter;
        }

        public async Task<T?> GetEmployeeFromHRMS<T>(string eeId, string site)
        {
            try
            {
                _parameter.BodyArgs["field"] = "acctg";
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);

                var response = await _client.PostAsync(_parameter.Urls[site], content);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                    HRMSResponse<T>? employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee is not null)
                        return employee.message[0];
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case (System.Net.HttpStatusCode)400:
                            throw new InvalidRequestException();
                        case (System.Net.HttpStatusCode)404:
                            throw new EmployeeNotFoundException(eeId);
                    }
                }
            }
            catch (InvalidRequestException) { }
            catch (EmployeeNotFoundException) { }
            return default;
        }

        public async Task<IEnumerable<T>?> GetNewlyHiredEmployeesFromHRMS<T>(DateTime fromDate, string site)
        {
            try
            {
                _parameter.BodyArgs["field"] = "newlyhired";
                _parameter.BodyArgs["joined_date_start"] = fromDate.ToString("yyyy-MM-dd");
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);

                var response = await _client.PostAsync(_parameter.Urls[site], content);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                    HRMSResponse<T>? employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee is not null)
                        return employee.message;
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case (System.Net.HttpStatusCode)400:
                            throw new InvalidRequestException();
                        //case (System.Net.HttpStatusCode)404:
                        //    throw new EmployeeNotFoundException(eeId);
                    }
                }
            }
            catch (InvalidRequestException) { }
            catch (EmployeeNotFoundException) { }
            return default;
        }


        public async Task<IEnumerable<T>?> GetResignedEmployeesFromHRMS<T>(DateTime fromDate, string site)
        {
            try
            {
                _parameter.BodyArgs["field"] = "resigned";
                _parameter.BodyArgs["resigned_date_start"] = fromDate.ToString("yyyy-MM-dd");
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);

                var response = await _client.PostAsync(_parameter.Urls[site], content);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings();
                    jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                    HRMSResponse<T>? employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee is not null)
                        return employee.message;
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case (System.Net.HttpStatusCode)400:
                            throw new InvalidRequestException();
                        //case (System.Net.HttpStatusCode)404:
                        //    throw new EmployeeNotFoundException(eeId);
                    }
                }
            }
            catch (InvalidRequestException) { }
            catch (EmployeeNotFoundException) { }
            return default;
        }

        public class HRMSResponse<T>
        {
            public string code = "";
            public List<T> message = new();
        }
    }
}
