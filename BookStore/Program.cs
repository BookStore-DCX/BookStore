using BookStore.Data;
using BookStore.Mappings;
using BookStore.Middleware;
using BookStore.Repositories.Implementations;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Implementations;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace BookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add Controllers
            builder.Services.AddControllers();

            // Database Connection
            builder.Services.AddDbContext<BookContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));


            // Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Unit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IBookConditionRepository, BookConditionRepository>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();


            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Services
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IBookConditionService, BookConditionService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BookStore API",
                    Version = "v1"
                });
            });

     

            var app = builder.Build();

            // Middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
