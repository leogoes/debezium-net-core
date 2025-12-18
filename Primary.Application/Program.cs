using MassTransit;
using Microsoft.EntityFrameworkCore;
using Primary.Application.Consumers;
using Primary.Application.Context;
using Primary.Application.Contracts;

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
    cfg.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
    
    cfg.AddRider(rider =>
    {
        rider.AddConsumer<PersonCreateConsumer>();

        rider.UsingKafka((context, k) =>
        {
            k.Host("127.0.0.1:29092");

            k.TopicEndpoint<PersonCreate>("app.db_debezium.Persons", "person-create-app-group", e =>
            {
                e.ConfigureConsumer<PersonCreateConsumer>(context);
                e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
            });
        });
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
