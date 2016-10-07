namespace App.Utils
{
    using System;

    /// <summary>
    ///     A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
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
                throw new ArgumentNullException(argumentName, String.Format("Argument '{0}' must not be null.", argumentName));
            }
        }
    }
}