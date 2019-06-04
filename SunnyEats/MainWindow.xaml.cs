using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;
using SunnyEats.Favourites;
using SunnyEats.Favourites.FavouriteManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            // Populate recipes list view
            recipes = new ObservableCollection<Recipe>();
            foreach (var item in dBContext.Recipes)
            {
                recipes.Add(item);
            }
            ListViewRecipes.ItemsSource = recipes;
            ListViewRecipes.SelectedIndex = 0;
        }

        private RecipeWindow recipeWindow;

        private MenuDBContext dBContext;

        private ObservableCollection<Recipe> recipes;

        /// <summary>
        /// Open a new window for creating a Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            recipeWindow = new RecipeWindow();
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        /// <summary>
        /// Open a window for modifying an already existing Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            recipeWindow = new RecipeWindow((Recipe)ListViewRecipes.SelectedItem);
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        /// <summary>
        /// Ensure that the size of each columns for the Recipe view remains above or equal to 54 pixels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sizeChangedEventArgs"></param>
        private void GridViewColumnHeaderRecipe_SizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            sizeChangedEventArgs.Handled = true;
            var column = (GridViewColumnHeader)sender;

            column.Column.Width = LimitWidth(54, column.Column.Width);
        }
        /// <summary>
        /// Return a width based on whether new width is below minWidth or not
        /// </summary>
        /// <param name="minWidth">Lowest allowed width</param>
        /// <param name="curWidth">The width being updated</param>
        /// <returns></returns>
        private double LimitWidth(double minWidth, double curWidth)
        {
            return curWidth > minWidth ? curWidth : minWidth;
        }

        /// <summary>
        /// Prompt the user if they want to delete a recipe, and if so delete the recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Prompt the user and ask what file type they would like to write to.
        /// Output a new file based on their response (Binary, XML, or JSON).
        /// Then update the ListView, as it will now have favourites
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Open a window that shows all the details of the selected recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecipesListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            var window = new RecipeViewWindow((Recipe)item.DataContext);
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Used to update the ListView for Recipes.
        /// The ListView is filtered based on the searchText
        /// </summary>
        /// <param name="isNew">Determines whether to completely refresh the default ObservableCollection or not</param>
        public void ListViewRecipes_Update(bool isNew = true)
        {
            if (isNew)
            {
                recipes = new ObservableCollection<Recipe>();
                foreach (var recipe in dBContext.Recipes)
                {
                    recipes.Add(recipe);
                }
                ListViewRecipes.ItemsSource = recipes;
            }


            // If the search input is null or only contains white space then reset the ListViewRecipes to default
            if (string.IsNullOrWhiteSpace(TextBoxSearch.Text))
            {
                ListViewRecipes.ItemsSource = recipes;
            }
            // Otherwise remove any beginning white space, and ending white space then filter the list
            else
            {
                string searchText = TextBoxSearch.Text;

                // Format searchText for searching
                searchText = searchText.Trim();
                searchText = searchText.ToUpper();

                // Create a new list using searchText as the filter then apply the new list to ListViewRecipes.ItemsSource
                ObservableCollection<Recipe> filteredRecipes = new ObservableCollection<Recipe>(
                    recipes.Where(recipe =>
                        (!string.IsNullOrWhiteSpace(recipe.Name) && recipe.Name.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(recipe.NumberOfServes) && recipe.NumberOfServes.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(recipe.PrepTime) && recipe.PrepTime.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(recipe.Cal_kJ_PerServe) && recipe.Cal_kJ_PerServe.ToUpper().Contains(searchText))
                     || (recipe.Category != null && recipe.Category.Name.ToUpper().Contains(searchText))));
                ListViewRecipes.ItemsSource = filteredRecipes;
            }
        }

        /// <summary>
        /// Open a Category window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow window = new CategoryWindow();
            window.Owner = this;
            window.Show();
            window.Show();
        }

        /// <summary>
        /// When text is changed in the search box, update the ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListViewRecipes_Update(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Ensures that the application properly shuts down
            Environment.Exit(0);
        }
    }
}
