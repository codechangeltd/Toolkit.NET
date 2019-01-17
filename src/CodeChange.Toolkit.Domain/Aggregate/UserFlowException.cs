namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;
    
    /// <summary>
    /// Represents an exception for when a user attempted an invalid operation
    /// </summary>
    public class UserFlowException : Exception
    {
        public UserFlowException
            (
                string message
            )
            : base(message)
        { }
    }
}
