using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ugolek.Backend.Web.Application.Features.Coals;
using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Application.Features.Orders;
using Ugolek.Backend.Web.Application.Services;
using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Presentation; 

public static class EndpointConfiguration {
    public static void MarUgolekApiEndpoints(this WebApplication app) {
        app.MapGet("/coals", CoalGetMany);
        
        app.MapPost("/customers/verification", CustomerVerify);

        app.MapPost("/customers", CustomerRegisterAsync);
        
        app.MapPost("/customers/orders", CustomerOrdersGetMany);
    }

    private static ICollection<Coal> CoalGetMany(IRepository<Coal> repo) {
        return repo.GetMany();
    }

    private static IResult CustomerVerify(
        [FromBody] CustomerVerifyRequest req, 
        ICustomerVerificationCodePersister verification,
        ICustomerToken customerToken,
        IRepository<Customer> customers
    ) {
        Customer? customer = customers.GetCustomerByEmail(req.Address);
        if (customer != null) {
            const bool codeIsValid = true;
            if (codeIsValid == verification.VerifyCustomerCode(customer.Id, req.Code)) {
                return Results.Ok(customerToken.GenerateToken(req.Address));
            }

            return Results.BadRequest();
        }

        return Results.BadRequest();
    }

    [Authorize]
    private static IResult CustomerOrdersGetMany(
        [FromBody] CustomerOrderReq req,
        OrderService orderService,
        ICustomerToken customerToken,
        IRepository<Customer> customers,
        HttpContext context
    ) {
        string? email = CustomerToken.GetCurrentEmail(context.User.Identity);
        Customer? customer = customers.GetCustomerByEmail(email);

        if (customer != null) {
            orderService.GetAddress(customer.Id, req.settlement, req.street, req.house);
            orderService.AddOrder(req.OrderItems, customer);
            return Results.Ok();
        }

        return Results.BadRequest();
    }

    private static async Task<IResult> CustomerRegisterAsync(
        [FromBody] CustomerRegisterRequest req,
        CustomerRegisterRequestValidator validator,
        CustomerService services,
        CancellationToken cancellationToken
    ) {
        if (validator.Validate(req) is { IsValid: false, Errors: var errors }) {
            return Results.BadRequest(errors);
        }

        await services.SendToCustomerVerificationCode(req.Address, cancellationToken);

        return Results.Ok();
    }
}