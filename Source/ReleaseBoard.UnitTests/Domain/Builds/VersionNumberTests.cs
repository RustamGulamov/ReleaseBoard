using System;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.Domain.ValueObjects.Exceptions;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Тесты для VersionNumber.
    /// </summary>
    public class VersionNumberTests
    {
        /// <summary>
        /// Проверка что номер включает в себя другой номер.
        /// </summary>
        [Fact]
        public void VersionNumberCreate_InvalidNumber_Exception()
        {
            Assert.Throws<CreateValueObjectException>(() => new VersionNumber("some strage"));
            Assert.Throws<CreateValueObjectException>(() => new VersionNumber("1.3.3.2.2"));
            Assert.Throws<CreateValueObjectException>(() => new VersionNumber("1.xx3.3aasd.2asd"));
        }

        /// <summary>
        /// Проверка что номер включает в себя другой номер.
        /// </summary>
        [Fact]
        public void VersionNumberContainsOther_Succeed()
        {
            VersionNumber number1 = new VersionNumber("1.3.4.442");
            VersionNumber number2 = new VersionNumber("1.3");

            Assert.True(number2.IsInclude(number1));
        }
    }
}
