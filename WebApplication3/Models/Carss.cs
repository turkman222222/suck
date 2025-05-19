using System.Numerics;

namespace WebApplication3.Models
{
    public class Carss
    {
        public int id { get; set; }
        public string model { get; set; }
        public int id_marki { get; set; }
        public int id_str { get; set; }
        public int god_poiz { get; set; }
        public int id_cvet { get; set; }
        public int id_salona { get; set; }
        public int id_kompl { get; set; }
        public byte[]? image { get; set; }
        public int? price { get; set; }

        public Marks Marks { get; set; }
        public strana strana { get; set; }
        public cveta cveta { get; set; }
        public salonch salonch { get; set; }
        public compl compl { get; set; }

        public ICollection<bron> bron { get; set; }
        public ICollection<izbr> izbr { get; set; }
    }
}
