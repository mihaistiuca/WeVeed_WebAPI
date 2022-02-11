using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeVeedWebAPI.Utils;
using MongoDB.Driver;
using WeVeed.Domain.Services;
using WeVeed.Application.Services;
using Resources.Base.SettingsModels;
using Resources.Base.Utils;
using FluentValidation.AspNetCore;
using WeVeedWebAPI.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using WeVeed.Domain.Services.Mappings;
using WeVeed.Application.Services.Video;
using Microsoft.AspNetCore.Http;
using WeVeed.Application.Services.View;
using WeVeed.Application.Services.Comment;
using WeVeed.Domain.Services.Toko;
using WeVeed.Application.Services.Toko;

namespace WeVeedWebAPI
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
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // for JWT - token authentication 
            var secretForJwt = Configuration.GetSection("AppSettings:Secret").Value;
            var issuerJwt = Configuration.GetSection("AppSettings:Issuer").Value;
            var audienceForJwt = Configuration.GetSection("AppSettings:Audience").Value;
            var jwtKey = Encoding.ASCII.GetBytes(secretForJwt);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuerJwt,
                        ValidAudience = audienceForJwt,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                    };
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = 
                    new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation(fvc =>
                fvc.RegisterValidatorsFromAssemblyContaining<UserAppService>());

            Mapper.Initialize(cfg =>
            {
                AutoMapperRegister.RegisterMappings(cfg);
            });

            services.Configure<MongoConfiguration>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConfiguration = sp.GetService<IOptions<MongoConfiguration>>();
                var client = new MongoClient(mongoConfiguration.Value.ConnectionString);
                return client;
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongoConfiguration = sp.GetService<IOptions<MongoConfiguration>>();
                var mongoClient = sp.GetService<IMongoClient>();
                var database = mongoClient.GetDatabase(mongoConfiguration.Value.Database);
                return database;
            });

            services.Configure<EmailServerSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<AppGeneralSettings>(Configuration.GetSection("AppSettings"));

            // Application
            services.AddTransient<IUserAppService, UserAppService>();
            services.AddTransient<ISeriesAppService, SeriesAppService>();
            services.AddTransient<IVideoAppService, VideoAppService>();
            services.AddTransient<IChannelAppService, ChannelAppService>();
            services.AddTransient<IViewAppService, ViewAppService>();
            services.AddTransient<ICommentAppService, CommentAppService>();

            // Domain
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISeriesService, SeriesService>();
            services.AddTransient<IVideoService, VideoService>();
            services.AddTransient<IChannelService, ChannelService>();
            services.AddTransient<IViewService, ViewService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IFollowService, FollowService>();
            services.AddTransient<IViewsFilterService, ViewsFilterService>();

            // Utils
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // Toko
            services.AddTransient<ITokoRoomService, TokoRoomService>();
            services.AddTransient<ITokoRoomAppService, TokoRoomAppService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(HttpStatusCodeExceptionMiddleware));

            app.UseAuthentication();

            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
