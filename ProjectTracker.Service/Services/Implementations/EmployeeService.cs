using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Core.Events;
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
        private readonly IMediator _mediator;

        public EmployeeService(IRepository<Employee> employeeRepository, IMapper mapper, IMediator mediator)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _mediator = mediator;
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
            await _mediator.Publish(new EmployeeUpdatedEvent(employee));
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

        public async Task<EmployeeDto> GetEmployeeByEmailAsync(string email)
        {
            // 1) IQueryable üzerinden Include / ThenInclude ile projeleri de yükle
            var employee = await _employeeRepository
                .GetQueryable()
                .Include(e => e.ProjectEmployees)
                    .ThenInclude(pe => pe.Project)
                .FirstOrDefaultAsync(e => e.Email == email);

            if (employee == null)
                return null;

            // 2) DTO'ya map et
            var dto = _mapper.Map<EmployeeDto>(employee);

            // 3) EmployeeDto.Projects koleksiyonunu elle doldur (MappingProfile'ın henüz projeleri map etmediği durumda)
            dto.Projects = employee.ProjectEmployees
                .Select(pe => _mapper.Map<ProjectDto>(pe.Project))
                .ToList();

            return dto;
        }
    }
}