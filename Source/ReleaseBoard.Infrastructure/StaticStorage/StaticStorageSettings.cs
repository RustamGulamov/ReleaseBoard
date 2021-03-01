using System;
using System.Collections.Generic;
using ReleaseBoard.Common.Contracts.Abstractions;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Настройки сервисов StaticStorage.
    /// </summary>
    public class StaticStorageSettings
    {
        /// <summary>
        /// Адреса для сервисов.
        /// </summary>
        public Dictionary<BuildSourceType, string> Services { get; } = new Dictionary<BuildSourceType, string>();
    }
}
