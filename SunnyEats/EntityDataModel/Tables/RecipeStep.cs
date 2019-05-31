namespace SunnyEats.EntityDataModel.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RecipeStep")]
    public partial class RecipeStep : INotifyPropertyChanged
    {
        private int recipeID;
        private int number;
        private string description;

        // Declare INotifyPropertyChanged event handler
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get;set; }

        public int RecipeID
        {
            get => recipeID;
            set
            {
                recipeID = value;

                // Call OnPropertyChanged whenver recipeID is updated
                OnPropertyChanged("RecipeID");
            }
        }

        public int Number
        {
            get => number;
            set
            {
                number = value;

                // Call OnPropertyChanged whenever number is updated
                OnPropertyChanged("Number");
            }
        }

        [Required]
        [StringLength(1000)]
        public string Description
        {
            get => description;
            set
            {
                description = value;

                // Call OnPropertyChanged whenever number is updated
                OnPropertyChanged("Description");
            }
        }

        public virtual Recipe Recipe { get; set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
