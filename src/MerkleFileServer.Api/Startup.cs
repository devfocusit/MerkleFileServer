using MerkleFileServer.Addon.AsClient;
using MerkleFileServer.Application;
using MerkleFileServer.Domain.Abstractions;
using MerkleFileServer.Infrastructure;

namespace MerkleFileServer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddMemoryCache();
            services.ConfigureForMemoryStorage(options => Configuration.GetSection("MemoryStorage").Bind(options));
            services.AddInfrastructure();
            services.AddApplication(Configuration);

            services.AddClientCapabilities(options => Configuration.GetSection("MerkleFileClient").Bind(options));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            PreloadFileIfRequested(app.ApplicationServices);
        }

        private void PreloadFileIfRequested(IServiceProvider applicationServices)
        {
            string file = Configuration["file"];

            if (!string.IsNullOrEmpty(file))
            {
                Task.Run(() =>
                {
                    applicationServices.GetRequiredService<ILoadFileHashService>().Load(file);
                });
            }

        }
    }
}
