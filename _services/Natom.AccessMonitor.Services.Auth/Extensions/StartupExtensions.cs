using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Natom.AccessMonitor.Services.Auth.Entities;
using Natom.AccessMonitor.Services.Auth.PackageConfig;
using Natom.AccessMonitor.Services.Auth.Profile;
using Natom.AccessMonitor.Services.Auth.Services;
using Natom.AccessMonitor.Services.Cache.Services;
using Natom.AccessMonitor.Services.Configuration.Services;
using System;

namespace Natom.AccessMonitor.Extensions
{
    public static class StartupExtensions
    {
        private static bool _hostedServiceAdded = false;

        public static IServiceCollection AddAuthService(this IServiceCollection service, string scope)
        {
            service.AddSingleton<AuthService>();
            service.AddSingleton(new AuthServiceConfig { Scope = scope });
            service.AddSingleton(new Mapper(MappingProfile.Build()));

            service.AddScoped<AccessToken>();

            return service;
        }
    }
}