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

app.MapPost("/api/seed-faker", async (int? count, LegacyContext db) =>
{
    int seedCount = count.GetValueOrDefault(10);

    var orders = new Faker<Order>()
        .RuleFor(x => x.Protocol, d => d.Hashids.ToString())
        .RuleFor(x => x.Number, d => d.Commerce.Ean13())
        .Generate(seedCount);

    var persons = new Faker<Person>()
        .RuleFor(x => x.Name, d => d.Person.FirstName)
        .RuleFor(x => x.Email, d => d.Person.Email)
        .Generate(seedCount);

    await db.Orders.AddRangeAsync(orders);
    await db.Persons.AddRangeAsync(persons);
    await db.SaveChangesAsync();

    return Results.Ok(new { Orders = orders.Count, Persons = persons.Count });
})
.WithName("SeedFaker")
.Produces(StatusCodes.Status200OK);


app.Run();