namespace SunnyEats.EntityDataModel.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RecipeIngredient")]
    public partial class RecipeIngredient
    {
        public int ID { get; set; }

        public int RecipeID { get; set; }

        public int IngredientID { get; set; }

        [Required]
        [StringLength(100)]
        public string Quantity { get; set; }

        [StringLength(50)]
        public string IngredientAppend { get; set; }

        public virtual Ingredient Ingredient { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
