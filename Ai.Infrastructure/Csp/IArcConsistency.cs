namespace Ai.Infrastructure.Csp
{
    public interface IArcConsistency<T>
        where T : CspValue
    {
        bool Propagate(Csp<T> csp);
    }
}