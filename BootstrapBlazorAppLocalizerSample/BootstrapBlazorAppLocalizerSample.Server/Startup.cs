using BootstrapBlazor.Components;
using BootstrapBlazorAppLocalizerSample.Shared.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BootstrapBlazorAppLocalizerSample.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<WeatherForecastService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // 增加多语言支持配置信息
            services.AddRequestLocalization<IOptions<BootstrapBlazorOptions>>((localizerOption, blazorOption) =>
            {
                var supportedCultures = blazorOption.Value.GetSupportedCultures();

                localizerOption.SupportedCultures = supportedCultures;
                localizerOption.SupportedUICultures = supportedCultures;
            });

            services.AddBootstrapBlazor(setupAction: options =>
            {
                // 设置自己的 RESX 多语言文化资源文件 如 Program.{CultureName}.resx
                options.ResourceManagerStringLocalizerType = typeof(Program);
            });
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
                app.UseExceptionHandler("/Error");
            }

            // 启用本地化
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()!.Value);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
