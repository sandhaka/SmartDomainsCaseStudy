using System;
using EventSourcing;

namespace UseCase.Domain
{
    public class TransportTruck
    {
        private readonly IEventSourcedRepository<TransportTruckStatus> _statusEsRepository;

        public static TransportTruck Create(IEventSourcedRepository<TransportTruckStatus> statusRepository)
        {
            return new TransportTruck(statusRepository);
        }
        
        protected TransportTruck(
            IEventSourcedRepository<TransportTruckStatus> statusEsRepository,
            Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            _statusEsRepository = statusEsRepository;
        }
        
        public Guid Id { get; }
        public string ModelCode { get; set; }
        public int Capacity { get; set; }
        public int FuelCapacity { get; set; }

        public TransportTruckStatus CurrentStatus()
        { 
            return _statusEsRepository.FindById(Id);
        }

        public TransportTruckStatus StatusAt(DateTime at)
        {
            return _statusEsRepository.FindById(Id, at);
        }
    }
}