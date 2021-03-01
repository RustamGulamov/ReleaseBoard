using System;
using System.ComponentModel.DataAnnotations;

namespace ReleaseBoard.Web.ApiModels.BuildModels
{
    /// <summary>
    /// Информация о загружаемых файлах сборки.
    /// </summary>
    public class BuildFileInfoDto
    {
        /// <summary>
        /// Название файла.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string FileName { get; set; }

        /// <summary>
        /// Индикатор необходимости распаковки файла.
        /// </summary>
        public bool ShouldUnzip { get; set; }
    }
}
