using System.Collections.Generic;
using EventSourcing;

namespace UseCase.Domain
{
    public class TransportFleet
    {
        private readonly IEventSourcedRepository<TransportTruckStatus> _TruckStatusEsRepository;

        public List<TransportTruck> TransportTrucks { get; set; }
        
        public TransportFleet(IEventSourcedRepository<TransportTruckStatus> truckStatusEsRepository)
        {
            _TruckStatusEsRepository = truckStatusEsRepository;
            TransportTrucks = new List<TransportTruck>();
        }

        public void Add(TransportTruck transportTruck)
        {
            TransportTrucks.Add(transportTruck);
        }
    }
}