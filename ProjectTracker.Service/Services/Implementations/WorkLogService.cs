using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WorkLogService(
            IRepository<WorkLog> workLogRepository,
            AppDbContext context,
            IMapper mapper)
        {
            _workLogRepository = workLogRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync()
        {
            var workLogs = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Where(w => w.IsActive)
                .OrderByDescending(w => w.WorkDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        public async Task<WorkLogDto?> GetWorkLogByIdAsync(int id)
        {
            var workLog = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Include(w => w.Details)
                .Include(w => w.Attachments)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsActive);

            if (workLog == null) return null;

            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<WorkLogDto> CreateWorkLogAsync(WorkLogDto workLogDto)
        {
            var workLog = _mapper.Map<WorkLog>(workLogDto);
            workLog.CreatedDate = DateTime.Now;
            workLog.IsActive = true;

            await _workLogRepository.AddAsync(workLog);
            await _workLogRepository.SaveAsync();

            // Yeni oluşturulan kaydı ilişkileriyle birlikte geri getir
            var createdWorkLog = await GetWorkLogByIdAsync(workLog.Id);
            return createdWorkLog ?? workLogDto;
        }

        public async Task<WorkLogDto?> UpdateWorkLogAsync(int id, WorkLogDto workLogDto)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null) return null;

            // ID'yi koruyarak güncelle
            var currentId = workLog.Id;
            _mapper.Map(workLogDto, workLog);
            workLog.Id = currentId;
            workLog.UpdatedDate = DateTime.Now;

            _workLogRepository.Update(workLog);
            await _workLogRepository.SaveAsync();

            return await GetWorkLogByIdAsync(id);
        }

        public async Task<bool> DeleteWorkLogAsync(int id)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null) return false;

            // Soft delete
            workLog.IsActive = false;
            workLog.UpdatedDate = DateTime.Now;

            _workLogRepository.Update(workLog);
            await _workLogRepository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int count)
        {
            var workLogs = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Where(w => w.IsActive)
                .OrderByDescending(w => w.WorkDate)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        public async Task<IQueryable<WorkLogDto>> GetAllWorkLogsQueryableAsync()
        {
            var workLogs = _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Where(w => w.IsActive)
                .Select(w => new WorkLogDto
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    WorkDate = w.WorkDate,
                    HoursSpent = w.HoursSpent,
                    ProjectId = w.ProjectId,
                    ProjectName = w.Project.Name,
                    EmployeeId = w.EmployeeId,
                    EmployeeName = w.Employee.FirstName + " " + w.Employee.LastName
                });

            return await Task.FromResult(workLogs);
        }
    }
}