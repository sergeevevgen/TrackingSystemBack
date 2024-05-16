using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface ILdapManager
    {
        bool CanAuthorize(UserLoginDto dto);

        Task SynchWithLdap();
    }
}
