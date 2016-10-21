namespace Domain.Tests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class GuardTests
    {
        [Test]
        public void IntMoreOrEqualToZero_ValueIsEqualToZero_DoesNotThrowAnyException()
        {
            // Arrange
            int someValue = 0;
            string someValueName = "someValue";

            // Act and Assert
            Assert.DoesNotThrow(() => Guard.IntMoreOrEqualToZero(someValue, someValueName));
        }

        [Test]
        public void IntMoreOrEqualToZero_ValueIsGreaterThanZero_DoesNotThrowAnyException()
        {
            // Arrange
            int someValue = 1;
            string someValueName = "someValue";

            // Act and Assert
            Assert.DoesNotThrow(() => Guard.IntMoreOrEqualToZero(someValue, someValueName));
        }

        [Test]
        public void IntMoreOrEqualToZero_ValueIsLessThanZero_ThrowsArgumentOutOfRangeExceptionWithInfo()
        {
            // Arrange
            int someValue = -1;
            string someValueName = "someValue";

            // Act and Assert
            ArgumentOutOfRangeException exception =
                Assert.Throws<ArgumentOutOfRangeException>(() => Guard.IntMoreOrEqualToZero(someValue, someValueName));

            Assert.AreEqual(someValueName, exception.ParamName);
        }

        [Test]
        public void NotNull_ArgumentIsNotNull_DoesNotThrowAnyException()
        {
            // Arrange
            object someArgument = new object();
            string someArgumentName = "someArgument";

            // Act and Assert
            Assert.DoesNotThrow(() => Guard.NotNull(someArgument, someArgumentName));
        }

        [Test]
        public void NotNull_ArgumentIsNull_ThrowsArgumentNullExceptionWithInfo()
        {
            // Arrange
            string someArgumentName = "someArgument";

            // Act and Assert
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(() => Guard.NotNull(null, someArgumentName));

            Assert.AreEqual(someArgumentName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_EnumerableIsEmpty_ThrowsArgumentExceptionWithInfo()
        {
            // Arrange
            IEnumerable<object> argument = Enumerable.Empty<object>();
            string argumentName = "someArgument";

            // Act and Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => Guard.NotNullOrEmpty(argument, argumentName));

            Assert.AreEqual(argumentName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_EnumerableIsNotEmpty_DoesNotThrowAnyException()
        {
            // Arrange
            List<object> argument = new List<object> { new object() };
            string argumentName = "someArgument";

            // Act and Assert
            Assert.DoesNotThrow(() => Guard.NotNullOrEmpty(argument, argumentName));
        }

        [Test]
        public void NotNullOrEmpty_EnumerableIsNull_ThrowsArgumentExceptionWithInfo()
        {
            // Arrange
            string argumentName = "someArgument";

            // Act and Assert
            ArgumentException exception = Assert.Throws<ArgumentNullException>(() => Guard.NotNullOrEmpty(null, argumentName));

            Assert.AreEqual(argumentName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_StringIsEmpty_ThrowsArgumentExceptionWithInfo()
        {
            // Arrange
            string someString = string.Empty;
            string someStringName = "someString";

            // Act and Assert
            ArgumentException exception =
                Assert.Throws<ArgumentException>(() => Guard.NotNullOrEmpty(someString, someStringName));

            Assert.AreEqual(someStringName, exception.ParamName);
        }

        [Test]
        public void NotNullOrEmpty_StringIsNotEmpty_DoesNotThrowAnyException()
        {
            // Arrange
            string someString = "This is string.";
            string someStringName = "someString";

            // Act and Assert
            Assert.DoesNotThrow(() => Guard.NotNullOrEmpty(someString, someStringName));
        }

        [Test]
        public void NotNullOrEmpty_StringIsNull_ThrowsArgumentNullExceptionWithInfo()
        {
            // Arrange
            string someStringName = "someString";

            // Act and Assert
            ArgumentNullException exception =
                Assert.Throws<ArgumentNullException>(() => Guard.NotNullOrEmpty(null, someStringName));

            Assert.AreEqual(someStringName, exception.ParamName);
        }
    }
}