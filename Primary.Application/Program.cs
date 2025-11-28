using MassTransit;
using Microsoft.EntityFrameworkCore;
using Primary.Application.Consumers;
using Primary.Application.Context;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

builder.Services.AddDbContext<PrimaryContext>(
    dbContextOptions => dbContextOptions
        .UseMySql("Server=localhost;Port=3306;Database=db_debezium;Uid=root;Pwd=root", serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.AddConsumer<PersonCreateConsumer>();

    cfg.UsingRabbitMq((context, transport) =>
    {
        transport.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        transport.ReceiveEndpoint("person-create-app", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.DefaultContentType = new ContentType("application/json");
            e.UseRawJsonSerializer();
            e.Consumer<PersonCreateConsumer>(context);
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
