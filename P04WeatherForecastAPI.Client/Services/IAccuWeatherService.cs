using P04WeatherForecastAPI.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.Services
{
    public interface IAccuWeatherService
    {
        Task<City[]> GetLocations(string locationName);
        Task<Weather> GetCurrentConditions(string cityKey);
        Task<WeatherForecastResponse> GetFiveDayForecast(string cityKey);
        Task<AlarmResponse[]> GetAlarms(string cityKey);
        Task<IndexResponse[]> GetIndeces(string cityKey);
        Task<History[]> GetHistories(string cityKey);
    }
}
