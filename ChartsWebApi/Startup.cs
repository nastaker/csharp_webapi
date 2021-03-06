﻿using System;
using System.Text;
using System.Threading.Tasks;
using ChartsWebApi.Extensions;
using ChartsWebApi.Models;
using ChartsWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ChartsWebApi
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
            string currConfig = Configuration["CurrConfig"];
            services.AddMvc()
                    .AddJsonOptions( options =>
                    {
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                        options.SerializerSettings.ContractResolver = new IgnoreEmptyEnumerablesResolver();
                    });
            services.AddMemoryCache();
            services.AddCors(options =>
                options.AddPolicy("CorsGuowenyan", 
                p => p.WithOrigins(Configuration[$"{currConfig}:Cors:Origins"])
                      .AllowCredentials()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("Authorization")));
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = 1_074_790_400;
                options.MultipartBodyLengthLimit = 1_074_790_400;
                options.MultipartHeadersLengthLimit = 1_074_790_400;
            });

            //将appsettings.json中的JwtSettings部分的配置读取到JwtSettings中，这是给其他地方用的
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            //将配置绑定到JwtSettings实例中
            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);
            //
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                //主要是jwt  token参数设置
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //Token颁发机构
                    ValidIssuer = jwtSettings.Issuer,
                    //颁发给谁
                    ValidAudience = jwtSettings.Audience,
                    //这里的key要进行加密，需要引用Microsoft.IdentityModel.Tokens
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuerSigningKey=true,
                    ////是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    ValidateLifetime = true,
                    ////允许的服务器时间偏移量
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // services.AddTransient<ITokenCreationService, TokenCreationService>();
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            // app.UseMiddleware(typeof(JwtTokenSlidingExpirationMiddleware));
            app.UseCors("CorsGuowenyan");
            app.UseMvc();
        }
    }
}
