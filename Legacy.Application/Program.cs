using Bogus;
using Legacy.Application.Context;
using Legacy.Application.Domain;
using Microsoft.EntityFrameworkCore;
using Person = Legacy.Application.Domain.Person;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

builder.Services.AddDbContext<LegacyContext>(
    dbContextOptions => dbContextOptions
        .UseMySql("Server=localhost;Port=3306;Database=db_debezium;Uid=root;Pwd=root", serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (IServiceScope scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LegacyContext>();

    await context.Orders.AddRangeAsync(new Faker<Order>()
        .RuleFor(x => x.Protocol, d => d.Hashids.ToString())
        .RuleFor(x => x.Number, d => d.Commerce.Ean13())
        .Generate(10));

    await context.Persons.AddRangeAsync(new Faker<Person>()
    .RuleFor(x => x.Name, d => d.Person.FirstName)
    .RuleFor(x => x.Email, d => d.Person.Email)
    .Generate(10));
}


app.Run();