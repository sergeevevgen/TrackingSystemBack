using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface ILdapAuthManager
    {
        bool CanAuthorize(UserLoginDto dto);
    }
}
