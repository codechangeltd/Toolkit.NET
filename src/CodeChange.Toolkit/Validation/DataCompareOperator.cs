namespace System
{
    using System.ComponentModel;

    /// <summary>
    /// Supported data comparer operators for generic object comparison
    /// </summary>
    public enum DataCompareOperator
    {
        [Description("Equals")]
        Equals = 0,

        [Description("Does Not Equal")]
        NotEqual = 1,

        [Description("Greater Than")]
        GreaterThan = 2,

        [Description("Greater Than Or Equal")]
        GreaterThanEqual = 3,

        [Description("Less Than")]
        LessThan = 4,

        [Description("Less Than Or Equal")]
        LessThanEqual = 5,

        [Description("Contains")]
        Contains = 6,

        [Description("Does Not Contain")]
        DoesNotContain = 7,

        [Description("Begins With")]
        BeginsWith = 8,

        [Description("Does Not Begin With")]
        DoesNotBeginWith = 9,

        [Description("Ends With")]
        EndsWith = 10,

        [Description("Does Not End With")]
        DoesNotEndWith = 11,

        [Description("Data Type Equality Check")]
        DataTypeCheck = 12
    }
}
