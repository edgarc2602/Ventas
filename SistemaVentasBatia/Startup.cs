using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SistemaVentasBatia.Context;
using SistemaVentasBatia.Options;
using SistemaVentasBatia.Services;
using SistemaVentasBatia.Repositories;
using SistemaVentasBatia.Converters;
using SistemaVentasBatia.Middleware;
using OfficeOpenXml;

namespace SistemaVentasBatia
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
            services.AddHttpClient();
            // Repository Context
            services.AddSingleton<DapperContext>();

            // Configure Options
            services.Configure<ProductoOption>(Configuration.GetSection("ProdOpt"));

            // Mapper
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapper());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Rest API
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
            });

            // SPA
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddHttpContextAccessor();

            // Services
            services.AddScoped<ICotizacionesService, CotizacionesService>();
            services.AddScoped<IProspectosService, ProspectosService>();
            services.AddScoped<ICatalogosService, CatalogosService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ITabuladorService, TabuladorService>();
            services.AddScoped<ISalarioService, SalarioService>();
            services.AddScoped<ICargaMasivaService, CargaMasivaService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IClienteService, ClienteService>();

            // Repositories
            services.AddScoped<ICotizacionesRepository, CotizacionesRepository>();
            services.AddScoped<IProspectosRepository, ProspectosRepository>();
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITabuladorRepository, TabuladorRepository>();
            services.AddScoped<ISalarioRepository, SalarioRepository>();
            services.AddScoped<IServicioRepository, ServicioRepository>();
            services.AddScoped<ICargaMasivaRepository, CargaMasivaRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            //Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<CustomErrorHandler>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
