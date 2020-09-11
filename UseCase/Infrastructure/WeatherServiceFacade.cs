using System;
using System.Linq;

namespace UseCase.Infrastructure
{
    /// <summary>
    /// A fake version of weather service
    /// </summary>
    public static class WeatherServiceFacade
    {
        private static readonly Random Random = new Random();
        
        public static string Forecast()
        {
            var wCodes = Enum.GetNames(typeof(WeatherCode));
            return wCodes[Random.Next(0, wCodes.Length)];
        }

        public static bool IsGoodWeather(WeatherCode weatherCode)
        {
            return new[] { WeatherCode.Sunny, WeatherCode.Dry, WeatherCode.Humid }.Contains(weatherCode);
        }
    }

    public enum WeatherCode
    {
        Sunny,
        Rainy, 
        Wet, 
        Humid, 
        Dry,  
        Foggy, 
        Windy, 
        Stormy
    }
}