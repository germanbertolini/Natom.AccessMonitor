using AutoMapper;
using Natom.AccessMonitor.Services.Auth.Entities;
using Natom.AccessMonitor.Services.Auth.Entities.Models;
using Natom.AccessMonitor.Services.Auth.Exceptions;
using Natom.AccessMonitor.Services.Auth.Helpers;
using Natom.AccessMonitor.Services.Auth.PackageConfig;
using Natom.AccessMonitor.Services.Auth.Repository;
using Natom.AccessMonitor.Services.Cache.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Auth.Services
{
    public class AuthService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CacheService _cacheService;
        private readonly AuthServiceConfig _config;
        private readonly Mapper _mapper;

        public AuthService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
            _mapper = (Mapper)serviceProvider.GetService(typeof(Mapper));
            _config = (AuthServiceConfig)serviceProvider.GetService(typeof(AuthServiceConfig));
        }

        public async Task<AccessToken> CreateTokenAsync(int? userId, string userName, int? clientId, string clientName, List<string> permissions, long tokenDurationMinutes)
        {
            string scope = _config.Scope;
            var token = new Token
            {
                Key = Guid.NewGuid().ToString("N"),
                Scope = scope.Length > 20 ? scope.Substring(0, 20) : scope,
                UserId = userId,
                UserFullName = userName,
                ClientId = clientId,
                ClientFullName = clientName,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(tokenDurationMinutes)
            };

            var repository = new TokenRepository(_serviceProvider);
            await repository.AddAsync(token);

            await _cacheService.SetValueAsync($"Auth.Tokens.{token.Scope}.{token.Key}", JsonConvert.SerializeObject(permissions), TimeSpan.FromMinutes(tokenDurationMinutes));

            return new AccessToken
            {
                Key = token.Key,
                Scope = token.Scope,
                UserId = token.UserId,
                UserFullName = token.UserFullName,
                ClientId = token.ClientId,
                ClientFullName = token.ClientFullName,
                CreatedAt = token.CreatedAt,
                ExpiresAt = token.ExpiresAt
            };
        }

        public async Task<AccessToken> CreateTokenForSynchronizerAsync(string instanceId, string userName, int? clientId, string clientName, List<string> permissions, long tokenDurationMinutes)
        {
            var scope = _config.Scope;
            var token = new Token
            {
                Key = Guid.NewGuid().ToString("N"),
                Scope = scope.Length > 20 ? scope.Substring(0, 20) : scope,
                SyncInstanceId = instanceId,
                UserFullName = userName,
                ClientId = clientId,
                ClientFullName = clientName,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(tokenDurationMinutes)
            };

            var repository = new TokenRepository(_serviceProvider);
            await repository.AddAsync(token);

            await _cacheService.SetValueAsync($"Auth.Tokens.{token.Scope}.{token.Key}", JsonConvert.SerializeObject(permissions), TimeSpan.FromMinutes(tokenDurationMinutes));

            return new AccessToken
            {
                Key = token.Key,
                Scope = token.Scope,
                SyncInstanceId = token.SyncInstanceId,
                UserFullName = token.UserFullName,
                ClientId = token.ClientId,
                ClientFullName = token.ClientFullName,
                CreatedAt = token.CreatedAt,
                ExpiresAt = token.ExpiresAt
            };
        }

        public async Task<AccessTokenWithPermissions> DecodeAndValidateTokenAsync(AccessToken injectedAccessToken, string bearerToken)
        {
            var accessTokenWithPermissions = await DecodeAndValidateTokenAsync(bearerToken);
            _mapper.Map(accessTokenWithPermissions, injectedAccessToken);
            return accessTokenWithPermissions;
        }

        public async Task<AccessTokenWithPermissions> DecodeAndValidateTokenAsync(string bearerToken)
        {
            AccessToken accessToken = null;

            if (string.IsNullOrEmpty(bearerToken))
                throw new InvalidTokenException("Token inválido.");

            try
            {
                var stringToken = bearerToken.Replace("Bearer ", string.Empty);
                accessToken = OAuthHelper.Decode(stringToken);
            }
            catch (Exception ex)
            {
                throw new InvalidTokenException("Formato token inválido.");
            }

            if (accessToken.ExpiresAt.HasValue && accessToken.ExpiresAt.Value < DateTime.Now)
                throw new InvalidTokenException("Token vencido.");

            var tokenCache = await _cacheService.GetValueAsync($"Auth.Tokens.{accessToken.Scope}.{accessToken.Key}");
            if (string.IsNullOrEmpty(tokenCache))
                throw new InvalidTokenException("Token vencido.");

            var accessTokenWithPermissions = _mapper.Map<AccessTokenWithPermissions>(accessToken);
            accessTokenWithPermissions.Permissions = JsonConvert.DeserializeObject<List<string>>(tokenCache);

            return accessTokenWithPermissions;
        }
    }
}
