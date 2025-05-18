using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    public class izbr
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("Carss")]
        public int car_id { get; set; }

        [ForeignKey("user")]
        public int user_id { get; set; }

        public Carss Carss { get; set; }
        public user user { get; set; }
    }
}