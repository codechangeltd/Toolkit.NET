namespace System.Data.Entity
{
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.SqlServer;
    using System.Threading;

    /// <summary>
    /// Represents an Azure Entity Framework database configuration
    /// </summary>
    public class AzureEfConfiguration : DbConfiguration
    {
        private static AsyncLocal<bool> _suspendStrategy = new AsyncLocal<bool>()
        {
            Value = false
        };

        /// <summary>
        /// Constructs the configuration by initialising the strategy
        /// </summary>
        public AzureEfConfiguration()
        {
            SetExecutionStrategy
            (
                "System.Data.SqlClient",
                () => SuspendExecutionStrategy
                    ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
                    : new SqlAzureExecutionStrategy()
            );
        }

        /// <summary>
        /// Gets or sets the suspend the retrying execution strategy flag
        /// </summary>
        public static bool SuspendExecutionStrategy
        {
            get
            {
                return _suspendStrategy.Value;
            }
            set
            {
                _suspendStrategy.Value = value;
            }
        }
    }
}
