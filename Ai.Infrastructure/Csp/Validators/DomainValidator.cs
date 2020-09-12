using Ai.Infrastructure.Csp.Model;
using FluentValidation;

namespace Ai.Infrastructure.Csp.Validators
{
    internal class DomainValidator<T> : AbstractValidator<Domain<T>>
        where T : CspValue
    {
        public DomainValidator()
        {
            RuleFor(d => d.Key).NotEmpty();
            RuleFor(d => d.Values).NotNull();
            RuleFor(d => d.Pruned).NotNull();
        }
    }
}