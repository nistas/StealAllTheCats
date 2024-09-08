using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StealAllTheCats.Entities
{
    public class CatTag
    {
        
        [ForeignKey(nameof(Cat))]
        public int CatId { get; set; }
        public Cat Cat { get; set; }
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
