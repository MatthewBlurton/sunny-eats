namespace SunnyEats.EntityDataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    using SunnyEats.EntityDataModel.Tables;

    public partial class MenuDBContext : DbContext
    {
        public MenuDBContext()
            : base("name=MenuDBContext")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeStep> RecipeSteps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.Ingredients)
                .WithRequired(e => e.Recipe)
                .WillCascadeOnDelete(false);
        }
    }
}
