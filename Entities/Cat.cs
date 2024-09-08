using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StealAllTheCats.Entities
{
    public class Cat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CaaSCatId { get; set; }
     
        public int Width { get; set; }
   
        public int Height { get; set; }

  
        public string Image { get; set; }

        //[DataType(DataType.DateTime)]
        public string Created { get; set; }
        [NotMapped]
        public List<string> Tags { get; set; } = [];
        //public virtual Tag Tags { get; set; }
    }
}
