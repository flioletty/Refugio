using System;
using System.Reflection;
using DBCore;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using VkAPI;
using Microsoft.OpenApi.Models;
using Clusterization;
using DBCore.Converters;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Refugio
{
    public class Startup
    {
        private readonly string _corsPolicy = "RefugioBmstu";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("RefugioDB");
            services.AddDbContext<VKContext>(options =>
                options.UseNpgsql(connectionString)
            );
            services.AddMvc();

            services.AddLogging();

            services.AddTransient<IUserRepository, PostgresSQLReposUser>();
            services.AddTransient<IGroupRepository, PostgresSQLReposGroup>();
            services.AddTransient<InitializationDb>();
            services.AddTransient<Clusterization_>();
            services.AddTransient<UserConverter>();
            services.AddTransient<SetInterests>();
            services.AddTransient<SetMetrics>();
            services.AddTransient<CombiningInterests>();
            services.AddSingleton<ModelSaver>();

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:38185",
                    ValidAudience = "https://localhost:38185",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey2410"))
                };
            });

            services.AddControllers();
            services.AddHealthChecks();
            services.AddCors(o => o.AddPolicy(_corsPolicy, builder =>
            {
                builder.AllowAnyOrigin();
            }));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Refugio", Version = "v1" });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });


            //services.AddLogging();

            //services.AddAuthorization(options =>
            //{
            //});

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            //}).
            //AddIdentityServerAuthentication(options =>
            //{
            //    options.ApiName = "Refugio";
            //    options.Authority = "http://localhost:38183/";
            //    options.RequireHttpsMetadata = false;
            //});

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //UpdateDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Refugio backend v1"));
            }

            app.UseCors(_corsPolicy);
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }

        //private static void UpdateDatabase(IApplicationBuilder app)
        //{
        //    using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        //    using var context = serviceScope.ServiceProvider.GetService<VKContext>();
        //    context.Database.Migrate();
        //}
    }
}

