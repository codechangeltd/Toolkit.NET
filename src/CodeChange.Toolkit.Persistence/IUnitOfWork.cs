namespace CodeChange.Toolkit.Persistence
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a Unit of Work contract that can be used with repositories
    /// </summary>
    /// <remarks>
    /// The unit of work represents a transaction when used in data layers. 
    /// Typically the unit of work will roll back the transaction if SaveChanges() 
    /// has not been invoked before being disposed.
    /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Refreshes all objects being tracked with data from the data source
        /// </summary>
        void RefreshAll();

        /// <summary>
        /// Asynchronously refreshes all objects being tracked with data from the data source
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        Task RefreshAllAsync
        (
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Saves all changes made in unit to the underlying database
        /// </summary>
        /// <returns>The number of objects written to the underlying database</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in unit to the underlying database
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The number of objects written to the underlying database</returns>
        Task<int> SaveChangesAsync
        (
            CancellationToken cancellationToken = default
        );
    }
}
