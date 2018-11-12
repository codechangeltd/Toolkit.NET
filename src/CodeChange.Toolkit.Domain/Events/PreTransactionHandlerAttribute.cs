namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    
    /// <summary>
    /// Represents an attribute that decorates a domain event handler as pre-transaction
    /// </summary>
    /// <remarks>
    /// Pre-transaction event handlers are called before a transaction is committed.
    /// In most cases this will be before a unit of works save changes event.
    /// 
    /// This is useful in scenarios where validation must be carried out before
    /// changes are saved back to the database.
    /// 
    /// By default, domain event handlers are NOT pre-transaction handlers.
    /// </remarks>
    public class PreTransactionHandlerAttribute : Attribute { }
}
