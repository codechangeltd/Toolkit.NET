namespace System
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ArrayExtensions
    {
        /// <summary>
        /// Joins two arrays of type T together and returns the resulting re-dimensioned array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array1">The first array</param>
        /// <param name="array2">The second array</param>
        /// <param name="removeDuplicates">If true, all duplicates are removed from the joined array</param>
        /// <returns>The combined arrays</returns>
        public static T[] JoinArrays<T>(this T[] array1, T[] array2, bool removeDuplicates = true)
        {
            if (array1.Length == 0)
            {
                return array2;
            }

            if (array2 == null || array2.Length == 0)
            {
                return array1;
            }

            var combinedList = new List<T>();

            combinedList.AddRange(array1);
            combinedList.AddRange(array2);

            if (removeDuplicates)
            {
                return combinedList.Distinct().ToArray();
            }
            else
            {
                return combinedList.ToArray();
            }
        }

        /// <summary>
        /// Determines if an array of integers is sequential
        /// </summary>
        /// <param name="array">The array to check</param>
        /// <returns>True, if the numbers are sequential; otherwise false</returns>
        public static bool IsSequential(this int[] array)
        {
            if (array == null)
            {
                return false;
            }
            else
            {
                var zipped = array.Zip(array.Skip(1), (a, b) => (a + 1) == b);

                return zipped.All(_ => _);
            }
        }
    }
}
