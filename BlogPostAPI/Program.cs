using BlogPostAPI.Handlers;
using BlogPostAPI.Repositories;
using Microsoft.AspNetCore.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder
            .WithOrigins(
                "http://localhost:4200"  // Add HTTPS variant
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)  // Important for broader origin support
            .AllowCredentials();
    });
}); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAngularApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
