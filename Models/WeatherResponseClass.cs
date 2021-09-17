using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QnaLuisBot.Models
{
    public class WeatherResponseClass
    {
        public double temperature { get; set; }
        public string weatherCondition { get; set; }
        public string city { get; set; }
        public string country  { get; set; }
        public string errorMessage  { get; set; }
    }
}
