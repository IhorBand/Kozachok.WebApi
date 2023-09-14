using AutoMapper;
using System.Reflection;
using Kozachok.WebApi.Infrastructure.MappingProfiles;

namespace Kozachok.WebApi.Setup
{
    public static class SetupHelper
    {
        public static IServiceCollection AddScopedByBaseType(this IServiceCollection services, Type baseType)
        {
            var assembly = Assembly.GetAssembly(baseType);
            if (assembly != null) 
            { 
                assembly.GetTypesOf(baseType)
                    .ForEach(type => 
                    {
                        var interf = type.GetInterface($"I{type.Name}");
                        if (interf != null)
                        {
                            services.AddScoped(interf, type);
                        }
                    });
            }
            return services;
        }

        public static IServiceCollection AddScopedHandlers(this IServiceCollection services, Type handlerType, Assembly assembly)
        {
            assembly
                .GetTypes()
                .ToList()
                .ForEach(type =>
                    type
                        .GetInterfaces()
                        .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == handlerType)
                        .ToList()
                        .ForEach(@interface => services.AddScoped(@interface, type))
                );

            return services;
        }

        public static List<Type> GetTypesOf(this Assembly assembly, Type baseType) => assembly
                .GetTypes()
                .Where(type =>
                    type.BaseType != null
                    && (
                        (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == baseType)) // -> Generics, ex: CrudRepository<>
                        || (baseType.IsAssignableFrom(type) && !type.IsAbstract) // -> Non generics, ex: Repository
                    )
                .ToList();

        public static void ConfigureDomainMappingProfiles(this IMapperConfigurationExpression configuration)
        {
            var profiles = typeof(DomainProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x));
            foreach (var profile in profiles)
            {
                configuration.AddProfile(Activator.CreateInstance(profile) as Profile);
            }
        }
    }
}
