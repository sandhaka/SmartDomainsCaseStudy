using System;
using System.Collections.Generic;
using System.Text;
using UseCase.Domain;

namespace UseCase.TestFactories
{
    public class DemoFleetDataFactory
    {
        public (string DepartureLocation, string ArrivalLocation, TimeSpan TravelTime, TimeSpan Delay, bool Accident)
            ReadTravel(string id)
        {
            throw new NotImplementedException();
            // TODO Read from events archive
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
                    randomString(6, false),
                    50,
                    90
                );
                
                fleet.Add(truck);
            }
            return fleet;
        }
    }
}