using CachingDemoApp.Data;
using Microsoft.Azure.Cosmos.Fluent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var cachingProvider = builder.Configuration.GetSection("DistributedCacheProvider").Value;

switch (cachingProvider)
{
    case "Redis":
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
            options.InstanceName = "CachingDemo_";
        });
        break;

    case "CosmosDb":
        {
            builder.Services.AddCosmosCache(options =>
            {
                options.DatabaseName = builder.Configuration.GetSection("CosmosDb:DatabaseName").Value;
                options.ContainerName = builder.Configuration.GetSection("CosmosDb:ContainerName").Value;
                options.ClientBuilder = new CosmosClientBuilder(builder.Configuration.GetSection("CosmosDb:ConnectionString").Value);
                options.CreateIfNotExists = true;
            });
        }
        break;

    default: throw new ArgumentException();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
