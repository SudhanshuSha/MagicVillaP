using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebAPI.Models
{
    public class VillaNumber
    {
        // not an identity column
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; }
        // we need to give the name of the foreign key mapper
        [ForeignKey("Villa")]
        public int VillaId { get; set; }

        //navigation property

        public Villa Villa { get; set; }
    }
}
