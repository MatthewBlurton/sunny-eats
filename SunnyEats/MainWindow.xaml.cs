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
using SunnyEats.Favourites;
using SunnyEats.Favourites.FavouriteManager;

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

            dBContext = new MenuDBContext();
            recipes = new ObservableCollection<Recipe>();
            foreach (var item in dBContext.Recipes)
            {
                recipes.Add(item);
            }
            ListViewRecipes.ItemsSource = recipes;
            ListViewRecipes.SelectedIndex = 1;
        }

        private RecipeWindow recipeWindow;

        private MenuDBContext dBContext;

        private ObservableCollection<Recipe> recipes;

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            recipeWindow = new RecipeWindow();
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            recipeWindow = new RecipeWindow((Recipe) ListViewRecipes.SelectedItem);
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        // Ensure that the size of each columns for the Recipe view remains above or equal to 54 pixels
        private void GridViewColumnHeaderRecipe_SizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            sizeChangedEventArgs.Handled = true;
            var column = (GridViewColumnHeader)sender;

            column.Column.Width = LimitWidth(54, column.Column.Width);
        }

        private double LimitWidth(double minWidth, double curWidth)
        {
            return curWidth > minWidth ? curWidth : minWidth;
        }

        // Delete the currently selected recipe from RecipesListView
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)this.ListViewRecipes.SelectedItem ?? null;

            // Prepare message box paramaters for failure incase selectedRecipe is null
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
                    dBContext.Recipes.Remove(selectedRecipe);
                    dBContext.SaveChanges();

                    recipes.Remove(selectedRecipe);
                    return;
                }
            }

            MessageBox.Show(messageBoxText, caption, button, icon);
        }
        private void ButtonFavourite_Click(object sender, RoutedEventArgs e)
        {
            IFavouriteManager manager = new BinaryFileFavouriteManager();

            // Write favourites to file
            try
            {
                List<int> recipeIDs = new List<int>();
                foreach (var listItem in ListViewRecipes.SelectedItems)
                {
                    Recipe recipe = listItem as Recipe;
                    recipeIDs.Add(recipe.ID);
                }

                Favourite favourite = new Favourite(recipeIDs.ToArray());
                manager.WriteFile("New Favourite", favourite);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            // Read favourites from file
            try
            {
                Favourite favourite = manager.ReadFile("New Favourite");

                string message = "";

                foreach (var id in favourite.ID)
                {
                    message += id + "\r\n";
                }

                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // When an item is clicked twice from the RecipeListView, open a Window specifically for viewing the recipe
        private void RecipesListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem) sender;
            var window = new RecipeViewWindow((Recipe) item.DataContext);
            window.Owner = this;
            window.Show();
        }

        public void RecipesListView_Update()
        {
            recipes = new ObservableCollection<Recipe>();
            foreach (var recipe in dBContext.Recipes)
            {
                recipes.Add(recipe);
            }
            ListViewRecipes.ItemsSource = recipes;
        }
    }

}
