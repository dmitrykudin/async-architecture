using System;
using System.Collections.Generic;
using AsyncArchitecture.Events.Interfaces;
using AsyncArchitecture.Events.Models;
using AsyncArchitecture.Events.Services;
using AsyncArchitecture.TaskTracker.AutoMapper;
using AsyncArchitecture.TaskTracker.Commands;
using AsyncArchitecture.TaskTracker.Database;
using AsyncArchitecture.TaskTracker.Interfaces.Commands;
using AsyncArchitecture.TaskTracker.Interfaces.Queries;
using AsyncArchitecture.TaskTracker.Interfaces.Repositories;
using AsyncArchitecture.TaskTracker.Processors;
using AsyncArchitecture.TaskTracker.Queries;
using AsyncArchitecture.TaskTracker.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace AsyncArchitecture.TaskTracker
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
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services
                .AddDbContext<TaskTrackerDbContext>(options =>
                {
                    options.UseSqlite(Configuration.GetConnectionString("TaskTrackerDb"));
                });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = Configuration["IdentityServerSettings:AuthorityUrl"];
                    options.ClientId = Configuration["IdentityServerSettings:ClientId"];
                    options.ClientSecret = Configuration["IdentityServerSettings:ClientSecret"];

                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    var scopes = Configuration.GetSection("IdentityServerSettings:Scopes").Get<string[]>();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.SaveTokens = true;
                });

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUserQueries, UserQueries>();
            services.AddScoped<IToDoItemQueries, ToDoItemQueries>();

            services.AddScoped<IToDoItemRepository, ToDoItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IToDoItemCommands, ToDoItemCommands>();

            services.AddSingleton<IEventSenderFactory, EventSenderFactory>();

            services.AddScoped<UserStreamProcessor>();
            services.AddSingleton(new EventProcessorConfiguration
            {
                TopicToEventProcessorMapping = new Dictionary<string, Type>
                {
                    { "user_stream", typeof(UserStreamProcessor) }
                }
            });
            services.AddScoped<IEventProcessorFactory, EventProcessorFactory>();
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
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}