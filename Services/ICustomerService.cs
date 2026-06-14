using RepoApi.Models;
using RepoApi.Models.DTOs;

namespace RepoApi.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetAllCustomersAsync ();

        Task<CustomerDTO?> GetCustomerByIdAsync(int id);

        Task<Customer?> CreateCustomerAsync(Customer customer);

        Task<Customer?> UpdateCustomerAsync(Customer customer , int id);

        Task<bool> DeleteCustomerAsync(int id);
    }
}
