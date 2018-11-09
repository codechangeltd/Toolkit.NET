namespace System
{
    /// <summary>
    /// Defines a contract for a service that compares that values of two objects
    /// </summary>
    /// <typeparam name="T">The type we are comparing</typeparam>
    public interface IDataComparer<T>
    {
        /// <summary>
        /// Determines if the candidate value matches the target value specified using the data compare operator
        /// </summary>
        /// <param name="candidateValue">The candidate value to compare</param>
        /// <param name="targetValue">The target value to compare against</param>
        /// <param name="compareOperator">The data comparer operator</param>
        /// <returns>True, if the candidate value matches the target value using the comparison operator</returns>
        bool IsMatch
        (
            T candidateValue,
            T targetValue,
            DataCompareOperator compareOperator
        );
    }
}
