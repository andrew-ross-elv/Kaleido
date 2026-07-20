using Kaleido;
using Microsoft.EntityFrameworkCore;
using ReferenceData;
using ReferenceData.ValueSets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddKaleido()
    .AddQueryableRecordsFromAssembly(typeof(ClientRecord).Assembly);

builder.Services.AddDbContext<KaleidoTestDbContext>(options =>
{
    options.UseSqlite("Data Source=data/kaleido-test-data.sqlite");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
        //options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class available for integration tests
public partial class Program { }