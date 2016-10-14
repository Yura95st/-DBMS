namespace App.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        ///     Throws <see cref="ArgumentOutOfRangeException" /> if the given argument is less than 0.
        /// </summary>
        /// <param name="argumentIntValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void IntMoreOrEqualToZero(int argumentIntValue, string argumentName)
        {
            if (argumentIntValue < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName,
                    $"Argument '{argumentName}' must be greater or equal to 0.");
            }
        }

        /// <summary>
        ///     Throws <see cref="ArgumentNullException" /> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName, $"Argument '{argumentName}' must not be null.");
            }
        }

        /// <summary>
        ///     Throws an exception if the tested string argument is null or an empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">The string value is null.</exception>
        /// <exception cref="ArgumentException">The string is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNullOrEmpty(string argumentValue, string argumentName)
        {
            Guard.NotNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException($"Argument '{argumentName}' must not be empty.", argumentName);
            }
        }

        /// <summary>
        ///     Throws an exception if the tested argument is null or an empty enumerable.
        /// </summary>
        /// <exception cref="ArgumentNullException">The argument value is null.</exception>
        /// <exception cref="ArgumentException">The argument is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void NotNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            Guard.NotNull(argumentValue, argumentName);

            if (!argumentValue.Any())
            {
                throw new ArgumentException($"Argument '{argumentName}' must not be empty.", argumentName);
            }
        }
    }
}