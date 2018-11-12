namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Generic interface for all aggregate root repositories
    /// </summary>
    /// <typeparam name="TRoot">The aggregate root type</typeparam>
    public interface IAggregateRepository<TRoot> : IDisposable
        where TRoot : IAggregateRoot
    { }
}
