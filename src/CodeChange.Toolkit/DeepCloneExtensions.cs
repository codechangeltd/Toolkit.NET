namespace CodeChange.Toolkit
{
    using System.Collections.Generic;
    using System.Linq;

    public static class DeepCloneExtensions
    {
        /// <summary>
        /// Produces another list with the same objects deeply cloned using
        /// their implementation of <see cref="IDeepCloneable{TSelf}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IEnumerable<T> DeepClone<T>(this IEnumerable<T> collection) where T : IDeepCloneable<T>
        {
            return collection.Select(item => item.DeepClone()).ToList();
        }
    }
}
