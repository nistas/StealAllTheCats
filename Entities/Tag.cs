using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StealAllTheCats.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //[DataType(DataType.DateTime)]
        public string Created { get; set; }
        //public List<Cat> Cats { get; set; } = [];
    }
}
