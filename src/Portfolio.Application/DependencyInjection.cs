using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Common.Behaviors;
using Portfolio.Application.Common.Mappings;

namespace Portfolio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(typeof(MappingConfig).Assembly);

        services.AddSingleton(mappingConfig);
        services.AddScoped<IMapper, Mapper>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddScoped(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}
