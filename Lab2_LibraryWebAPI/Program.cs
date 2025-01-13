
using System.Diagnostics;
using System.Text.Json.Serialization;
using Lab2_LibraryWebAPI.Data;
using Microsoft.Data.SqlClient;
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

            var connectionString = builder.Configuration.GetConnectionString("BooksDb");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Azure")
            {
                //UserSecretsId from project settings
                builder.Configuration.AddUserSecrets("6b34173c-9d9e-4152-993e-77a0f01ef3f3");
                //set password when running in Azure
                var password = builder.Configuration["DbPassword"];

                var sqlconnectionBuilder = new SqlConnectionStringBuilder(connectionString);
                sqlconnectionBuilder.Password = password;
                connectionString = sqlconnectionBuilder.ConnectionString;
            }

            builder.Services.AddDbContext<BooksDbContext>(options =>
                options.UseSqlServer(connectionString)
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
