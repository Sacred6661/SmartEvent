using IdentityService.Models;
using IdentityService.Models.DTOs;
using Mapster;

namespace IdentityService.Mapping
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ApplicationUser, UserInfo>();
        }
    }
}
