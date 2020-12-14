using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace CurrencyExchange.Services
{
    public class DataService : IDisposable, IDataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly HttpClient _httpClient;
        private const string path = "observations/group/FX_RATES_DAILY/json";
        public DataService(HttpClient httpClient, ILogger<DataService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.bankofcanada.ca/valet/");
        }
        public async Task<IDictionary<(string, string), decimal>> GetForexRateFromBankOfCanada(string date, CancellationToken cancellationToken = default)
        {
            var equations = new Dictionary<(string, string), decimal>();
            try
            {

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                dynamic observe = await GetDynamicData(date, cancellationToken);
                stopWatch.Stop();
                _logger.LogInformation($"it takes {stopWatch.ElapsedMilliseconds / 1000} sec to retrieve forex rates\r\n");


                foreach (var item in observe)
                {

                    if (item.Name == "d") continue;
                    var equation = CreateEquation((string)item.Name);
                    var success = decimal.TryParse((string)item.First["v"], out var value);
                    equations.Add(equation, value);
                }
                return equations;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return equations;
            }
        }
        /// <summary>
        /// Note: dynamic objects resolve on run time
        /// </summary>
        /// <param name="date"></param>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>a dynamic object containing forex rates</returns>
        public async Task<dynamic> GetDynamicData(string date, CancellationToken cancellationToken)
        {
            var pathAndQuery = $"{path}?start_date={date}&end_date={date}";
            var response = await _httpClient.GetAsync(pathAndQuery, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            JObject jObj = JObject.Parse(content);
            var observation = jObj["observations"].First;
            //var ser = JsonConvert.SerializeObject(jObj["observations"]);
            //dynamic observation = JArray.Parse(ser);
            //dynamic obj = JsonConvert.DeserializeObject(content);

            ////dynamic observe = observations.FirstOrDefault(x => x["d"].ToString().Equals(date.Trim()));
            return observation;


        }


        public (string, string) CreateEquation(string name)
        {
            string name1 = name.Trim().Substring(2, 3);
            string name2 = name.Trim().Substring(5, 3);
            return (name1, name2);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}



