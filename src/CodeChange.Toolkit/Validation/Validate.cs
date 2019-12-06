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

        /// <summary>
        /// Ensures an integer is greater than zero
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsNotNegative
            (
                int input,
                [CallerMemberName] string memberName = ""
            )
        {
            IsNotNegative((double)input, memberName);
        }

        /// <summary>
        /// Ensures an integer is greater than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsGreaterThan
            (
                int input,
                int threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsGreaterThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures an integer is less than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must not be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsLessThan
            (
                int input,
                int threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsLessThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures an integer is between a range of numbers
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The start range</param>
        /// <param name="endRange">The end range</param>
        /// <param name="memberName">The member name</param>
        public static void IsBetween
            (
                int input,
                int startRange,
                int endRange,
                [CallerMemberName] string memberName = ""
            )
        {
            IsBetween
            (
                (double)input,
                (double)startRange,
                (double)endRange,
                memberName
            );
        }

        /// <summary>
        /// Ensures a long is greater than zero
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsNotNegative
            (
                long input,
                [CallerMemberName] string memberName = ""
            )
        {
            IsNotNegative((double)input, memberName);
        }

        /// <summary>
        /// Ensures a long is greater than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsGreaterThan
            (
                long input,
                long threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsGreaterThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures a long is less than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must not be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsLessThan
            (
                long input,
                long threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsLessThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures a long is between a range of numbers
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The start range</param>
        /// <param name="endRange">The end range</param>
        /// <param name="memberName">The member name</param>
        public static void IsBetween
            (
                long input,
                long startRange,
                long endRange,
                [CallerMemberName] string memberName = ""
            )
        {
            IsBetween
            (
                (double)input,
                (double)startRange,
                (double)endRange,
                memberName
            );
        }

        /// <summary>
        /// Ensures a decimal is greater than zero
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsNotNegative
            (
                decimal input,
                [CallerMemberName] string memberName = ""
            )
        {
            IsNotNegative((double)input, memberName);
        }

        /// <summary>
        /// Ensures a decimal is greater than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsGreaterThan
            (
                decimal input,
                decimal threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsGreaterThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures a decimal is less than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must not be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsLessThan
            (
                decimal input,
                decimal threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            IsLessThan((double)input, (double)threshold, memberName);
        }

        /// <summary>
        /// Ensures a decimal is between a range of numbers
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The start range</param>
        /// <param name="endRange">The end range</param>
        /// <param name="memberName">The member name</param>
        public static void IsBetween
            (
                decimal input,
                decimal startRange,
                decimal endRange,
                [CallerMemberName] string memberName = ""
            )
        {
            IsBetween
            (
                (double)input,
                (double)startRange,
                (double)endRange,
                memberName
            );
        }

        /// <summary>
        /// Ensures a double is greater than zero
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsNotNegative
            (
                double input,
                [CallerMemberName] string memberName = ""
            )
        {
            if (input < 0)
            {
                throw new ArgumentOutOfRangeException
                (
                    $"A parameter for {memberName} must be greater than zero."
                );
            }
        }

        /// <summary>
        /// Ensures a double is greater than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The threshold that must be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsGreaterThan
            (
                double input,
                double threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            if (input <= threshold)
            {
                throw new ArgumentOutOfRangeException
                (
                    $"A parameter for {memberName} must be greater than {threshold}."
                );
            }
        }

        /// <summary>
        /// Ensures a double is less than a specified minimum
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="threshold">The threshold that must not be exceeded</param>
        /// <param name="memberName">The member name</param>
        public static void IsLessThan
            (
                double input,
                double threshold,
                [CallerMemberName] string memberName = ""
            )
        {
            if (input >= threshold)
            {
                throw new ArgumentOutOfRangeException
                (
                    $"A parameter for {memberName} must be less than {threshold}."
                );
            }
        }

        /// <summary>
        /// Ensures a double is between a range of numbers
        /// </summary>
        /// <param name="input">The input number to validate</param>
        /// <param name="startRange">The start range</param>
        /// <param name="endRange">The end range</param>
        /// <param name="memberName">The member name</param>
        public static void IsBetween
            (
                double input,
                double startRange,
                double endRange,
                [CallerMemberName] string memberName = ""
            )
        {
            if (input < startRange || input > endRange)
            {
                throw new ArgumentOutOfRangeException
                (
                    $"A parameter for {memberName} must be between {startRange} and {endRange}."
                );
            }
        }
    }
}
