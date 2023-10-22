
namespace P04WeatherForecastAPI.Client.Models
{
    public class Alarm
    {
        public string AlarmType { get; set; }
        public AlarmValue Value { get; set; }
        public Day Day { get; set; }
        public Night Night { get; set; }
    }
}