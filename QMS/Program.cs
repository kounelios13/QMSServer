using Microsoft.OpenApi.Models;
using QMS.DAL;
using QMS.Db;
using QMS.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc , _ , _) =>
    {
        doc.Servers = [];
        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IFrontDeskRepository, FrontDeskRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddDbContext<QmsDbContext>();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
});
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        // #FIX make it configurable through appsettings
        policy.SetIsOriginAllowed( origin =>
        {
            return origin.StartsWith("https://localhost:7182");
        });
        policy.AllowCredentials();
        policy.Build();
    });
});


var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();
app.MapHub<QueueHub>("/hubs/queue");

app.Run();
