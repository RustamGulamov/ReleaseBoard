using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Services;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain
{
    /// <summary>
    /// Тесты для сервиса дистрибутивов.
    /// </summary>
    public class CollectionsComparerTests
    {
        private readonly CollectionsComparer comparer;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public CollectionsComparerTests()
        {
            comparer = new CollectionsComparer();
        }

        /// <summary>
        /// Коллекции для проверки на дифф с результатми.
        /// </summary>
        public static IEnumerable<object[]> NewAndOldCollectionsWithDiffResults => new[]
        {
            new object[]
            {
                new List<int>() { 1, 2, 3, 4 }, // old collection
                new List<int>() { 1, 5, 6 }, // new collection
                new List<int>() { 2, 3, 4 }, // remove items
                new List<int>() { 5, 6 }, // new items
            },
            new object[]
            {
                new List<int>() { 1, 2, }, // old collection
                new List<int>() { 5, 6 }, // new collection
                new List<int>() { 1, 2 }, // remove items
                new List<int>() { 5, 6 }, // new items
            },
            new object[]
            {
                new List<int>() { 1, 2, 3 }, // old collection
                new List<int>() { 1, 2, 3 }, // new collection
                new List<int>() { }, // remove items
                new List<int>() { }, // new items
            },
        };

        /// <summary>
        /// Тесты проверки на дифф.
        /// </summary>
        /// <param name="oldCollection">Старая коллекция.</param>
        /// <param name="newCollection">Новая коллекция.</param>
        /// <param name="expectedRemoveItems">Удаляемые объекты.</param>
        /// <param name="expectedNewItems">Новые объекты.</param>
        [Theory]
        [MemberData(nameof(NewAndOldCollectionsWithDiffResults))]
        public void GeneratedBindingsDiffCorrectly(List<int> oldCollection, List<int> newCollection, List<int> expectedRemoveItems, List<int> expectedNewItems)
        {
            (IList<int> newItems, IList<int> removedItems) = comparer.Compare(oldCollection, newCollection);

            Assert.Equal(expectedRemoveItems, removedItems);
            Assert.Equal(expectedNewItems, newItems);
        }
    }
}
