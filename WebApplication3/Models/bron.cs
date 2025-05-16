namespace WebApplication3.Models
{
    public class bron
    {
        public int id { get; set; }
        public int id_car { get; set; }
        public int id_usr { get; set; }

        public Carss Carss { get; set; }
        public user user { get; set; }
    }
}
