using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspCoreAPI
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
            services.AddTransient<UserDAO>();

            var signinConfigurations = new SigningConfigurations();
            services.AddSingleton(signinConfigurations);

            var tokenConfigurations = new TokenConfiguration();
            new ConfigureFromConfigurationOptions<TokenConfiguration>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);

            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(authOpt =>
            {
                authOpt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOpt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOpt =>
            {
                var paramsValidation = bearerOpt.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signinConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                //paramsValidation as assinatura do tokenConfigurations recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                //verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerância para a expiração de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });


            // Ativa o uso do token como forma de autorizar o acesso a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer",new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
