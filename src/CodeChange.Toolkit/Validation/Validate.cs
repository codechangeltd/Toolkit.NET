namespace System
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides various static parameter validation methods
    /// </summary>
    public static class Validate
    {
        /// <summary>
        /// Checks that an object value is not null
        /// </summary>
        /// <param name="o">The value to check</param>
        /// <param name="memberName">The member name</param>
        /// <param name="sourceLineNumber">The source line number</param>
        public static void IsNotNull
            (
                object o,
                [CallerMemberName] string memberName = "",
                [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (o == null)
            {
                throw new ArgumentNullException
                (
                    $"A parameter for {memberName} at line {sourceLineNumber} was null."
                );
            }
        }

        /// <summary>
        /// Ensures a string has a value (i.e. it is not null or empty)
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <param name="memberName">The member name</param>
        /// <param name="sourceLineNumber">The source line number</param>
        public static void IsNotEmpty
            (
                string input,
                [CallerMemberName] string memberName = "",
                [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException
                (
                    $"A parameter for {memberName} at line {sourceLineNumber} was null or empty."
                );
            }
        }
    }
}
