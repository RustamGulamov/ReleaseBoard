using System;

namespace ReleaseBoard.Web.ApiModels
{
    /// <summary>
    /// Модель для создания паттерн распознавания сборок.
    /// </summary>
    public class NewBuildMatchPatternModel
    {
        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Краткое описание, пример подходящей строки.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Регулярное выражение.
        /// </summary>
        public string Regexp { get; set; }
    }
}
