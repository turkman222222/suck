namespace WebApplication3.Models
{
    public class Marks
    {
        public int id { get; set; }
        public string name_marka { get; set; }

        public ICollection<Carss> Carss { get; set; }
    }
}
