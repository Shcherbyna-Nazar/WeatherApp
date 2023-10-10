using P04WeatherForecastAPI.Client.Models;
using P04WeatherForecastAPI.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P04WeatherForecastAPI.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AccuWeatherService accuWeatherService;
        public MainWindow()
        {
            InitializeComponent();
            accuWeatherService = new AccuWeatherService();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            City[] cities = await accuWeatherService.GetLocations(txtCity.Text);

            // standardowy sposób dodawania elementów
            //lbData.Items.Clear();
            //foreach (var c in cities)
            //    lbData.Items.Add(c.LocalizedName);

            // teraz musimy skorzystac z bindowania danych bo chcemy w naszej kontrolce przechowywac takze id miasta 
            lbData.ItemsSource = cities;
        }

        private async void lbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCity = (City)lbData.SelectedItem;
            if (selectedCity != null)
            {
                var weather = await accuWeatherService.GetCurrentConditions(selectedCity.Key);
                lblCityName.Content = selectedCity.LocalizedName;
                double tempValue = weather.Temperature.Metric.Value;
                lblTemperatureValue.Content = $"{tempValue}°C";

                WeatherForecastResponse forecast = await accuWeatherService.GetFiveDayForecast(selectedCity.Key);

                foreach (var dailyForecast in forecast.DailyForecasts)
                {
                    // Replace periods with commas for maximum temperature
                    string maxTempStr = dailyForecast.Temperature.Maximum.Value.Replace('.', ',');
                    if (double.TryParse(maxTempStr, out double maxTempFahrenheit))
                    {
                        double maxTempCelsius = (maxTempFahrenheit - 32) * 5 / 9;
                        dailyForecast.Temperature.Maximum.Value = $"{maxTempCelsius:0.0}°C";
                    }
                    else
                    {
                        // Handle the parsing error, log it, or take appropriate action.
                        Console.WriteLine($"Error parsing maximum temperature: {dailyForecast.Temperature.Maximum.Value}");
                    }

                    // Replace periods with commas for minimum temperature
                    string minTempStr = dailyForecast.Temperature.Minimum.Value.Replace('.', ',');
                    if (double.TryParse(minTempStr, out double minTempFahrenheit))
                    {
                        double minTempCelsius = (minTempFahrenheit - 32) * 5 / 9;
                        dailyForecast.Temperature.Minimum.Value = $"{minTempCelsius:0.0}°C";
                    }
                    else
                    {
                        // Handle the parsing error, log it, or take appropriate action.
                        Console.WriteLine($"Error parsing minimum temperature: {dailyForecast.Temperature.Minimum.Value}");
                    }
                }

                lbFiveDayForecast.ItemsSource = forecast.DailyForecasts;

                // Retrieve and display alarms
                IndexResponse[] indexResponse = await accuWeatherService.GetIndeces(selectedCity.Key);

                lbIndices.ItemsSource = indexResponse;



                History[] history = await accuWeatherService.GetHistories(selectedCity.Key);

                // Display maximum and minimum temperature from the history
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

                    lblMaxMinTemperature.Content = $"Max Temp (Last 6 Hours): {maxTemp}°C, Min Temp: {minTemp}°C";
                }
            }
        }

    }
}
