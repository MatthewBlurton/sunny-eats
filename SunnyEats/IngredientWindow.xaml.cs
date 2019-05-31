using SunnyEats.EntityDataModel.Tables;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for IngredientWindow.xaml
    /// </summary>
    public partial class IngredientWindow : Window
    {
        public IngredientWindow(Recipe recipe)
        {
            InitializeComponent();

            this.recipe = recipe;
            this.ingredient = new Ingredient();
        }

        public IngredientWindow(Recipe recipe, Ingredient ingredient) : this(recipe)
        {
            this.recipe = recipe;
            this.ingredient = ingredient;

            this.DataContext = this.ingredient;

            this.btnSubmit.Content = "Overwrite";
        }

        public readonly Recipe recipe;

        private Ingredient ingredient;

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Owner as RecipeViewWindow;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
