using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class user
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_user { get; set; }

        public string user_name { get; set; }
        public string mail { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int? rol_id { get; set; }

        public rol rol { get; set; }
        public ICollection<bron> bron { get; set; }
        public ICollection<izbr> izbr { get; set; }
    }
}
