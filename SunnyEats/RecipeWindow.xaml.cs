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
            foreach (Category category in dbContext.Categories)
            {
                categories.Add(category);
            }

            Recipe recipe = dbContext.Recipes.FirstOrDefault();
            steps = new ObservableCollection<RecipeStep>();
            foreach (RecipeStep step in recipe.RecipeSteps)
            {
                steps.Add(step);
            }

            cmbxCategory.ItemsSource = categories;
            ListViewSteps.ItemsSource = steps;
        }

        private MenuDBContext dbContext;
        ObservableCollection<Category> categories;
        ObservableCollection<RecipeStep> steps;
    }
}
