using System;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using StaticStorageService.Shared;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Класс скрывающий работу с StaticStorage.
    /// </summary>
    public interface IStaticStorageFacade
    {
        /// <summary>
        /// Возвращает адрес для скачивания файлов по тикету.
        /// </summary>
        /// <param name="buildSourceType">Тип хранилища</param>
        /// <param name="path">Путь для расположения файлов.</param>
        /// <param name="comment">Комментарий.</param>
        /// <returns>Адрес для скачивания файлов.</returns>
        Task<string> GetUrlToDownloadFiles(BuildSourceType buildSourceType, string path, string comment = null);

        /// <summary>
        /// Возвращает адрес для загрузки файлов по тикету.
        /// </summary>
        /// <param name="buildSourceType">Тип хранилища</param>
        /// <param name="path">Путь для расположения файлов.</param>
        /// <param name="uploadOptions">Настройки загрузки..</param>
        /// <param name="uploadFileInfos">Информация о файлах.</param>
        /// <param name="comment">Комментарий.</param>
        /// <returns>Адрес для загрузки файлов.</returns>
        Task<string> GetUrlToUploadFiles(BuildSourceType buildSourceType, string path, UploadOptions uploadOptions, UploadFileInfoDto[] uploadFileInfos, string comment = null);
    }
}
