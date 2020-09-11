using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UseCase.Domain;
using UseCase.Infrastructure;

namespace UseCase.TestFactories
{
    public static class DemoFleetDataFactory
    {
        private static List<List<DemoHistory>> _demoData;
        
        public static List<(
                string DepartureLocation, 
                string ArrivalLocation, 
                DateTime DepartureTime, 
                DateTime ArrivalTime, 
                string WeatherCode,
                double FatigueScore,
                TimeSpan Delay, 
                bool Accident)> 
            ReadNextJourneyHistory(int elemNumber)
        {
            if (_demoData == null)
            {
                var dataFile = new DirectoryInfo("../../../")
                    .GetFiles("*.json")
                    .OrderByDescending(p => p.Name)
                    .FirstOrDefault() ?? throw new NullReferenceException("Data file");

                var data = File.ReadAllText(dataFile.FullName);
                _demoData = JsonConvert.DeserializeObject<List<List<DemoHistory>>>(data);
            }

            return _demoData[elemNumber]
                .Select(d =>
                (
                    d.DepartureLocation, 
                    d.ArrivalLocation, 
                    d.DepartureTime, 
                    d.ArrivalTime, 
                    d.WeatherCode,
                    d.FatigueScore,
                    d.Delay, 
                    d.Accident
                ))
                .ToList();
        }

        public static List<TransportTruck> CreateRandomFleet(int size)
        {
            var random = new Random();
            Func<int, bool, string> randomString = (int length, bool lowerCase) =>
            {
                var builder = new StringBuilder(length);

                // Unicode/ASCII Letters are divided into two blocks
                // (Letters 65–90 / 97–122):   
                // The first group containing the uppercase letters and
                // the second group containing the lowercase.  

                // char is a single Unicode character  
                char offset = lowerCase ? 'a' : 'A';
                const int lettersOffset = 26; // A...Z or a..z: length = 26  

                for (var i = 0; i < length; i++)
                {
                    var @char = (char) random.Next(offset, offset + lettersOffset);
                    builder.Append(@char);
                }

                return lowerCase ? builder.ToString().ToUpper() : builder.ToString();
            };
            
            var fleet = new List<TransportTruck>();

            for (var i = 0; i < size; i++)
            {
                var truck = new TransportTruck(
                    Guid.NewGuid(), 
                    randomString(6, false),
                    50,
                    90
                );
                
                // Bind optional logger domain action
                truck.DomainLog = DemoLogger.DomainLog;
                
                fleet.Add(truck);
            }
            return fleet;
        }
    }
}