namespace SunnyEats.EntityDataModel.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Recipe")]
    public partial class Recipe : INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recipe()
        {
            Ingredients = new HashSet<Ingredient>();
            RecipeSteps = new HashSet<RecipeStep>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Private variables
        private string name;
        private string description;
        private string prepTime;
        private Category category;
        private string numServes;
        private string calKjPerServe;

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        [StringLength(255)]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        [StringLength(50)]
        public string PrepTime
        {
            get
            {
                return prepTime;
            }
            set
            {
                prepTime = value;
                OnPropertyChanged("PrepTime");
            }
        }

        public int? CategoryID { get; set; }

        [StringLength(50)]
        public string NumberOfServes
        {
            get
            {
                return numServes;
            }
            set
            {
                numServes = value;
                OnPropertyChanged("NumberOfServes");
            }
        }

        [StringLength(50)]
        public string Cal_kJ_PerServe
        {
            get
            {
                return calKjPerServe;
            }
            set
            {
                calKjPerServe = value;
                OnPropertyChanged("Cal_kJ_PerServe");
            }
        }

        public virtual Category Category
        {
            get
            {
                return category;
            }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecipeStep> RecipeSteps { get; set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
