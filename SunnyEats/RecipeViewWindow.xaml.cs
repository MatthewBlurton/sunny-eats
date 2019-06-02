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

using SunnyEats.EntityDataModel.Tables;

namespace SunnyEats
{
    /// <summary>
    /// Interaction logic for RecipeViewWindow.xaml
    /// </summary>
    public partial class RecipeViewWindow : Window
    {
        public RecipeViewWindow(Recipe recipe)
        {
            InitializeComponent();

            this.recipe = recipe;
            ingredients = new ObservableCollection<Ingredient>();
            steps = new ObservableCollection<RecipeStep>();
        }

        private Recipe recipe;
        private ObservableCollection<Ingredient> ingredients;
        private ObservableCollection<RecipeStep> steps;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                ingredients.Add(ingredient);
            }
            foreach (var step in recipe.RecipeSteps)
            {
                steps.Add(step);
            }
            steps = new ObservableCollection<RecipeStep>(steps.OrderBy(Step => Step.Number));
            ListViewIngredients.ItemsSource = ingredients;
            ListViewSteps.ItemsSource = steps;
        }
    }
}
