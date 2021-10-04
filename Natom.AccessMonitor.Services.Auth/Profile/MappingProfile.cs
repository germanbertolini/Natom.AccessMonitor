using AutoMapper;
using Natom.AccessMonitor.Services.Auth.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Services.Auth.Profile
{
    public static class MappingProfile
    {
        public static MapperConfiguration Build()
                            => new MapperConfiguration(cfg =>
                                {
                                    cfg.CreateMap<AccessToken, AccessTokenWithPermissions>().ReverseMap();
                                });
    }
}
