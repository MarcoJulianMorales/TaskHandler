namespace TasksHandler.Entities
{
    public class Steps
    {
        public Guid Id { get; set; }
        public int TasksId { get; set; }
        public Tasks task { get; set; }
        public string Description{ get; set;}
        public bool Done{ get; set;}
        public int Orden{ get; set;}
    }
}
