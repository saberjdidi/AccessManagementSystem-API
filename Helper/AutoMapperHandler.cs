using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using AutoMapper;

namespace AccessManagementSystem_API.Helper
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<Customer, CustomerDto>().ForMember(item => item.Statusname, opt => opt.MapFrom(
                item => (item.IsActive != null && item.IsActive.Value) ? "Active" : "In active")).ReverseMap();
        }
    }
}
