
using EduAdvisor.Application.Behaviors;
using EduAdvisor.Application.Validators.AuthModules;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
namespace EduAdvisor.Application
{
    public static class DepedencyInjection
    {
        public static IServiceCollection ApplicationDependencies(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblyContaining(typeof(DepedencyInjection));
            });

            services.AddValidatorsFromAssembly(typeof(RegisterStudentCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }


    }
}
