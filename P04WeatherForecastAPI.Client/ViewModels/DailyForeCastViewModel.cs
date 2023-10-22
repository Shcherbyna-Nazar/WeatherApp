using P04WeatherForecastAPI.Client.Models;
using System;

namespace P04WeatherForecastAPI.Client.ViewModels
{
    public class DailyForecastViewModel
    {
        public DateTime? Date { get; set; }
        public TemperatureForecast Temperature { get; set; }
        public Day Day { get; set; }

        public DailyForecastViewModel(DailyForecast forecast)
        {
            Date = forecast.Date;
            Temperature = forecast.Temperature;
            Day = forecast.Day;
        }
    }
}
