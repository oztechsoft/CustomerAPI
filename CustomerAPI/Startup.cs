using CustomerAPI.Data;
using CustomerAPI.Interfaces;
using CustomerAPI.Models;
using CustomerAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI
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

            services.AddControllers();

            // Register the DbContext
            services.AddDbContext<CustomerDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "CustomerDB"));

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CustomerDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                SeedDatabase(dbContext);
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // Helper method to add example data to the in-memory database
        private static void SeedDatabase(CustomerDbContext dbContext)
        {
            var customers = new List<Customer>
            {
        new Customer { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 5, 15) },
        new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1985, 8, 20) },
        new Customer { Id = 3, FirstName = "Michael", LastName = "Johnson", DateOfBirth = new DateTime(1982, 11, 10) }
            };

            dbContext.Customers.AddRange(customers);
            dbContext.SaveChanges();
        }
    }
}
