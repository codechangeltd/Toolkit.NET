namespace CodeChange.Toolkit.Domain.Events
{
    using System;
    
    /// <summary>
    /// Represents an attribute that decorates a domain event handler as post-transaction
    /// </summary>
    /// <remarks>
    /// Post-transaction domain event handlers are called after a transaction completes.
    /// In most cases this will be after a unit of works save changes event.
    /// 
    /// This is useful in scenarios where domain event handlers should only 
    /// be called after the event transaction was successfully completed.
    /// 
    /// For example:
    /// A user account is created and an event is raised. There is an event 
    /// handler that will send an email to the user to confirm their account 
    /// has been created. However, we only want to send this email when their 
    /// account has been successfully saved to the database. 
    /// 
    /// With a post-transaction event, the event will only be raised after the
    /// transaction has taken place and therefore the email will only be sent
    /// once the user account has been saved to the database.
    /// 
    /// By default, domain events are post-transaction events.
    /// </remarks>
    public class PostTransactionHandlerAttribute : Attribute { }
}
