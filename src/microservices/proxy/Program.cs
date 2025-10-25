using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using proxy;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var moviesMigrationPercent = builder.Configuration.GetSection("MOVIES_MIGRATION_PERCENT").Get<int>();
var gradualMigration = builder.Configuration.GetSection("GRADUAL_MIGRATION").Get<bool?>() ?? true;

builder.Services.AddOcelot().AddCustomLoadBalancer<WeightedRoundRobinBalancer>(
            (provider, route, discoveryProvider) =>
                new WeightedRoundRobinBalancer(
                    discoveryProvider.GetAsync,
                    new int[] { 100-moviesMigrationPercent, moviesMigrationPercent },
                    gradualMigration
                )
        );


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () =>
 new { status = "success" });


app.UseWhen(context => !context.Request.Path.StartsWithSegments("/health"), appBuilder =>
{
    appBuilder.UseOcelot().Wait();
});

app.Run();
