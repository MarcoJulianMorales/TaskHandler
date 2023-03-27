namespace TasksHandler.Models
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DoneSteps { get; set; }
        public int TotalSteps { get; set; }
    }
}
