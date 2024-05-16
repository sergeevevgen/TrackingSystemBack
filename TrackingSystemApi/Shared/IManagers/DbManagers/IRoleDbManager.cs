using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface IRoleDbManager
    {
        Guid? GetElement(ERoles role);
    }
}
