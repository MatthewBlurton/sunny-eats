namespace SunnyEats.EntityDataModel.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RecipeStep")]
    public partial class RecipeStep
    {
        public int ID { get; set; }

        public int RecipeID { get; set; }

        public int Number { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
