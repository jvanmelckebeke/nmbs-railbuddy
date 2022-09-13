namespace Eindwerk.Models.Rail.PackedResponses
{
    /// <summary>
    ///     interface for defining responses which are of the form
    ///     <code lang="json">
    /// {
    ///     "version":"1.1",
    ///     "timestamp":1489622781,
    ///     "&lt;the actual content&gt;": {...}
    /// }
    /// </code>
    ///     and thus difficult to base a single datatype on
    /// </summary>
    /// <typeparam name="T">The Type of the actual content</typeparam>
    public interface IPackedResponse<out T> : IDtoModel
    {
        /// <summary>
        ///     returns the actual content of the packed response
        /// </summary>
        /// <returns>the actual content as type <typeparamref name="T" /></returns>
        T Content { get; }
    }
}