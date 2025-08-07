using System;

namespace ProjectTracker.Service.DTOs
{
    public class EquipmentActionDto
    {
        public DateTime Date { get; set; }
        public string Equipment { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
