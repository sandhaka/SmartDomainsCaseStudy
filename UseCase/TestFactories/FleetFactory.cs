using System;
using System.Text;
using EventSourcing;
using EventSourcing.Infrastructure;
using UseCase.Domain;

namespace UseCase.TestFactories
{
    public class FleetFactory
    {
        public static TransportFleet CreateRandomFleet(int size)
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

                return lowerCase ? builder.ToString().ToLower() : builder.ToString();
            };

            var repository = new EventSourcedRepository<TransportTruckStatus>(new InMemoryStore());
            
            var fleet = new TransportFleet(repository);

            for (var i = 0; i < size; i++)
            {
                var truck = CreateRandomTruck(repository);
                truck.Capacity = 50;
                truck.FuelCapacity = 90;
                truck.ModelCode = randomString(6, false);
                
                fleet.Add(truck);
            }
            return fleet;
        }

        private static TransportTruck CreateRandomTruck(IEventSourcedRepository<TransportTruckStatus> statusRepository)
        {
            return TransportTruck.Create(statusRepository);
        }
    }
}