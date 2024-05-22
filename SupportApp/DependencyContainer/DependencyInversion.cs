﻿using SupportApp.Repository;
using SupportApp.Repository.IReposiroty;
using SupportApp.Service;

namespace SupportApp.DependencyContainer
{
    public static class DependencyInversion
    {
        public static IServiceCollection RegistrationServices(this IServiceCollection services)
        {

            //services ......
            services.AddTransient<TicketTypeService, TicketTypeService>();
            


            // repositories ......
            services.AddTransient<ICodeSnippetInterface , CodeSnippetRepository>();
            services.AddTransient<ITicketInterface, TicketRepository>();
            services.AddTransient<ITicketTypeInterface, TicketTypeRepository>();
            services.AddTransient<IGlobalFileUploadInterface, GlobalFileUploadRepository>();

            return services;
        }
    }
}
