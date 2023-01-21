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
            _client = new()
            {
                Timeout = TimeSpan.FromSeconds(30d)
            };
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
                        return employee.Message.ToList()[0];
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

        public async Task<T?> GetEmployeeFromHRMS<T>(string eeId, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                _parameter.BodyArgs["field"] = "acctg";
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);
                var response = await _client.PostAsync(_parameter.Urls[site], content, cancellationToken);
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    HRMSResponse<T>? employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee != null) return employee.Message.FirstOrDefault();
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

        public async Task<ICollection<T>> GetNewlyHiredEmployeesFromHRMS<T>(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                _parameter.BodyArgs["field"] = "newlyhired";
                _parameter.BodyArgs["joined_date_start"] = fromDate.ToString("yyyy-MM-dd");
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);
                var response = await _client.PostAsync(_parameter.Urls[site], content, cancellationToken);
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    HRMSResponse<T>? employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee != null) return employee.Message.ToList();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case (System.Net.HttpStatusCode)400:
                            throw new InvalidRequestException();
                    }
                }
            }
            catch (InvalidRequestException) { }
            catch (EmployeeNotFoundException) { }

            return Enumerable.Empty<T>().ToList();
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
                        return employee.Message.ToList();
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

        public async Task<ICollection<T>> GetResignedEmployeesFromHRMS<T>(DateTime fromDate, string site, CancellationToken cancellationToken = default)
        {
            try
            {
                _parameter.BodyArgs["field"] = "resigned";
                _parameter.BodyArgs["resigned_date_start"] = fromDate.ToString("yyyy-MM-dd");
                var content = new FormUrlEncodedContent(_parameter.BodyArgs);
                var response = await _client.PostAsync(_parameter.Urls[site], content, cancellationToken);
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var employee = JsonConvert.DeserializeObject<HRMSResponse<T>>(responseString, jsonSettings);
                    if (employee != null) return employee.Message.ToList();
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case (System.Net.HttpStatusCode)400:
                            throw new InvalidRequestException();
                    }
                }
            }
            catch (InvalidRequestException) { }
            catch (EmployeeNotFoundException) { }

            return Enumerable.Empty<T>().ToList();
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
                        return employee.Message.ToList();
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

        //public class HRMSResponse<T>
        //{
        //    public string code = "";
        //    public List<T> message = new();
        //}

        public class HRMSResponse<T>
        {
            [JsonProperty("code")]
            public string Code { get; set; } = string.Empty;

            [JsonProperty("message")]
            public IEnumerable<T> Message { get; set; } = Enumerable.Empty<T>();
        }
    }
}
