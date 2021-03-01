using System;
using System.Collections.Generic;

namespace ReleaseBoard.Web.Settings
{
    /// <summary>
    /// Настройки адресов хранилищ сборок.
    /// </summary>
    public class BuildStorageUrlSettings
    {
        /// <summary>
        /// Базовые адреса для открытия сборок.
        /// </summary>
        public Dictionary<BuildSourceType, string> BaseUrlsToOpenBuilds { get; set; } = new Dictionary<BuildSourceType, string>();
    }
}
