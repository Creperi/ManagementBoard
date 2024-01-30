using System.ComponentModel.DataAnnotations;

namespace BasicManagementBoard.Models
{
    public class TaskItem
    {
        public long Id { get; set; }
        public long projectId { get; set; }
        public string? description { get; set; }
        public int progress { get; set; }
        public string status { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
    }
}
