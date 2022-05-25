namespace System;

/// <summary>
/// Represents a data comparer implementation for string types
/// </summary>
public class StringDataComparer : IDataComparer<string>
{
    private readonly StringComparer _comparer;

    public StringDataComparer()
    {
        _comparer = StringComparer.CurrentCulture;
    }

    public StringDataComparer(StringComparer comparer)
    {
        _comparer = comparer;
    }

    public bool IsMatch(string candidateValue, string targetValue, DataCompareOperator compareOperator)
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
