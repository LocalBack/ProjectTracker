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
        private readonly IRepository<WorkLogDetail> _detailRepository;
        private readonly IRepository<WorkLogAttachment> _attachmentRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public WorkLogService(
            IRepository<WorkLog> workLogRepository,
            IRepository<WorkLogDetail> detailRepository,
            IRepository<WorkLogAttachment> attachmentRepository,
            AppDbContext context,
            IMapper mapper)
        {
            _workLogRepository = workLogRepository;
            _detailRepository = detailRepository;
            _attachmentRepository = attachmentRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync()
        {
            var workLogs = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Include(w => w.Details)
                .Include(w => w.Attachments)
                .Where(w => w.IsActive)
                .OrderByDescending(w => w.WorkDate)
                .ToListAsync();

            var workLogDtos = _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);

            foreach (var dto in workLogDtos)
            {
                var workLog = workLogs.First(w => w.Id == dto.Id);
                dto.DetailCount = workLog.Details?.Count ?? 0;
                dto.AttachmentCount = workLog.Attachments?.Count ?? 0;
            }

            return workLogDtos;
        }

        public async Task<IEnumerable<WorkLogDto>> GetWorkLogsByProjectIdAsync(int projectId)
        {
            var workLogs = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Include(w => w.Details)
                .Include(w => w.Attachments)
                .Where(w => w.ProjectId == projectId && w.IsActive)
                .OrderByDescending(w => w.WorkDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        public async Task<IEnumerable<WorkLogDto>> GetWorkLogsByEmployeeIdAsync(int employeeId)
        {
            var workLogs = await _context.WorkLogs
                .Include(w => w.Project)
                .Include(w => w.Employee)
                .Where(w => w.EmployeeId == employeeId && w.IsActive)
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
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workLog == null) return null;

            var dto = _mapper.Map<WorkLogDto>(workLog);
            dto.DetailCount = workLog.Details?.Count ?? 0;
            dto.AttachmentCount = workLog.Attachments?.Count ?? 0;

            return dto;
        }

        public async Task<IEnumerable<WorkLogDetailDto>> GetWorkLogDetailsAsync(int workLogId)
        {
            var details = await _detailRepository.FindAsync(d => d.WorkLogId == workLogId);
            return _mapper.Map<IEnumerable<WorkLogDetailDto>>(details.OrderBy(d => d.StepNumber));
        }

        public async Task<IEnumerable<WorkLogAttachmentDto>> GetWorkLogAttachmentsAsync(int workLogId)
        {
            var attachments = await _attachmentRepository.FindAsync(a => a.WorkLogId == workLogId);
            return _mapper.Map<IEnumerable<WorkLogAttachmentDto>>(attachments);
        }

        public async Task<WorkLogDto> CreateWorkLogAsync(CreateWorkLogDto createWorkLogDto)
        {
            var workLog = _mapper.Map<WorkLog>(createWorkLogDto);
            await _workLogRepository.AddAsync(workLog);
            await _workLogRepository.SaveAsync();

            return await GetWorkLogByIdAsync(workLog.Id);
        }

        public async Task UpdateWorkLogAsync(int id, WorkLogDto workLogDto)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                throw new Exception($"WorkLog with id {id} not found");

            _mapper.Map(workLogDto, workLog);
            _workLogRepository.Update(workLog);
            await _workLogRepository.SaveAsync();
        }

        public async Task DeleteWorkLogAsync(int id)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                throw new Exception($"WorkLog with id {id} not found");

            workLog.IsActive = false;
            _workLogRepository.Update(workLog);
            await _workLogRepository.SaveAsync();
        }
    }
}