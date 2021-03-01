using System;
using System.Net.Http;
using System.Threading.Tasks;
using StaticStorageService.Shared;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Поставщик Url для работы со статическим сервисом.
    /// </summary>
    public class StaticStorageClient
    {
        private const string DownloadFilesControllerPath = "api/Download";
        private const string TicketsApi = "api/tickets";
        private const string UploadFilesControllerPath = "api/UploadFiles";

        private readonly HttpClient httpClient;
        private string serviceUrl;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="httpClient"><see cref="HttpClient"/>Http клиент.</param>
        public StaticStorageClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Путь до сервиса.
        /// </summary>
        public string ServiceUrl
        {
            get => serviceUrl;
            set
            {
                serviceUrl = value;
                httpClient.BaseAddress = new Uri(value);
            }
        }

        /// <summary>
        /// Возвращает адрес для скачивания файлов по тикету.
        /// </summary>
        /// <param name="path">Путь для расположения файлов.</param>
        /// <param name="comment">Комментарий.</param>
        /// <returns>Адрес для скачивания файлов.</returns>
        public async Task<string> GetUrlToDownloadFilesByTicket(string path, string comment = null)
        {
            TicketDtoBase ticket = await GetDownloadTicket(path, comment);

            return $"{ServiceUrl}/{DownloadFilesControllerPath}/{ticket.Id}";
        }

        /// <summary>
        /// Возвращает адрес для загрузки файлов по тикету.
        /// </summary>
        /// <param name="path">Путь для расположения файлов.</param>
        /// <param name="uploadOptions">Настройки загрузки..</param>
        /// <param name="uploadFilesInfo">Информация о файлах.</param>
        /// <param name="comment">Комментарий.</param>
        /// <returns>Адрес для загрузки файлов.</returns>
        public async Task<string> GetUrlToUploadFilesByTicket(string path, UploadOptions uploadOptions, UploadFileInfoDto[] uploadFilesInfo, string comment = null)
        {
            TicketDtoBase ticket = await GetUploadTicket(path, uploadOptions, uploadFilesInfo, comment);

            return $"{ServiceUrl}/{UploadFilesControllerPath}/{ticket.Id}";
        }

        private async Task<TicketDtoBase> GetDownloadTicket(string path, string comment = null)
        {
            var uri = $"{TicketsApi}/downloadTicket";
            var content = new { Comment = comment ?? string.Empty, Path = path };

            return await Lighthouse.Contracts.Extensions.HttpClientExtensions.PostAsJsonAsync<DownloadTicketDto>(httpClient, uri, content);
        }

        private async Task<TicketDtoBase> GetUploadTicket(string path, UploadOptions uploadOptions, UploadFileInfoDto[] uploadFilesInfo, string comment = null)
        {
            var uri = $"{TicketsApi}/uploadTicket";
            var content = new { Comment = comment ?? string.Empty, Path = path, UploadFiles = uploadFilesInfo, UploadOptions = uploadOptions };

            return await Lighthouse.Contracts.Extensions.HttpClientExtensions.PostAsJsonAsync<DownloadTicketDto>(httpClient, uri, content);
            
        }
    }
}
