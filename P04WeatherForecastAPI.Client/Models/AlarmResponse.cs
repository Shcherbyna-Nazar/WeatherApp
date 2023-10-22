using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04WeatherForecastAPI.Client.Models
{
    public class AlarmResponse
    {
        public DateTime Date { get; set; }
        public int EpochDate { get; set; }
        public List<Alarm> Alarms { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }
}