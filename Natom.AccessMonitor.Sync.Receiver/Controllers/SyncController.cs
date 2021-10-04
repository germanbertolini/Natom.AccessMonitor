using Microsoft.AspNetCore.Mvc;
using Natom.AccessMonitor.Services.Auth.Services;
using Natom.AccessMonitor.Services.Cache.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Sync.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SyncController : BaseController
    {
        private readonly AuthService _authService;
        private readonly CacheService _cacheService;

        public SyncController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            _cacheService = (CacheService)serviceProvider.GetService(typeof(CacheService));
        }
    }
}
