using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SunnyEats
{
    /// <summary>
    /// Interaction logic for RecipeWindow.xaml
    /// </summary>
    public partial class RecipeWindow : Window
    {
        public RecipeWindow()
        {
            InitializeComponent();

            dbContext = new MenuDBContext();

            categories = new ObservableCollection<Category>();
            steps = new ObservableCollection<RecipeStep>();

            cmbxCategory.ItemsSource = categories;
            ListViewSteps.ItemsSource = steps;
        }

        public RecipeWindow(Recipe recipe)
        {
            
        }

        private MenuDBContext dbContext;
        ObservableCollection<Category> categories;
        ObservableCollection<RecipeStep> steps;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // if a recipe is included, populate the steps with the recipe's steps
            if (this.Recipe != null)
            {
                foreach (RecipeStep step in this.Recipe.RecipeSteps)
                {
                    steps.Add(step);
                }
            }

            // Load all the categories
            foreach (Category category in dbContext.Categories)
            {
                this.categories.Add(category);
            }
        }

        private Recipe recipe;
        public Recipe Recipe
        {
            get
            {
                return this.recipe;
            }
            set
            {
                this.recipe = value;
            }
        }
    }
}
