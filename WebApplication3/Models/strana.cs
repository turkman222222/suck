namespace WebApplication3.Models
{
    public class strana
    {
        public int id { get; set; }
        public string strana_name { get; set; }

        public ICollection<Carss> Carss { get; set; }
    }
}
