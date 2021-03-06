﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Key_onboarding.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage ="Name cannot be longer than 50 characters.")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        public virtual List<ProductSold> ProductSolds { get; set; }
    }
}