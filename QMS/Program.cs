using QMS.DAL;
using QMS.Db;
using QMS.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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

var app = builder.Build();

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
