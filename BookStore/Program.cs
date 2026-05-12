using System.Text;
using AutoMapper;
using BookStore.Data;
using BookStore.Mappings;
using BookStore.Middleware;
using BookStore.Repositories.Implementations;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
	var keyDirectory = Path.Combine(
		builder.Environment.ContentRootPath,
		"keys");

	Directory.CreateDirectory(keyDirectory);

	builder.Services
		.AddDataProtection()
		.PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
		.SetApplicationName("BookStoreWebAPI");
}

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File(
		"logs/bookstore-.txt",
		rollingInterval: RollingInterval.Day)
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<BookContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();

builder.Services.AddScoped(
	typeof(IGenericRepository<>),
	typeof(GenericRepository<>));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme =
			JwtBearerDefaults.AuthenticationScheme;

		options.DefaultChallengeScheme =
			JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters =
			new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,

				ValidIssuer = jwtSettings["Issuer"],

				ValidAudience = jwtSettings["Audience"],

				IssuerSigningKey =
					new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(
							jwtSettings["Secret"]!))
			};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(
		"AdminOnly",
		policy => policy.RequireRole("Admin"));

	options.AddPolicy(
		"StoreOwner",
		policy => policy.RequireRole(
			"Admin",
			"StoreOwner"));

	options.AddPolicy(
		"RegisteredUser",
		policy => policy.RequireRole(
			"Admin",
			"StoreOwner",
			"RegisteredUser"));
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc(
		"v1",
		new OpenApiInfo
		{
			Title = "BookStore API",
			Version = "v1"
		});

	c.AddSecurityDefinition(
		"Bearer",
		new OpenApiSecurityScheme
		{
			In = ParameterLocation.Header,
			Description = "Enter: Bearer {token}",
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey
		});

	c.AddSecurityRequirement(
		new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference =
						new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
				},
				Array.Empty<string>()
			}
		});
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();