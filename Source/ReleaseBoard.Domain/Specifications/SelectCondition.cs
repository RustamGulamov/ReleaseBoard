using System;

namespace ReleaseBoard.Domain.Specifications
{
    /// <summary>
    /// Условие выборки.
    /// </summary>
    public enum SelectCondition
    {
        /// <summary>
        /// Условие ИЛИ.
        /// </summary>
        Or = 0,

        /// <summary>
        /// Условие И.
        /// </summary>
        And = 1
    }
}
