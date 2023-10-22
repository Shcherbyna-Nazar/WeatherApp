using System.Windows;
using P04WeatherForecastAPI.Client.Services;
using P04WeatherForecastAPI.Client.ViewModels;

namespace P04WeatherForecastAPI.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new AccuWeatherService()); // Initialize your view model with a service.
        }
    }
}
