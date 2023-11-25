using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ugolekback.Coals.Model;
using ugolekback.Core;
using ugolekback.CustomerF;
using ugolekback.OrderF;

namespace Ugolek.Backend.Web.Services
{
    public static class EndpointConfiguration
    {
        public static void MapGetCoal(this WebApplication app)
        {
            app.MapGet("/coals", (IRepository<Coal> repo) =>
            {
                return repo.GetMany();
            });
        }

        public static void MapPostCustomer(this WebApplication app)
        {
            app.MapPost("/customers", async (
                [FromBody] CustomerRegisterRequest req,
                CustomerRegisterRequestValidator validator,
                CustomerService services,
                CancellationToken cancellationToken
                ) =>
            {
                if (validator.Validate(req) is { IsValid: false, Errors: var errors })
                {
                    return Results.BadRequest(errors);
                }
                await services.SendToCustomerVerificationCode(req.Address, cancellationToken);
                return Results.Ok();
            });
        }

        public static void MapPostCustomerVerification(this WebApplication app)
        {
            app.MapPost("/customers/verification", (
            [FromBody] CustomerVerifyRequest req,
            ICustomerVerificationCodePersister verification,
            ICustomerToken customerToken,
            IRepository<Customer> customers
            ) =>
            {
                Customer? customer = customers.GetCustomerByEmail(req.Address);
                if (customer != null)
                {
                    const bool codeIsValid = true;
                    if (codeIsValid == verification.VerifyCustomerCode(customer.Id, req.Code))
                    {
                        return Results.Ok(customerToken.GenerateToken(req.Address));
                    }
                    return Results.BadRequest();
                }
                return Results.BadRequest();
            });
        }

        public static void MapPostOrder(this WebApplication app)
        {
            app.MapPost("/customers/orders", [Authorize] (
            [FromBody] CustomerOrderReq req,
            OrderService orderService,
            ICustomerToken customerToken,
            IRepository<Customer> customers,
            HttpContext context
            ) =>
            {
                string? email = CustomerToken.GetCurrentEmail(context.User.Identity);
                Customer? customer = customers.GetCustomerByEmail(email);

                if (customer != null)
                {
                    orderService.GetAddress(customer.Id, req.settlement, req.street, req.house);
                    orderService.AddOrder(req.OrderItems, customer);
                    return Results.Ok();

                }
                return Results.BadRequest();
            });
        }
    }
}
