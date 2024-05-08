using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface IPlaceManager
    {
        /// <summary>
        /// Метод для создания или обновления помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<PlaceResponseDto> CreateOrUpdate(PlaceDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для удаления помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(PlaceDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения одного помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<PlaceResponseDto>> Read(PlaceDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения помещений по идентификаторам
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<PlaceResponseDto>> ReadAll(List<PlaceDto> model, CancellationToken cancellationToken);
    }
}
