using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public class User : ValueObject<User>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="sid">Сид.</param>
        /// <param name="name">Имя.</param>
        public User(string sid, string name)
        {
            sid.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(User), "Cannot create User: sid not defined "));
            name.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(User),"Cannot create User: name not defined "));

            Sid = sid;
            Name = name;
        }

        /// <summary>
        /// Полное ФИО пользователя.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string Sid { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Name}, {Sid}";

        /// <inheritdoc />
        protected override bool EqualsCore(User other) => Name == other.Name && Sid == other.Sid;

        /// <inheritdoc />
        protected override int GetHashCodeCore() => HashCode.Combine(Name, Sid);
    }
}
