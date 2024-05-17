﻿using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface IGroupManager
    {
        /// <summary>
        /// Метод для создания или обновления группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<GroupResponseDto> CreateOrUpdate(GroupDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для удаления группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(GroupDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения одной группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<GroupResponseDto>> Read(GroupDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения групп по идентификаторам
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<List<GroupResponseDto>>> ReadAll(List<GroupDto> model, CancellationToken cancellationToken = default);
    }
}
