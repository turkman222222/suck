namespace WebApplication3.Models
{
    public class izbr
    {
        public int id { get; set; }
        public int car_id { get; set; }
        public int user_id { get; set; }

        public Carss Carss { get; set; }
        public user user { get; set; }
    }
}
