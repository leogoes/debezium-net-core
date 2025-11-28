using MassTransit;
using Microsoft.EntityFrameworkCore;
using Secondary.Application.Consumers;
using Secondary.Application.Context;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

builder.Services.AddDbContext<SecondaryContext>(
    dbContextOptions => dbContextOptions
        .UseMySql("Server=localhost;Port=3306;Database=db_debezium;Uid=root;Pwd=root", serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.AddConsumer<OrderCreateConsumer>();

    cfg.UsingRabbitMq((context, transport) =>
    {
        transport.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        transport.ReceiveEndpoint("order-create", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.DefaultContentType = new ContentType("application/json");
            e.UseRawJsonSerializer();
            e.Consumer<OrderCreateConsumer>(context);
        });

        transport.UseRawJsonSerializer();
        transport.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
