using System;

namespace ReleaseBoard.Application.Models
{
    /// <summary>
    /// Представляет диапазон дат. Используется для фильтрации сборок по дате создания.
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// Дата начала диапазона.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания диапазона.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
