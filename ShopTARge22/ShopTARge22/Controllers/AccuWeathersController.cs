﻿using Microsoft.AspNetCore.Mvc;
using ShopTARge22.Core.Dto.AccuWeatherDtos;
using ShopTARge22.Core.ServiceInterface;
using ShopTARge22.Models.AccuWeatherForecast;
using ShopTARge22.Models.AccuWeathers;

namespace ShopTARge22.Controllers
{
    public class AccuWeathersController : Controller
    {
        private readonly IAccuWeatherServices _accuWeatherServices;
        public AccuWeathersController(IAccuWeatherServices accuWeatherServices)
        {
            _accuWeatherServices = accuWeatherServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchCity(SearchCityViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Weather", "AccuWeathers", new { city = model.CityName });
            }
            return View(model);
        }

       
        [HttpGet]
        public async Task<IActionResult> Weather(string city)
        {
            try
            {
                string cityKey = await _accuWeatherServices.GetSubmittedCityKey(city);
                AccuWeatherResponseDto dto = await _accuWeatherServices.GetWeatherInfo(cityKey);

                if (dto != null)
                {
                    AccuWeatherViewModel vm = new();
                    vm.CityName = city;
                    vm.Temp = dto.Temp;
                    vm.RealFeelTemp = dto.RealFeelTemp;
                    vm.RelativeHum = dto.RelativeHum;
                    vm.PressureM = dto.PressureM;
                    vm.WindSpeed = dto.WindSpeed;
                    vm.WeatherText = dto.WeatherText;

                    return View(vm);
                }
                else
                {
                    Console.WriteLine("Error: AccuWeatherResponseDto is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            // Rediricet to Index on error
            return View("Index");

        }
         
    }
}
