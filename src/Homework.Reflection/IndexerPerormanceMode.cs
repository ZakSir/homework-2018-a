namespace Homework
{
    /// <summary>
    /// The performance mode of the indexer.
    /// </summary>
    public enum IndexerPerormanceMode
    {
        /// <summary>
        /// Baseline mode - No shortcuts.
        /// </summary>
        NoEnhancement = 0,

        /// <summary>
        /// High Performance Mode. Common Properties only.
        /// </summary>
        CommonProperties = 1
    }
}
