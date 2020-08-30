using System;
using System.Threading.Tasks;

namespace EventSourcing
{
    public interface IEventSourcedRepository<TEventSourced>
        where TEventSourced : EventSourcedAggregate
    {
        TEventSourced FindById(Guid id);
        TEventSourced FindById(Guid id, DateTime at);
        void Add(TEventSourced aggregate);
        void Save(TEventSourced aggregate);
        void StartTransaction();
        Task EndActiveTransactionAndCommitAsync();
        Task AbortActiveTransactionAsync();
        Task DoMultiTransactionalWork(TEventSourced[] aggregates, Action<TEventSourced[]> action);
    }
}