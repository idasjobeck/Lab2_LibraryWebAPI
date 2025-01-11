
using System.Diagnostics;
using System.Text.Json.Serialization;
using Lab2_LibraryWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddDbContext<BooksDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BooksDb"))
                    .LogTo(message => Debug.WriteLine(message))
                    .EnableSensitiveDataLogging());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            /*
            using (var scope = app.Services.CreateScope())
               {
                   var db = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
                   db.Database.EnsureDeleted();
                   db.Database.EnsureCreated();
               }
            */

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
