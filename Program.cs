using Microsoft.EntityFrameworkCore;
using ECommerceProductsAPI.Data;
using ECommerceProductsAPI.Services.Products;
using ECommerceProductsAPI.Services.Users;
using ECommerceProductsAPI.Services.Caching;
using ECommerceProductsAPI.Services.Users.Addresses;
using ECommerceProductsAPI.Services.Users.Password;
using ECommerceProductsAPI.Services.Users.Register;
using ECommerceProductsAPI.Services.Users.Subscriptions;
using ECommerceProductsAPI.Services.Products.VariableProducts;
using ECommerceProductsAPI.Services.Products.GroupedProducts;
using ECommerceProductsAPI.Utils.CustomRegex;
using System.Text.Json.Serialization;
using System.Text.Json;
using ECommerceProductsAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductsDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
// builder.Services.AddControllers(); 
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductVariationService, ProductVariationService>();
builder.Services.AddScoped<IGroupedProductService, GroupedProductService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IStrongPasswordRegex, StrongPasswordRegex>();

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();
// app.Services.GetRequiredService<IServiceProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce Products API");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
// app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();