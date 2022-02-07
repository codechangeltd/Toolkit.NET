namespace System.Collections.Generic
{
    using System;

    public static class ListExtensions
    {
        /// <summary>
        /// Adds only the unique items found in the collection specified to the list
        /// </summary>
        /// <typeparam name="T">The list type</typeparam>
        /// <param name="initialList">The list being updated</param>
        /// <param name="newCollection">The new collection of items to add</param>
        /// <remarks>
        /// This algorithm uses the object default equality comparer to check for uniqueness
        /// TODO: create overload that accepts an Comparer[T] argument
        /// </remarks>
        public static void AddUnique<T>(this List<T> initialList, IEnumerable<T> newCollection)
        {
            if (newCollection == null)
            {
                throw new ArgumentException("The new collection cannot be null.");
            }

            foreach (var item in newCollection)
            {
                if (false == initialList.Contains(item))
                {
                    initialList.Add(item);
                }
            }
        }
    }
}
