using AccessManagementSystem_API.Data;
using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem_API.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;
        public CustomerService(AppDbContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<APIResponse> Create(CustomerDto data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this.logger.LogInformation("Create Begins");
                Customer _customer = this.mapper.Map<CustomerDto, Customer>(data);
                await this.context.Customers.AddAsync(_customer);
                await this.context.SaveChangesAsync();
                response.ResponseCode = 201;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Message = ex.Message;
                this.logger.LogError(ex.Message, ex);
            }
            return response;
        }

        public async Task<List<CustomerDto>> Getall()
        {
            List<CustomerDto> _response = new List<CustomerDto>();
            var _data = await this.context.Customers.ToListAsync();
            if (_data != null)
            {
                _response = this.mapper.Map<List<Customer>, List<CustomerDto>>(_data);
            }
            return _response;
        }

        public async Task<CustomerDto> Getbycode(string code)
        {
            CustomerDto _response = new CustomerDto();
            var _data = await this.context.Customers.FindAsync(code);
            if (_data != null)
            {
                _response = this.mapper.Map<Customer, CustomerDto>(_data);
            }
            return _response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await this.context.Customers.FindAsync(code);
                if (_customer != null)
                {
                    this.context.Customers.Remove(_customer);
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Message = "Data not found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<APIResponse> Update(CustomerDto data, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                var _customer = await this.context.Customers.FindAsync(code);
                if (_customer != null)
                {
                    _customer.Name = data.Name;
                    _customer.Email = data.Email;
                    _customer.Phone = data.Phone;
                    _customer.IsActive = data.IsActive;
                    _customer.Creditlimit = data.Creditlimit;
                    await this.context.SaveChangesAsync();
                    response.ResponseCode = 200;
                    response.Result = "pass";
                }
                else
                {
                    response.ResponseCode = 404;
                    response.Message = "Data not found";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
