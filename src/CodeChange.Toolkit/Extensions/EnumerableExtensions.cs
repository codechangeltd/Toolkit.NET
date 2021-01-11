namespace System.Collections.Generic
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    
    /// <summary>
    /// Various extension methods for the IEnumerable interface
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Sorts the enumerable collection by the column and direction specified
        /// </summary>
        /// <typeparam name="T">The enumerable type</typeparam>
        /// <param name="list">A list of items to sort</param>
        /// <param name="column">The name of the column to sort by</param>
        /// <param name="direction">The sort direction</param>
        /// <returns>An ordered enumerable of items</returns>
        /// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
        /// <exception cref="System.Reflection.TargetException"></exception>
        /// <exception cref="System.Reflection.TargetParameterCountException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="System.Reflection.TargetInvocationException"></exception>
        public static IOrderedEnumerable<T> Sort<T>
            (
                this IEnumerable<T> list,
                string column,
                ListSortDirection direction = ListSortDirection.Ascending
            )
        {
            Validate.IsNotNull(list);
            Validate.IsNotEmpty(column);

            Func<T, object> selector = (_ => _.GetType().GetProperty(column).GetValue(_, null));

            return direction == ListSortDirection.Ascending
                ? list.OrderBy(selector)
                : list.OrderByDescending(selector);
        }
    }
}
