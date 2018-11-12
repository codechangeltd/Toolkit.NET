namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    
    /// <summary>
    /// Represents an attribute that decorates a domain event as pre-transaction
    /// </summary>
    /// <remarks>
    /// Pre-transaction domain events are raised before a transaction is committed.
    /// In most cases this will be before a unit of works save changes event.
    /// 
    /// This is useful in scenarios where validation must be carried out before
    /// changes are saved back to the database.
    /// 
    /// By default, domain events are NOT pre-transaction events.
    /// </remarks>
    public class PreTransactionEventAttribute : Attribute { }
}
