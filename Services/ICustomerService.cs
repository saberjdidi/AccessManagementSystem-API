using AccessManagementSystem_API.Dtos;

namespace AccessManagementSystem_API.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> Getall();
        Task<CustomerDto> Getbycode(string code);
        Task<APIResponse> Remove(string code);
        Task<APIResponse> Create(CustomerDto data);

        Task<APIResponse> Update(CustomerDto data, string code);
    }
}
