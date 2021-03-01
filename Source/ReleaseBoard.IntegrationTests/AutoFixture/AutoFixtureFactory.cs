using System;
using AutoFixture;
using AutoFixture.Kernel;
using ReleaseBoard.IntegrationTests.AutoFixture.Customizations;

namespace ReleaseBoard.IntegrationTests.AutoFixture
{
    /// <summary>
    /// Фабрика для создания автофикстур <see cref="IFixture"/>.
    /// </summary>
    public class AutoFixtureFactory
    {
        private static readonly ISpecimenBuilder randomBooleanGenerator = new RandomBooleanSequenceGenerator();
        private static readonly ISpecimenBuilderTransformation omitRecursionBehavior = new OmitOnRecursionBehavior();
        private static readonly ICustomization buildsCustomization = new BuildsCustomization();

        /// <summary>
        /// Создает экземпляр <see cref="IFixture"/>.
        /// </summary>
        /// <returns>Настроенный экземпляр <see cref="Fixture"/>.</returns>
        public static IFixture Create()
        {
            return new Fixture
            {
                Behaviors = { omitRecursionBehavior },
                Customizations = { randomBooleanGenerator }
            };
        }

        /// <summary>
        /// Создает экземпляр <see cref="IFixture"/>.
        /// </summary>
        /// <returns>Настроенный экземпляр <see cref="Fixture"/> для создания кастомных сборок.</returns>
        public static IFixture CreateWithCustomizedBuilds()
        {
            return Create().Customize(buildsCustomization);
        }
    }
}
