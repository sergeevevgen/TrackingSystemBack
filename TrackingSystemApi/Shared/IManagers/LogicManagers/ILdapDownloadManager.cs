using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface ILdapDownloadManager
    {
        Task SynchWithLdap();
    }
}
