using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Ugolek.Backend.Web.Services;
using ugolekback;
using ugolekback.Coals.Model;
using ugolekback.Core;
using ugolekback.CustomerF;
using ugolekback.EmailF;
using ugolekback.OrderF;
using ugolekback.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<EmailServiceOptions>()
    .BindConfiguration("EmailService");

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ugolek API", Description = "", Version = "v1" });
});

{ // Register Other Services
    builder.Services.AddScoped<IEmailSender, EmailSender>();

    builder.Services.AddMemoryCache();
}

{ // Register Feature Services
    builder.Services.AddScoped<CustomerService>();

    builder.Services.AddScoped<ICustomerVerificationCodePersister, CustomerVerificationCodePersister>();
}

{ // Register Persistence Services
    builder.Services.AddSingleton<IRepository<Coal>>(new BaseInMemRepository<Coal>(new List<Coal> {
        new() { Id = 1, Name = "ДМСШ 0-25", Price = 150000, },
        new() { Id = 2, Name = "ДО 25-50", Price = 210000, },
        new() { Id = 3, Name = "TДПК 50-200", Price = 230000 }
    }));

    builder.Services.AddSingleton<IRepository<Customer>>(new BaseInMemRepository<Customer>(new List<Customer> {
        new() { Id = 1, Email = "sample1@foo.bar", City = "Абакан", Street = "Кирова", House = "120" },
        new() { Id = 2, Email = "sample2@foo.bar", City = "Абаза", Street = "Кирова", House = "112" },
        new() { Id = 3, Email = "sample3@foo.bar", City = "Сорск", Street = "Кирова", House = "1" }
    }));

    builder.Services.AddSingleton<IRepository<Order>, BaseInMemRepository<Order>>();
}

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoalStore API V1"); });

{ // Endpoints mapping
    app.MapGet("/coals", (IRepository<Coal> repo) =>
    {
        return repo.GetMany();
    });

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

    app.MapPost("/customers/verification", (
        [FromBody] CustomerVerifyRequest req,
        CustomerVerificationCodePersister verification,
        IRepository<Customer> customers
    ) =>
    {
        Customer? customer = customers.GetCustomerByEmail(req.Address);
        if (customer != null)
        {
            if (verification.VerifyCustomerCode(customer.Id, req.Code))
            {
                return Results.Ok();
            }
            return Results.BadRequest();
        }
        return Results.BadRequest();
    });
}

app.Run();

public class CustomerRegisterRequest {
    public string Address { get; init; }
}

public class CustomerRegisterRequestValidator : AbstractValidator<CustomerRegisterRequest> {
    public CustomerRegisterRequestValidator() {
        RuleFor(x => x.Address)
            .NotEmpty().MaximumLength(256).EmailAddress();
    }
}

public class CustomerVerifyRequest {
    public string Address { get; init; }

    public string Code { get; init; }
}