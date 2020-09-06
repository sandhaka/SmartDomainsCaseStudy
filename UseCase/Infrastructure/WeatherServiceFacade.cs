using System;
using UseCase.Domain;

namespace UseCase.Infrastructure
{
    /// <summary>
    /// A fake version of weather service
    /// </summary>
    public static class WeatherServiceFacade
    {
        public static string Forecast(TransportTruck transportTruck)
        {
            throw new NotImplementedException();
        }
    }
}