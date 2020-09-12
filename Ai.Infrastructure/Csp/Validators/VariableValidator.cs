using Ai.Infrastructure.Csp.Model;
using FluentValidation;

namespace Ai.Infrastructure.Csp.Validators
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