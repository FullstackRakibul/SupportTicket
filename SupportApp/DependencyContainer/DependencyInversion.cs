using SupportApp.Controllers;
using SupportApp.Repository;
using SupportApp.Repository.IReposiroty;
using SupportApp.Service;
using SupportApp.Service.Pagination;

namespace SupportApp.DependencyContainer
{
    public static class DependencyInversion
    {
        public static IServiceCollection RegistrationServices(this IServiceCollection services)
        {

            //services ......
            services.AddTransient<TicketTypeService, TicketTypeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<EmailBoxService, EmailBoxService>();
            services.AddTransient<TicketService, TicketService>();
            services.AddTransient<PaginationService, PaginationService>();
            services.AddScoped<TargetService, TargetService>();
            services.AddTransient<AuthController>();



            // repositories ......
            services.AddTransient<ICodeSnippetInterface , CodeSnippetRepository>();
            services.AddTransient<ITicketInterface, TicketRepository>();
            services.AddTransient<ITicketTypeInterface, TicketTypeRepository>();
            services.AddTransient<IGlobalFileUploadInterface, GlobalFileUploadRepository>();
            services.AddTransient<ITaskItemInterface, TaskItemRepository>();


            return services;
        }
    }
}
