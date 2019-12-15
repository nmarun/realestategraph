using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.API.Models;
using RealEstate.API.Mutations;
using RealEstate.API.Queries;
using RealEstate.API.Schema;
using RealEstate.DataAccess.Repositories;
using RealEstate.DataAccess.Repositories.Contracts;
using RealEstate.Database;
using RealEstate.Types.Payment;
using RealEstate.Types.Property;

namespace RealEstate.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddDbContext<RealEstateContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:RealEstateDb"]));
            services.AddScoped<IDocumentExecuter, DocumentExecuter>();
            services.AddScoped<PropertyQuery>();
            services.AddScoped<PropertyMutation>();
            services.AddScoped<PropertyType>();
            services.AddScoped<PropertyInputType>();
            services.AddScoped<PaymentType>();
            services.AddScoped<ISchema, RealEstateSchema>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RealEstateContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseGraphiQl();
            app.UseMvc();
            db.EnsureSeedData();
        }
    }
}
