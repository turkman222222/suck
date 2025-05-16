namespace WebApplication3.Models
{
    public class salonch
    {
        public int id { get; set; }
        public string salon { get; set; }

        public ICollection<Carss> Carss { get; set; }
    }
}
