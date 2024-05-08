using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface IParserManager
    {
        Task<ResponseModel<string>> ParseTimetable();
    }
}
