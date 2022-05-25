namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Represents an exception for when an invalid entity state transition was attempted
    /// </summary>
    public class StateTransitionException : Exception
    {
        public StateTransitionException(string message)
            : base(message)
        { }
    }
}
