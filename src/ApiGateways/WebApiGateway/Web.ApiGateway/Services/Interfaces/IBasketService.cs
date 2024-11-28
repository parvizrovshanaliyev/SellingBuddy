using System.Threading.Tasks;
using Web.ApiGateway.Models.Basket;

namespace Web.ApiGateway.Services.Interfaces
{
    /// <summary>
    /// Interface for handling basket-related operations.
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Retrieves a basket by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the basket.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the basket object.</returns>
        Task<Basket> GetBasketAsync(string id);

        /// <summary>
        /// Updates an existing basket or creates a new one if not found.
        /// </summary>
        /// <param name="basket">The basket to be updated or created.</param>
        /// <returns>A task representing the asynchronous operation, with the updated basket object as the result.</returns>
        Task<Basket> UpdateBasketAsync(Basket basket);
    }
}