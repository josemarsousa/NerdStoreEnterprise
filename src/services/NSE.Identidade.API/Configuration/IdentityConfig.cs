using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSE.Identidade.API.Data;
using NSE.Identidade.API.Extensions;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Identidade.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfigutarion(this IServiceCollection services,IConfiguration configuration)
        {
            //Contexto
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            //End Contexto

            //Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();
            //End Identity

            //Config levada para Building Blocks em Services/Identidade junto com o AppSettings localizada em Extensions/AppSettings_old.cs
            ////JWT
            //var appSettingsSection = configuration.GetSection("AppSettings"); //para pegar o nó AppSettings no arquivo appsettings.json e atribuir para appSettingsSection
            //services.Configure<AppSettings>(appSettingsSection); //para a classe AppSettings em Extensions seja representada pela appSettingsSection definida acima

            //var appSettings = appSettingsSection.Get<AppSettings>(); //appSettings representa a classe AppSettings
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret); //atribui a chave em uma sequência de bytes para var key

            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = true; //Trabalhando somente com https
            //    x.SaveToken = true; //o token é guardado no http AuthenticationProperties após uma auth com sucesso
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true, //validar o emissor com base na assinatura
            //        IssuerSigningKey = new SymmetricSecurityKey(key), //chave de criptografia. Key definida acima
            //        ValidateIssuer = true, //validar o emissor para nao aceitar tokens de outros emissores com outras assinaturas 
            //        ValidateAudience = true, //validar a audiência para verificar onde o token é válido, para quais domínios é válido
            //        ValidAudience = appSettings.ValidoEm, //cria uma audiência válida. appSettings.ValidoEm definido acima
            //        ValidIssuer = appSettings.Emissor //cria um issuer válido. appSettings.Emissor difinido acima
            //    };
            //});
            ////End JWT

            //JWT
            services.AddJwtConfiguration(configuration);
            //End JWT

            return services;
        }

        //Config levada para Building Blocks em Services/Identidade
        //public static IApplicationBuilder UseIdentityConfiguration(this IApplicationBuilder app)
        //{
        //    app.UseAuthentication();
        //    app.UseAuthorization();

        //    return app;
        //}
    }
}
