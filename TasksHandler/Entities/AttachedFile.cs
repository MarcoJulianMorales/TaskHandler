using Microsoft.EntityFrameworkCore;

namespace TasksHandler.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public Tasks Task { get; set; }
        [Unicode]
        public string Url { get; set; }
        public string Tittle { get; set; }
        public int Orden { get; set; } 
        public DateTime CreationDate { get; set; }
    }
}
