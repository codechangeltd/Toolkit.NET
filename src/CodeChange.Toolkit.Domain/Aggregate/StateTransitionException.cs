namespace CodeChange.Toolkit.Domain.Aggregate
{
    /// <summary>
    /// Represents an exception for when an invalid entity state transition was attempted
    /// </summary>
    public class StateTransitionException : UserFlowException
    {
        public StateTransitionException
            (
                string message
            )
            : base(message)
        { }
    }
}
