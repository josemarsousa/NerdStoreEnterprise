using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace NSE.WebAPI.Core.Identidade
{
    public static class JwtConfig
    {
        public static void AddJwtConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            //JWT
            var appSettingsSection = configuration.GetSection("AppSettings"); //para pegar o nó AppSettings no arquivo appsettings.json e atribuir para appSettingsSection
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
        }

        public static void UseAuthConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}