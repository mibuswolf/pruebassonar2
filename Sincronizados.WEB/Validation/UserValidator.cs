using FluentValidation;
using Sincronizados.Shared.Models;
using Sincronizados.WEB.Resources;

namespace Sincronizados.WEB.Validation
{
    public class UserValidator : AbstractValidator<Users>
    {
        public UserValidator()
        {
            RuleFor(customer => customer.VatNum).NotEmpty()
                .WithMessage(Resource.IdentificationCardValid)
                .When(customer => customer.IsEmployee == true);
        }
    }
}
