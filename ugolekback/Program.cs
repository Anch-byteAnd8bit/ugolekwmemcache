using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Ugolek.Backend.Web.Application.Features.Coals;
using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Application.Features.Orders;
using Ugolek.Backend.Web.Application.Services;
using Ugolek.Backend.Web.Application.Services.CustomerTokens;
using Ugolek.Backend.Web.Configuration.Customers;
using Ugolek.Backend.Web.Configuration.Services;
using Ugolek.Backend.Web.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.Services.AddOptions<EmailServiceOptions>()
    .BindConfiguration("EmailService");

builder.Services.AddOptions<TokenServiceOptions>()
    .BindConfiguration("Jwt");

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ugolek API", Description = "", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme." +
                      " \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                      "\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services
    .AddAuthentication(option => {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options => {
        var tokenOptions = builder.Configuration.GetSection("Jwt").Get<TokenServiceOptions>();

        if (tokenOptions is not { }) {
            throw new ArgumentException("Configuration of JWT token is invalid");
        }

        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateLifetime = false,

            ValidateAudience = true,
            ValidAudience = tokenOptions.Audience,

            ValidateIssuer = true,
            ValidIssuer = tokenOptions.Issuer,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = tokenOptions.SecurityKey
        };
    });

{ // Register Other Services
    builder.Services.AddScoped<IEmailSender, EmailSender>();
    builder.Services.AddScoped<ICustomerVerificationCodePersister, CustomerVerificationCodePersister>();
    builder.Services.AddScoped<ICustomerToken, CustomerToken>();
    builder.Services.AddMemoryCache();
}

{ // Register Feature Services
    builder.Services.AddScoped<CustomerService>();
    builder.Services.AddScoped<OrderService>();
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
    builder.Services.AddSingleton<IRepository<OrderItem>>(new BaseInMemRepository<OrderItem>(new List<OrderItem> {}));
    builder.Services.AddSingleton<IRepository<Order>>(new BaseInMemRepository<Order>(new List<Order> {}));
}

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoalStore API V1"); });

app.MarUgolekApiEndpoints();

app.Run();