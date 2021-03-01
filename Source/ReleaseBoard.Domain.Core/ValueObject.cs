using System;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Класс объект-значение.
    /// </summary>
    public abstract class ValueObject<T> where T : class
    {
        /// <summary>
        /// Оператор ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>Результат.</returns>
        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Оператор !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>Результат.</returns>
        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            T valueObject = obj as T;
            return EqualsCore(valueObject);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return GetHashCodeCore();
            }
        }
        
        /// <summary>
        /// Метод сравнения объектов конкретного типа.
        /// </summary>
        /// <param name="other">Сравниваемый объект.</param>
        /// <returns>Результат сравнения.</returns>
        protected abstract bool EqualsCore(T other);

        /// <summary>
        /// Возвращает хэш код.
        /// </summary>
        /// <returns>Число.</returns>
        protected abstract int GetHashCodeCore();
    }
}
