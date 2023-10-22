using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P04WeatherForecastAPI.Client.Models;
using P04WeatherForecastAPI.Client.Services;

namespace P04WeatherForecastAPI.Client.ViewModels
{

    public partial class MainWindowViewModel : INotifyPropertyChanged
    {
        private IAccuWeatherService _accuWeatherService;
        private CityViewModel _selectedCity;
        private WeatherViewModel _weather;
        private ObservableCollection<DailyForecastViewModel> _dailyForecasts;
        private ObservableCollection<IndexResponse> _indices;
        private string _maxMinTemperature;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CityViewModel> Cities { get; set; }

        public ObservableCollection<DailyForecastViewModel> DailyForecasts
        {
            get => _dailyForecasts;
            set
            {
                if (_dailyForecasts != value)
                {
                    _dailyForecasts = value;
                    OnPropertyChanged(nameof(DailyForecasts));
                }
            }
        }

        public ObservableCollection<IndexResponse> Indices
        {
            get => _indices;
            set
            {
                if (_indices != value)
                {
                    _indices = value;
                    OnPropertyChanged(nameof(Indices));
                }
            }
        }

        public string MaxMinTemperature
        {
            get => _maxMinTemperature;
            set
            {
                if (_maxMinTemperature != value)
                {
                    _maxMinTemperature = value;
                    OnPropertyChanged(nameof(MaxMinTemperature));
                }
            }
        }

        public CityViewModel SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
                    OnPropertyChanged(nameof(SelectedCity));
                    LoadData();
                }
            }
        }

        public WeatherViewModel WeatherData
        {
            get => _weather;
            set
            {
                if (_weather != value)
                {
                    _weather = value;
                    OnPropertyChanged(nameof(WeatherData));
                }
            }
        }

        public MainWindowViewModel(IAccuWeatherService accuWeatherService)
        {
            _accuWeatherService = accuWeatherService;
            Cities = new ObservableCollection<CityViewModel>();
            DailyForecasts = new ObservableCollection<DailyForecastViewModel>();
            Indices = new ObservableCollection<IndexResponse>();
        }

        [RelayCommand]
        public async void LoadCities(string locationName)
        {
            // podejście nr 2:
            var cities = await _accuWeatherService.GetLocations(locationName);
            Cities.Clear();
            foreach (var city in cities)
                Cities.Add(new CityViewModel(city));
        }

        private async void LoadData()
        {
            if (SelectedCity != null)
            {
                Weather weather = await _accuWeatherService.GetCurrentConditions(SelectedCity.Key);
                WeatherData = new WeatherViewModel(weather);
                LoadDailyForecasts();
                LoadIndices();
                LoadHistory();
            }
        }

        private async void LoadDailyForecasts()
        {
            if (SelectedCity != null)
            {
                // You should implement a method to retrieve daily forecasts based on the selected city.
                var response = await _accuWeatherService.GetFiveDayForecast(SelectedCity.Key);

                DailyForecasts.Clear();
                foreach (var dailyForecast in response.DailyForecasts)
                {
                    string maxTempStr = dailyForecast.Temperature.Maximum.Value.Replace('.', ',');
                    if (double.TryParse(maxTempStr, out double maxTempFahrenheit))
                    {
                        double maxTempCelsius = (maxTempFahrenheit - 32) * 5 / 9;
                        dailyForecast.Temperature.Maximum.Value = $"{maxTempCelsius:0.0}°C";
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing maximum temperature: {dailyForecast.Temperature.Maximum.Value}");
                    }

                    string minTempStr = dailyForecast.Temperature.Minimum.Value.Replace('.', ',');
                    if (double.TryParse(minTempStr, out double minTempFahrenheit))
                    {
                        double minTempCelsius = (minTempFahrenheit - 32) * 5 / 9;
                        dailyForecast.Temperature.Minimum.Value = $"{minTempCelsius:0.0}°C";
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing minimum temperature: {dailyForecast.Temperature.Minimum.Value}");
                    }
                    DailyForecasts.Add(new DailyForecastViewModel(dailyForecast));
                }

            }
        }

        private async void LoadIndices()
        {
            if (SelectedCity != null)
            {
                var indices = await _accuWeatherService.GetIndeces(SelectedCity.Key);
                Indices.Clear();
                foreach (var index in indices)
                {
                    Indices.Add(index);
                }
            }
        }

        private async void LoadHistory()
        {
            if (SelectedCity != null)
            {
                var history = await _accuWeatherService.GetHistories(SelectedCity.Key);
                if (history.Length > 0)
                {
                    double maxTemp = history[0].Temperature.Metric.Value;
                    double minTemp = history[0].Temperature.Metric.Value;
                    foreach (var entry in history)
                    {
                        if (entry.Temperature.Metric.Value > maxTemp)
                        {
                            maxTemp = entry.Temperature.Metric.Value;
                        }
                        if (entry.Temperature.Metric.Value < minTemp)
                        {
                            minTemp = entry.Temperature.Metric.Value;
                        }
                    }
                    MaxMinTemperature = $"Max Temp (Last 6 Hours): {maxTemp}°C, Min Temp: {minTemp}°C";
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
