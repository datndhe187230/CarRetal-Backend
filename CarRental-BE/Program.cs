using CarRental_BE.Chatbot;
using CarRental_BE.Data;
using CarRental_BE.Helpers;
using CarRental_BE.Middleware;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.Entities;
using CarRental_BE.Models.Mapper;
using CarRental_BE.Repositories;
using CarRental_BE.Repositories.Impl;
using CarRental_BE.Services;
using CarRental_BE.Services.Impl;
using CarRental_BE.Services.Vnpay;
using CloudinaryDotNet;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Console.WriteLine(">>> builder ENV: " + builder.Environment.EnvironmentName);

// Add services to the container.
builder.Services.AddControllers();

// Load User Secrets (automatically included in Development)
//builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


//Add Repository and Services
builder.Services.AddScoped<IAccountRepository, AccountRepositoryImpl>();
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<IAuthService, AuthServiceImpl>();
builder.Services.AddScoped<ICarRepository, CarRepositoryImpl>();
builder.Services.AddScoped<ICarService, CarServiceImpl>();
builder.Services.AddScoped<IEmailService, EmailServiceImpl>();
builder.Services.AddScoped<IRedisService, RedisServiceImpl>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryServiceImpl>();
builder.Services.AddScoped<IBookingRepository, BookingRepositoryImpl>();
builder.Services.AddScoped<IBookingService, BookingServiceImpl>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepositoryImpl>();
builder.Services.AddScoped<IDashboardService, DashboardServiceImpl>();
builder.Services.AddScoped<IWalletRepository, WalletRepositoryImpl>();
builder.Services.AddScoped<IWalletService, WalletServiceImpl>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackServiceImpl>();
builder.Services.AddScoped<IChatbotService, ChatbotServiceImpl>();

//Configure Elasticsearch settings (local)
var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
    .CertificateFingerprint("d14784d90529b16a164b3e178eb770b02664afb1322a3225ca4145eef7d6c270")
    .Authentication(new BasicAuthentication("elastic", "MRil5d*w9Q__xNGV+OFQ"))
    .EnableDebugMode()
    .PrettyJson()
    .RequestTimeout(TimeSpan.FromMinutes(2));

var client = new ElasticsearchClient(settings);

//Configure Cloudinary settings
var cloudName = builder.Configuration.GetValue<string>("CloudinarySettings:CloudName");
var apiKey = builder.Configuration.GetValue<string>("CloudinarySettings:ApiKey");
var apiSecret = builder.Configuration.GetValue<string>("CloudinarySettings:ApiSecret");

if (new[] { cloudName, apiKey, apiSecret }.Any(string.IsNullOrWhiteSpace))
{
    throw new ArgumentException("Please specify Cloudinary account details!");
}

builder.Services.AddSingleton(new Cloudinary(new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret)));

// Add VNPAY service to the container.
builder.Services.AddSingleton<VNPAY.NET.IVnpay, VNPAY.NET.Vnpay>();

// Configure email settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Register DbContext using connection string from user secrets
builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DatabaseConnection"]));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});
//Configure Authentication With JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var requestPath = context.HttpContext.Request.Path.Value?.ToLower();
                var accessToken = context.Request.Cookies["Access_Token"];
                var forgotPasswordToken = context.Request.Cookies["Forgot_Password_Token"];

                // Route-based token selection
                if (!string.IsNullOrEmpty(requestPath) && requestPath.Contains("/reset-password"))
                {
                    if (!string.IsNullOrEmpty(forgotPasswordToken))
                    {
                        Console.WriteLine(forgotPasswordToken);
                        context.Token = forgotPasswordToken;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    code = 401,
                    message = "Unauthorized. Token is missing, invalid, or expired."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });
//Add VnPay
builder.Services.AddScoped<IVnPayService, VnPayService>();
//Register AutoMapper 
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
//booking


app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            code = 403,
            message = "Forbidden: You do not have permission to access this resource."
        });
        await context.Response.WriteAsync(result);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//booking



app.UseHttpsRedirection();

app.MapControllers();

app.Run();
