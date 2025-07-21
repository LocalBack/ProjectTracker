using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IRepository<Employee> employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return _mapper.Map<EmployeeDto>(employee);
        }

        // Add this method
        public async Task<EmployeeDto> GetEmployeeByUserIdAsync(int userId)
        {
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            await _employeeRepository.AddAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeDto employeeDto)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return null;

            _mapper.Map(employeeDto, employee);
            await _employeeRepository.UpdateAsync(employee);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return false;

            await _employeeRepository.DeleteAsync(employee);
            return true;
        }

        // Add this method
        public async Task<bool> EmployeeExistsAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee != null;
        }
    }
}