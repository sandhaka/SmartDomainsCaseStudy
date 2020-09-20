using Ai.Infrastructure.Csp.Csp.Model;
using FluentValidation;

namespace Ai.Infrastructure.Csp.Csp.Validators
{
    internal class VariableValidator<T> : AbstractValidator<Variable<T>>
        where T : CspValue
    {
        public VariableValidator()
        {
            RuleFor(v => v.Key).NotEmpty();
        }
    }
}