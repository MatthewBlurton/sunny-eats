using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;

namespace SunnyEats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.dBContext = new MenuDBContext();

            foreach (var item in this.dBContext.Recipes)
            {
                Recipes.Add(item);
            }
            this.ListViewRecipes.ItemsSource = recipes;
            this.ListViewRecipes.SelectedIndex = 1;
        }

        private RecipeWindow recipeWindow;

        private MenuDBContext dBContext;

        private ObservableCollection<Recipe> recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get
            {
                if (recipes == null)
                    recipes = new ObservableCollection<Recipe>();
                return recipes;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.recipeWindow = new RecipeWindow();
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            this.recipeWindow = new RecipeWindow((Recipe) ListViewRecipes.SelectedItem);
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        // Ensure that the size of each columns for the Recipe view remains above or equal to 54 pixels
        private void RecipeGridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            sizeChangedEventArgs.Handled = true;
            var column = (GridViewColumnHeader)sender;

            column.Column.Width = LimitWidth(54, column.Column.Width);
        }

        private double LimitWidth(double minWidth, double curWidth)
        {
            return curWidth > minWidth ? curWidth : minWidth;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)this.ListViewRecipes.SelectedItem ?? null;

            string messageBoxText = "Could not delete recipe. There are currently no recipes selected";
            string caption = "Error deleting recipe";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;

            // Only proceed with removing the recipe if a recipe is selected
            if (selectedRecipe != null)
            {
                // Ensure that this is the action the user wants to take
                messageBoxText = "Are you sure you want to delete " + selectedRecipe.Name + "\r\nThis action cannot be undone.";
                caption = "Delete " + selectedRecipe.Name;
                button = MessageBoxButton.YesNo;
                icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.No);

                // If the user answers yes delete the recipe both from the ObservableCollection and the Database then exit the method
                if (result == MessageBoxResult.Yes)
                {
                    this.dBContext.Recipes.Remove(selectedRecipe);
                    this.dBContext.SaveChanges();

                    this.Recipes.Remove(selectedRecipe);
                    return;
                }
            }

            MessageBox.Show(messageBoxText, caption, button, icon);
        }
    }


}
