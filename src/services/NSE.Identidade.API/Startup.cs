using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSE.Identidade.API
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
            //Contexto
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //End Contexto

            //Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();
            //End Identity

            //JWT
            var appSettingsSection = Configuration.GetSection("AppSettings"); //para pegar o nó AppSettings no arquivo appsettings.json e atribuir para appSettingsSection
            services.Configure<AppSettings>(appSettingsSection); //para a classe AppSettings em Extensions seja representada pela appSettingsSection definida acima

            var appSettings = appSettingsSection.Get<AppSettings>(); //appSettings representa a classe AppSettings
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); //atribui a chave em uma sequência de bytes para var key

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; //Trabalhando somente com https
                x.SaveToken = true; //o token é guardado no http AuthenticationProperties após uma auth com sucesso
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, //validar o emissor com base na assinatura
                    IssuerSigningKey = new SymmetricSecurityKey(key), //chave de criptografia. Key definida acima
                    ValidateIssuer = true, //validar o emissor para nao aceitar tokens de outros emissores com outras assinaturas 
                    ValidateAudience = true, //validar a audiência para verificar onde o token é válido, para quais domínios é válido
                    ValidAudience = appSettings.ValidoEm, //cria uma audiência válida. appSettings.ValidoEm definido acima
                    ValidIssuer = appSettings.Emissor //cria um issuer válido. appSettings.Emissor difinido acima
                };
            });
            //End JWT

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "NerdStore Enterprise Identity API",
                    Description = "Esta API faz parte do curso ASP.NET Core Enterprise Applications.",
                    Contact = new OpenApiContact() { Name = "Josemar Sousa", Email = "josemaar.sousa@gmail.com"},
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")},
                    Version = "v1" 
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NSE.Identidade.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
