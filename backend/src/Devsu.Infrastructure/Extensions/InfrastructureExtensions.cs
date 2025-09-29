
using Devsu.Application.Jobs;
using Devsu.Application.Services.Core.Pdf;
using Devsu.Application.Services.Transactions;
using Devsu.Infrastructure.Middlewares;

namespace Devsu.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        string xml)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfig(xml);

        services.AddPersistence(configuration)
            .AddConfigOptions(configuration)
            .AddCorsWithConfiguration(configuration)
            .AddValidators()
            .AddRepositories()
            .AddServices()
            .ConfigurePdf()
            .AddAutoMapper(typeof(UserMapper).Assembly);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHostedService<DailyLimitBackgroundService>();
        
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseExceptionHandler();
        return app;
    }

    private static IServiceCollection ConfigurePdf(this IServiceCollection services)
    {
        services.AddScoped<IPdfService, PdfService>();
        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            opt.EnableSensitiveDataLogging(false)
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Warning);
        });

        return services;
    }
    
    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    private static IServiceCollection AddCorsWithConfiguration(this
        IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = new CorsConfigOption();
        configuration.GetSection(nameof(CorsConfigOption)).Bind(corsOptions);

        var originsAllowed = corsOptions.OriginsAllowed ?? ["*"];
        var methodsAllowed = corsOptions.MethodsAllowed ?? ["*"];

        var corsPolicy = new CorsPolicyBuilder()
            .WithOrigins(string.Join(",", originsAllowed))
            .AllowAnyHeader()
            .WithMethods(string.Join(",", methodsAllowed))
            .Build();

        services.AddCors(c => c.AddDefaultPolicy(corsPolicy));

        return services;
    }
    
    private static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsConfigOption>(configuration.GetSection(nameof(CorsConfigOption)));
        services.Configure<JobOption>(configuration.GetSection(nameof(JobOption)));
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection service)
    {
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IAccountService, AccountService>();
        service.AddScoped<ITransactionService, TransactionService>();
        return service;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection service)
    {
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<IAccountRepository, AccountRepository>();
        service.AddScoped<ITransactionRepository, TransactionRepository>();
        return service;
    }


    private static void AddSwaggerConfig(this IServiceCollection services, string xml)
    {
        services.AddSwaggerGen(config =>
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
            config.IncludeXmlComments(xmlPath);

            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1.0",
                Title = "DEVSU APP API",
            });

            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}