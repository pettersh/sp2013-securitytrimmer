
namespace Pzl.SecurityTrimmer
{
    /// <summary>
    /// Interface for user repositories
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Lookup a user in the repository
        /// </summary>
        /// <param name="key">the user property to search for</param>
        /// <returns>the user property, or null if not found</returns>
        string Lookup(string key);
    }
}
