

using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Infrastructure.Persistence;
using BackOffice.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Api {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            string connectionString = builder.Configuration.GetConnectionString("BackOfficeDatabase") ?? throw new InvalidOperationException("A connection string 'BackOfficeDatabase' não foi configurada.");

            builder.Services.AddDbContext<BackOfficeDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IUserRepository, EfUserRepository>();

            builder.Services.AddScoped<IServiceRequestRepository, EfServiceRequestRepository>();

            builder.Services.AddScoped<CreateServiceRequestUseCase>();
            builder.Services.AddScoped<ListServiceRequestsUseCase>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope()) {
                BackOfficeDbContext dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                var developmentUser = BackOfficeDatabaseSeeder.SeedDevelopmentUser(dbContext);

                Console.WriteLine($"Usuário de desenvolvimento: {developmentUser.Name}");
                Console.WriteLine($"Id do Usuário de desenvolvimento: {developmentUser.Id}");
            }

            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
