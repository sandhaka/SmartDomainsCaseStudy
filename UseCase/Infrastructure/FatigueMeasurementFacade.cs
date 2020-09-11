using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Option;

namespace UseCase.Infrastructure
{
    /// <summary>
    /// A fake version of Physical metrics measurement system service 
    /// </summary>
    public static class FatigueMeasurementFacade
    {
        private static readonly Random Random = new Random();
        
        public static double Measure(IEnumerable<DemoHistory> data)
        {
            var history = data.Reverse().ToList();

            // Last time effort
            var jt = history
                .FirstOrNone()
                .Map(d => d.ArrivalTime - d.DepartureTime)
                .Reduce(() => TimeSpan.FromMinutes(1))
                .TotalMinutes;
            var jd = history
                .FirstOrNone()
                .Map(d => d.Delay)
                .Reduce(() => TimeSpan.Zero)
                .TotalMinutes;
            
            // Gathering delay over latest 10 days
            var latest10JData = history
                .Take(10)
                .ToList();
            var gd = TimeSpan.FromMinutes(
                latest10JData
                    .IfAny<DemoHistory, double>(seq => seq.Average(i => i.Delay.TotalMinutes))
                    .Reduce()
            ).TotalMinutes;
            
            // Weather condition can affect the outcome to 40% 
            var wf = latest10JData
                .FirstOrNone()
                .Map(i => WeatherServiceFacade.IsGoodWeather(Enum.Parse<WeatherCode>(i.WeatherCode)))
                .Reduce(() => true)
                ? 1.0f
                : 1.40f;

            // A random factor can affect the final outcome
            var rr = Random.NextDouble() % 0.2f;
            
            // Apply formula:
            // ((jd / jt )^(1 / gd)) * wf + rr
            // Where
            // [journey delay] jd
            // [journey duration] jt
            // [gathering delay over last 10 days] gd
            // [weather condition factor] wf
            // [Random rate] rr
            return Math.Pow(jd / jt, 1.0f / gd) * wf + rr;
        }
    }
}