using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Kozachok.WebApi.Hubs;
using Kozachok.WebApi.Infrastructure.MappingProfiles;
using Kozachok.Shared.DTO.Configuration;
using Kozachok.WebApi.Services.Abstractions;
using Kozachok.WebApi.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Kozachok.Repository.Contexts;
using Microsoft.AspNetCore.Authorization;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Domain.UoW;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.WebApi.Auth;
using Kozachok.Domain.Handlers.Commands;
using Kozachok.Domain.Handlers.Events;
using Kozachok.Repository.Repositories.Common;
using MediatR;
using Kozachok.WebApi.Setup;
using System.Reflection;
using MimeKit.Cryptography;

namespace Kozachok.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.Configuration = configuration;
            this.Env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddMediatR(options => options.AsScoped(), AppDomain.CurrentDomain.GetAssemblies());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            this.SetupDependencyInjection(services);

            services.AddCors(options =>
            {
                options.AddPolicy("OpenPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("ProdPolicy", builder =>
                {
                    builder.WithOrigins("https://kozachok.monster", "https://dev.kozachok.monster")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var connectionStrings =
                this.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfiguration>();
            var jwtSettings =
                this.Configuration.GetSection("Jwt").Get<JwtTokenConfiguration>();

            services
                .AddControllers()
                .AddNewtonsoftJson((op) =>
                {
                    op.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidIssuer = jwtSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            //TODO: Check headers to fix signalR Authorization
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (context.HttpContext.WebSockets.IsWebSocketRequest || context.Request.Headers["Accept"] == "text/event-stream"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kozachok.WebApi", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorizationheader using the Bearer scheme. \n\n
                        Enter 'Bearer[space]'and then your token in the text  input below.\n\n
                        Example:  'Bearer tes2543t432to243ke324n'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });

                c.AddSignalRSwaggerGen();
            });

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*
            app.UseRequestResponseLogging();
            */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kozachok v1"));
            }
            else
            {
                //app.UseHsts();
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            
            if (env.IsDevelopment())
            {
                app.UseCors("OpenPolicy");
            }
            else
            {
                app.UseCors("ProdPolicy");
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chathub");
                endpoints.MapHub<VideoHub>("/hubs/videohub");
            });
        }

        private void SetupDependencyInjection(IServiceCollection services)
        {
            var connectionStrings = this.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfiguration>();
            services.AddSingleton(connectionStrings);

            var jwtSettings = this.Configuration.GetSection("Jwt").Get<JwtTokenConfiguration>();
            services.AddSingleton(jwtSettings);

            var mailSettings = this.Configuration.GetSection("Mail").Get<MailConfiguration>();
            services.AddSingleton(mailSettings);

            var endpointSettings = this.Configuration.GetSection("Endpoints").Get<EndpointsConfiguration>();
            services.AddSingleton(endpointSettings);

            var storedEventsConfiguration = this.Configuration.GetSection("StoredEvents").Get<StoredEventsConfiguration>();
            services.AddSingleton(storedEventsConfiguration);

            var fileServerConfiguration = this.Configuration.GetSection("FileServer").Get<FileServerConfiguration>();
            services.AddSingleton(fileServerConfiguration);

            // AutoMapper Configuration
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DomainProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddDbContext<MainDbContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(connectionStrings.Main));

            services.AddDbContext<EventStoreSQLContext>(options =>
                options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(connectionStrings.Main));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddScoped<IUser, UserControl>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScopedByBaseType(typeof(CrudRepository<>)); // -> Repositories
            services.AddScopedHandlers(typeof(INotificationHandler<>), typeof(UserEventHandler).Assembly); // -> Events
            services.AddScopedHandlers(typeof(IRequestHandler<,>), typeof(UserCommandHandler).Assembly); // -> Commands

            services.AddScoped<ITokenService, TokenService>();
        }
    }
}