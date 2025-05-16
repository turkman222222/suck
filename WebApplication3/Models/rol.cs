namespace WebApplication3.Models
{
    public class rol
    {
        public int id { get; set; }
        public string rol_name { get; set; }

        public ICollection<user> user { get; set; }
    }
}
