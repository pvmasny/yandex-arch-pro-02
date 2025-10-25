using events.Infrastructure.Kafka;
using events.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaOption>(builder.Configuration.GetSection(KafkaOption.Position));
builder.Services.AddScoped<KafkaProducer>();
builder.Services.AddScoped<KafkaConsumer>();
builder.Services.AddHostedService<FakeConsumerWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
