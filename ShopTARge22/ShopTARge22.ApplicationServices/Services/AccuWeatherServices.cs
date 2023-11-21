﻿using ShopTARge22.Core.ServiceInterface;
using ShopTARge22.Core.Dto.AccuWeatherDtos;
using Newtonsoft.Json;

namespace ShopTARge22.ApplicationServices.Services
{
    public class AccuWeatherServices : IAccuWeatherServices
    {

        private readonly string apiKey = "myapikey";
        public async Task<string?> GetSubmittedCityKey(string city)
        {
            string resourceUrl = "https://dataservice.accuweather.com/locations/v1/cities/search?";
            string apiCallUrl = $"{resourceUrl}apikey={apiKey}&q={city}";
            using (HttpClient client = new())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiCallUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var cityInfoList = ParseJsonResponse(responseBody);

                        if (cityInfoList.Count > 0)
                        {
                            string cityKey = cityInfoList[0]["Key"].ToString();
                            //Console.WriteLine($"Key: {cityKey}");
                            return cityKey;
                        }
                        else
                        {
                            Console.WriteLine("City key not found in the response.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return null;
        }

        // Method to parse JSON response using Newtonsoft.Json
        private List<Dictionary<string, object>> ParseJsonResponse(string responseBody)
        {
            // Deserialize to a list of dictionaries
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseBody);
        }

        public async Task<AccuWeatherResponseDto> GetWeatherInfo(string cityKey)
        {
            string resourceUrl = "http://dataservice.accuweather.com/currentconditions/v1/";
            string apiCallUrl = $"{resourceUrl}{cityKey}?apikey={apiKey}&details=true";

            using (HttpClient client = new())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiCallUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        List<AccuWeatherResponseRootDto> weatherResponse = JsonConvert.DeserializeObject<List<AccuWeatherResponseRootDto>>(responseBody);
                      
                        if (weatherResponse.Count > 0)
                        {
                            AccuWeatherResponseDto dto = new();
                            dto.WeatherText = weatherResponse[0].WeatherText;
                            dto.RelativeHum = weatherResponse[0].RelativeHumidity;

                            // Check if Temperature and Metric is not null before accessing its properties
                            if (weatherResponse[0].Temperature != null && weatherResponse[0].Temperature.Metric != null)
                            {
                                dto.Temp = weatherResponse[0].Temperature.Metric.Value;
                            }
                            else
                            {
                                Console.WriteLine("Error: TemperatureR or Metric is null.");
                                dto.Temp = 0.0;
                            }

                            // Check if RealFeelTemperature and Metric is not null
                            if (weatherResponse[0].RealFeelTemperature != null && weatherResponse[0].RealFeelTemperature.Metric != null)
                            {
                                dto.RealFeelTemp = weatherResponse[0].RealFeelTemperature.Metric.Value;
                            }

                            else
                            {
                                // Handle the case where RealFeelTemperature or its Metric property is null
                                Console.WriteLine("Error: RealFeelTemperature or Metric is null.");
                                dto.RealFeelTemp = 0.0; 
                            }

                            dto.PressureM = weatherResponse[0].Pressure.Metric.Value;

                            if (weatherResponse[0].Wind != null && weatherResponse[0].Wind.Speed != null && weatherResponse[0].Wind.Speed.Metric != null)
                            {
                                dto.WindSpeed = weatherResponse[0].Wind.Speed.Metric.Value;
                            }

                            else
                            {
                                dto.WindSpeed = 0.0;
                                Console.WriteLine("Error: Wind properties are null.");
                            }

                            return dto;
                        }

                        else
                        {
                            Console.WriteLine("Error: Empty response array.");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }

                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
            return null;
        }
    }
}
