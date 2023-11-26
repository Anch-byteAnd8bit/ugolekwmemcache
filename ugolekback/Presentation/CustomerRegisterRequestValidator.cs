using FluentValidation;

namespace Ugolek.Backend.Web.Presentation;

public class CustomerRegisterRequestValidator : AbstractValidator<CustomerRegisterRequest> {
    public CustomerRegisterRequestValidator() {
        RuleFor(x => x.Address)
            .NotEmpty().MaximumLength(256).EmailAddress();
    }
}