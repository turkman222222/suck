namespace WebApplication3.Models
{
    public class compl
    {
        public int id { get; set; }
        public string kompl_name { get; set; }

        public ICollection<Carss> Carss { get; set; }
    }
}
