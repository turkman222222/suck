namespace WebApplication3.Models
{
    public class cveta
    {
        public int id { get; set; }
        public string cvet_name { get; set; }

        public ICollection<Carss> Carss { get; set; }
    }
}
