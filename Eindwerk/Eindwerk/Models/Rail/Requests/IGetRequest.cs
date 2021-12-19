namespace Eindwerk.Models.Rail.Requests
{
    public interface IGetRequest : IDtoModel
    {
        /// <summary>
        ///     creates get parameter string from request object
        /// </summary>
        /// <returns>get parameter string from the object instance</returns>
        string ToGetParameters();
    }
}