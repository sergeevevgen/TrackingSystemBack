namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IParserDbManager
    {
        Task<bool> DeleteSubjectIsDifferenceZero();
    }
}
