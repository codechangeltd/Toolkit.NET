namespace System
{
    /// <summary>
    /// Represents a data comparer implementation for string types
    /// </summary>
    public class StringDataComparer : IDataComparer<string>
    {
        private readonly StringComparer _comparer;

        /// <summary>
        /// Constructs a new string data comparer using the default configuration settings
        /// </summary>
        public StringDataComparer()
        {
            _comparer = StringComparer.CurrentCulture;
        }

        /// <summary>
        /// Constructs a new string data comparer using the string comparer specified
        /// </summary>
        /// <param name="comparer">The string comparer</param>
        public StringDataComparer(StringComparer comparer)
        {
            _comparer = comparer;
        }

        /// <summary>
        /// Determines if the candidate value matches the target value specified using the data compare operator
        /// </summary>
        /// <param name="candidateValue">The candidate value to compare</param>
        /// <param name="targetValue">The target value to compare against</param>
        /// <param name="compareOperator">The data comparer operator</param>
        /// <returns>True, if the candidate value matches the target value using the comparison operator</returns>
        public bool IsMatch
            (
                string candidateValue,
                string targetValue,
                DataCompareOperator compareOperator
            )
        {
            if (String.IsNullOrEmpty(candidateValue) || String.IsNullOrEmpty(targetValue))
            {
                return false;
            }

            switch (compareOperator)
            {
                case DataCompareOperator.Equals:
                    return _comparer.Equals(candidateValue, targetValue);

                case DataCompareOperator.NotEqual:
                    return false == _comparer.Equals(candidateValue, targetValue);

                case DataCompareOperator.GreaterThan:
                case DataCompareOperator.GreaterThanEqual:
                case DataCompareOperator.LessThan:
                case DataCompareOperator.LessThanEqual:
                    if (candidateValue.IsNumeric() && targetValue.IsNumeric())
                    {
                        var candidateNumber = Double.Parse(candidateValue);
                        var targetNumber = Double.Parse(targetValue);

                        switch (compareOperator)
                        {
                            case DataCompareOperator.GreaterThan:
                                return (candidateNumber > targetNumber);

                            case DataCompareOperator.GreaterThanEqual:
                                return (candidateNumber >= targetNumber);

                            case DataCompareOperator.LessThan:
                                return (candidateNumber < targetNumber);

                            case DataCompareOperator.LessThanEqual:
                                return (candidateNumber <= targetNumber);

                            default:
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                case DataCompareOperator.Contains:
                    return candidateValue.Contains(targetValue);

                case DataCompareOperator.DoesNotContain:
                    return false == candidateValue.Contains(targetValue);

                case DataCompareOperator.BeginsWith:
                    return candidateValue.StartsWith(targetValue);

                case DataCompareOperator.DoesNotBeginWith:
                    return false == candidateValue.StartsWith(targetValue);

                case DataCompareOperator.EndsWith:
                    return candidateValue.EndsWith(targetValue);

                case DataCompareOperator.DoesNotEndWith:
                    return false == candidateValue.EndsWith(targetValue);

                default:
                    return false;
            }
        }
    }
}
