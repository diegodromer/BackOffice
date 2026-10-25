

using BackOffice.Application.Interfaces;
using BackOffice.Application.UseCases;
using BackOffice.Infrastructure.Persistence;
using BackOffice.Infrastructure.Repositories;
using BackOffice.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace BackOffice.Api {
    public class Program {

        protected Program() { }

        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddOptions<JwtOptions>()
                .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
                .Validate(
                    options => !string.IsNullOrWhiteSpace(options.Issuer)
                        && !string.IsNullOrWhiteSpace(options.Audience),
                    "O emissor e o destinatário do JWT são obrigatórios."
                )
                .Validate(
                    options => options.ExpirationMinutes > 0,
                    "A duração do JWT deve ser maior que zero."
                )
                .Validate(
                    options => {
                        if (string.IsNullOrWhiteSpace(options.SigningKey)) {
                            return false;
                        }

                        try {
                            return Convert.FromBase64String(
                                options.SigningKey
                            ).Length >= 32;
                        } catch (FormatException) {
                            return false;
                        }
                    },
                    "A chave de assinatura do JWT deve estar em Base64 e conter pelo menos 32 bytes."                    
                )
                .ValidateOnStart();

            string connectionString = builder.Configuration.GetConnectionString("BackOfficeDatabase") ?? throw new InvalidOperationException("A connection string 'BackOfficeDatabase' não foi configurada.");

            builder.Services.AddDbContext<BackOfficeDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IUserRepository, EfUserRepository>();

            builder.Services.AddScoped<IServiceRequestRepository, EfServiceRequestRepository>();

            builder.Services.AddScoped<CreateServiceRequestUseCase>();
            builder.Services.AddScoped<ListServiceRequestsUseCase>();
            builder.Services.AddScoped<GetServiceRequestByIdUseCase>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<LoginUseCase>();
            builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            builder.Services
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)

                .Configure<IOptions<JwtOptions>>(
                    (bearerOptions, jwtOptionsAccessor) => {
                        JwtOptions jwtOptions = jwtOptionsAccessor.Value;

                        bearerOptions.TokenValidationParameters =
                            new TokenValidationParameters {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtOptions.Issuer,
                                ValidAudience = jwtOptions.Audience,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Convert.FromBase64String(jwtOptions.SigningKey)
                                ),
                                ValidAlgorithms = new[] {
                                    SecurityAlgorithms.HmacSha256
                                },
                                ClockSkew = TimeSpan.Zero
                            };
                    }
                );

            builder.Services.AddAuthorization();
            
            var app = builder.Build();

            using (var scope = app.Services.CreateScope()) {
                BackOfficeDbContext dbContext = scope.ServiceProvider.GetRequiredService<BackOfficeDbContext>();

                IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var developmentUser = BackOfficeDatabaseSeeder.SeedDevelopmentUser(dbContext, passwordHasher);

                Console.WriteLine($"Usuário de desenvolvimento: {developmentUser.Name}");
                Console.WriteLine($"Id do Usuário de desenvolvimento: {developmentUser.Id}");
            }

            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
