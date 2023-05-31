﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebAPI.Models.Dto
{
    public class VillaNumberDTO
    {
        public int VillaNo { get; set; }
        public string SpecialDetails { get; set; }
        [Required]
        public int VillaId { get; set; }
    }
}
