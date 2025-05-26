namespace SimpleMongoMigrations
{
    /// <summary>
    /// Migration transaction scope.
    /// </summary>
    public enum TransactionScope
    {
        /// <summary>
        /// Apply migrations without transactions.
        /// </summary>
        NoTransaction,

        /// <summary>
        /// Apply each migration in a separate transaction.
        /// </summary>
        PerMigration,

        /// <summary>
        /// Apply all migrations in a single transaction.
        /// </summary>
        SingleTransaction
    }
}
