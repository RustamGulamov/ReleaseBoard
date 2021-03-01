using System;

namespace ReleaseBoard.Domain.Builds
{
    /// <summary>
    /// Cостояние артефакта.
    /// </summary>
    public enum ArtifactState
    {
        /// <summary>
        /// Создано.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Обновлено.
        /// </summary>
        Updated = 1,

        /// <summary>
        /// Не отслеживается.
        /// </summary>
        UnTracked = 2,

        /// <summary>
        /// Возврашено на отслеживается.
        /// </summary>
        ReturnToTracked = 3,

        /// <summary>
        /// Архивировано.
        /// </summary>
        Archived = 4,
    }
}
