using System;
using System.Threading.Tasks;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Класс для параллельного исполнения асинхронных задач.
    /// </summary>
    public static class TaskCombiner
    {
        /// <summary>
        /// Параллельно исполняет две задачи и возвращает кортеж с результатами их выполнения.
        /// </summary>
        /// <typeparam name="T1">Тип результата первой задачи.</typeparam>
        /// <typeparam name="T2">Тип результата второй задачи.</typeparam>
        /// <param name="task1">Первая задача.</param>
        /// <param name="task2">Вторая задача.</param>
        /// <returns>Кортеж результатов двух задач.</returns>
        public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        /// <summary>
        /// Параллельно исполняет три задачи и возвращает кортеж с результатами их выполнения.
        /// </summary>
        /// <typeparam name="T1">Тип результата первой задачи.</typeparam>
        /// <typeparam name="T2">Тип результата второй задачи.</typeparam>
        /// <typeparam name="T3">Тип результата третьей задачи.</typeparam>
        /// <param name="task1">Первая задача.</param>
        /// <param name="task2">Вторая задача.</param>
        /// <param name="task3">Третья задача.</param>
        /// <returns>Кортеж результатов трех задач.</returns>
        public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            await Task.WhenAll(task1, task2, task3);
            return (task1.Result, task2.Result, task3.Result);
        }

        /// <summary>
        /// Параллельно исполняет три задачи и возвращает кортеж с результатами их выполнения.
        /// </summary>
        /// <typeparam name="T1">Тип результата первой задачи.</typeparam>
        /// <typeparam name="T2">Тип результата второй задачи.</typeparam>
        /// <typeparam name="T3">Тип результата третьей задачи.</typeparam>
        /// <typeparam name="T4">Тип результата четвертой задачи.</typeparam>
        /// <param name="task1">Первая задача.</param>
        /// <param name="task2">Вторая задача.</param>
        /// <param name="task3">Третья задача.</param>
        /// <param name="task4">Четвертая задача.</param>
        /// <returns>Кортеж результатов трех задач.</returns>
        public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
        {
            await Task.WhenAll(task1, task2, task3, task4);
            return (task1.Result, task2.Result, task3.Result, task4.Result);
        }

        /// <summary>
        /// Параллельно исполняет три задачи и возвращает кортеж с результатами их выполнения.
        /// </summary>
        /// <typeparam name="T1">Тип результата первой задачи.</typeparam>
        /// <typeparam name="T2">Тип результата второй задачи.</typeparam>
        /// <typeparam name="T3">Тип результата третьей задачи.</typeparam>
        /// <typeparam name="T4">Тип результата четвертой задачи.</typeparam>
        /// <typeparam name="T5">Тип результата пятой задачи.</typeparam>
        /// <param name="task1">Первая задача.</param>
        /// <param name="task2">Вторая задача.</param>
        /// <param name="task3">Третья задача.</param>
        /// <param name="task4">Четвертая задача.</param>
        /// <param name="task5">Пятая задача.</param>
        /// <returns>Кортеж результатов трех задач.</returns>
        public static async Task<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
        {
            await Task.WhenAll(task1, task2, task3, task4, task5);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }
    }
}
