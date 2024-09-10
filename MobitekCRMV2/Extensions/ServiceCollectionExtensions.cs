namespace MobitekCRMV2.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using MobitekCRMV2.Business.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ProjectsService>();
            services.AddScoped<NewsSitesService>();
            services.AddScoped<BacklinksService>();
            services.AddScoped<AdminService>();
            services.AddScoped<CreateTodos>();
            services.AddScoped<TodosService>();
            services.AddScoped<KeywordsService>();
            services.AddScoped<ContentBudgetService>();
            services.AddScoped<PasswordService>();

            return services;
        }
    }
}
