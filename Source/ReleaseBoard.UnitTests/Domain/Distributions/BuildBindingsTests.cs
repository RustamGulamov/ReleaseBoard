using System;
using Force.DeepCloner;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions
{
    /// <summary>
    /// Тесты для привязок к сборкам.
    /// </summary>
    public class BuildBindingsTests
    {
        /// <summary>
        /// Проверяет, что копия равна оригиналу.
        /// </summary>
        [Fact]
        public void CopyEqualsToOriginal()
        {
            BuildsBinding binding = FakeGenerator.GetBuildsBinding();

            BuildsBinding copy = binding.DeepClone();

            Assert.True(binding.Equals(copy));
            Assert.Equal(binding.GetHashCode(), copy.GetHashCode());
        }

        /// <summary>
        /// Проверяет симметричность сравнения.
        /// </summary>
        [Fact]
        public void Symmetry()
        {
            BuildsBinding a = FakeGenerator.GetBuildsBinding();
            BuildsBinding b = FakeGenerator.GetBuildsBinding();

            bool
                primary = a.Equals(b),
                backwards = b.Equals(a);

            Assert.Equal(primary, backwards);
        }

        /// <summary>
        /// Проверка на транзитивность.
        /// </summary>
        [Fact]
        public void Transitivity()
        {
            BuildsBinding a = FakeGenerator.GetBuildsBinding();
            BuildsBinding b = FakeGenerator.GetBuildsBinding();
            BuildsBinding c = FakeGenerator.GetBuildsBinding();

            bool ab = a.Equals(b), ac = a.Equals(c), bc = b.Equals(c);

            Assert.Equal(ab && ac, bc);
        }
    }
}
