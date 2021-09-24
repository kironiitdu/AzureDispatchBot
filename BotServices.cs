// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QnaLuisBot.Models;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            //Dispatch = new LuisRecognizer(new LuisApplication(
            //    configuration["LuisAppId"],
            //    configuration["LuisAPIKey"],
            //    $"https://{configuration["LuisAPIHostName"]}.api.cognitive.microsoft.com"),
            //    new LuisPredictionOptions { IncludeAllIntents = true, IncludeInstanceData = true },
            //    true);

            //SampleQnA = new QnAMaker(new QnAMakerEndpoint
            //{
            //    KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
            //    EndpointKey = configuration["QnAEndpointKey"],
            //    Host = configuration["QnAEndpointHostName"]
            //});
        }

        public LuisRecognizer Dispatch { get; private set; }
        public QnAMaker SampleQnA { get; private set; }

        public async Task<WeatherResponseClass> GetWeatherUpdate(string cityName)
        {
            WeatherResponseClass _objResponse = new WeatherResponseClass();
            try
            {


                if (string.IsNullOrEmpty(cityName))
                {

                    _objResponse.errorMessage = "City Name is required!";
                    return _objResponse;
                }
                //Get Weather APP ID:
                var appID = "003cbdeca4c56c95d240ef36f7433dc8";
                // Call  API
                HttpClient _client = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}", cityName.Trim(), appID));
                HttpResponseMessage response = await _client.SendAsync(newRequest);

                //Check status code and retrive response

                if (response.IsSuccessStatusCode)
                {

                    CustomWeatherClass objSearchResponse = JsonConvert.DeserializeObject<CustomWeatherClass>(await response.Content.ReadAsStringAsync());

                    var calculatedTemp = objSearchResponse.main.temp - 273.15;
                    _objResponse.temperature = calculatedTemp;
                    _objResponse.weatherCondition = objSearchResponse.weather[0].description;
                    _objResponse.city = objSearchResponse.name;
                    _objResponse.country = objSearchResponse.sys.country;



                    return _objResponse;
                }
                else
                {
                    var result_string = await response.Content.ReadAsStringAsync();
                    _objResponse.errorMessage = result_string;
                    return _objResponse;
                }

            }
            catch (Exception ex)
            {
                _objResponse.errorMessage = ex.Message;
                return _objResponse;
            }

        }

        public async Task<EmployeeAPIResponse> GetEmployee(int employee_id)
        {
            try
            {
                EmployeeAPIResponse _objResponse = new EmployeeAPIResponse();
                if (string.IsNullOrEmpty(employee_id.ToString()))
                {

                    _objResponse.message = "Employee Name / ID is required!";
                    return _objResponse;
                }

                // Call  API
                HttpClient _client = new HttpClient();
                HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("http://dummy.restapiexample.com/api/v1/employee/{0}", employee_id));
                HttpResponseMessage response = await _client.SendAsync(newRequest);

                //Check status code and retrive response

                if (response.IsSuccessStatusCode)
                {

                    EmployeeAPIResponse objSearchResponse = JsonConvert.DeserializeObject<EmployeeAPIResponse>(await response.Content.ReadAsStringAsync());
                    _objResponse.status = objSearchResponse.status;
                    _objResponse.data = objSearchResponse.data;
                    _objResponse.message = objSearchResponse.message;
                    return _objResponse;
                }
                else
                {
                    var result_string = await response.Content.ReadAsStringAsync();
                    _objResponse.message = result_string;
                    _objResponse.data.employee_name = "Tiger Nixon";
                    _objResponse.data.employee_salary = 320800;
                    return _objResponse;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
