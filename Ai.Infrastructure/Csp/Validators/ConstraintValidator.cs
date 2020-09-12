using Ai.Infrastructure.Csp.Model;
using FluentValidation;

namespace Ai.Infrastructure.Csp.Validators
{
    internal class ConstraintValidator<T> : AbstractValidator<Constraint<T>>
        where T : CspValue
    {
        public ConstraintValidator()
        {
            RuleFor(c => c.Rule).NotNull();
        }
    }
}