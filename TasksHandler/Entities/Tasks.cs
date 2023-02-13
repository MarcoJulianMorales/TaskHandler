using System.ComponentModel.DataAnnotations;

namespace TasksHandler.Entities
{
    public class Tasks
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Tittle { get; set; }
        public string Description { get; set; }
        public int Orden { get; set; }
        public DateTime Creationdate { get; set; }
        public List<Steps> Steps { get; set; }
        public List<AttachedFile> AttachedFiles { get; set; }

    }
}
