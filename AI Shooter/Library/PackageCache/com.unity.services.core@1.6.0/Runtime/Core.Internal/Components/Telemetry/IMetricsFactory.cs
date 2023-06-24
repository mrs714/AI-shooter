using System.Collections.Generic;
using Unity.Services.Core.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
    /// <summary>
    /// Component to get or create the proper <see cref="IMetrics"/> for a given package.
    /// </summary>
    public interface IMetricsFactory : IServiceComponent
    {
        /// <summary>
        /// All tags shared among all metrics events generated by <see cref="IMetrics"/> created by this factory.
        /// </summary>
        IReadOnlyDictionary<string, string> CommonTags { get; }

        /// <summary>
        /// Create a <see cref="IMetrics"/> setup with common tags for the given <paramref name="packageName"/>.
        /// </summary>
        /// <param name="packageName">
        /// The name of the package that will use the created <see cref="IMetrics"/> to send metric events.
        /// Example value: "com.unity.services.core"
        /// </param>
        /// <returns>
        /// Return a <see cref="IMetrics"/> setup with common tags for the given <paramref name="packageName"/>.
        /// </returns>
        IMetrics Create(string packageName);
    }
}