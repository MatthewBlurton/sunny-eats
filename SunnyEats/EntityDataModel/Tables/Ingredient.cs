namespace SunnyEats.EntityDataModel.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ingredient")]
    public partial class Ingredient
    {
        public int ID { get; set; }

        public int RecipeID { get; set; }

        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        [StringLength(25)]
        public string Quantity { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
