namespace BasicManagementBoard.Models
{
    public class ProjectItem
    {
        public long Id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public int progress { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
    }
}
