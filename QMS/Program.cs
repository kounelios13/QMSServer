using QMS.DAL;
using QMS.Db;
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

app.Run();
