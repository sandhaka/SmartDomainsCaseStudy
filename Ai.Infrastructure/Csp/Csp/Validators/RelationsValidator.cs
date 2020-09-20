using Ai.Infrastructure.Csp.Csp.Model;
using FluentValidation;

namespace Ai.Infrastructure.Csp.Csp.Validators
{
    internal class RelationsValidator<T> : AbstractValidator<Relations<T>>
        where T : CspValue
    {
        public RelationsValidator()
        {
            RuleFor(r => r.Key).NotEmpty();
            RuleFor(r => r.Values)
                .ForEach(v =>
                    v.SetValidator(new VariableValidator<T>()));
        }
    }
}