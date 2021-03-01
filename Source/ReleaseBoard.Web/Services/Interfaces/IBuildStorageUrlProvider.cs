using System;
using System.Threading.Tasks;

namespace ReleaseBoard.Web.Services.Interfaces
{
    /// <summary>
    /// Сервис для генерации ссылок на сборки.
    /// </summary>
    public interface IBuildStorageUrlProvider
    {
        /// <summary>
        /// Возвращает ссылку на скачивание сборки.
        /// </summary>
        /// <exception cref="NotSupportedException">В случае не поддерживаемого <see cref="BuildSourceType"/>.</exception>
        /// <param name="buildSourceType"><see cref="BuildSourceType"/>.</param>
        /// <param name="buildLocation">Относительный путь сборки.</param>
        /// <returns>Ссылка для открытия сборки.</returns>
        Task<string> GetOpenUrl(BuildSourceType buildSourceType, string buildLocation);
    }
}
