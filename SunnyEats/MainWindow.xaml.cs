using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;
using SunnyEats.Favourites;
using SunnyEats.Favourites.FavouriteManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCustomMessageBox;

namespace SunnyEats
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            dbContext = new MenuDBContext();

            // Set the sort
            sortDesc = false;
            sortSelection = SORT_NAME;

            // Populate recipes list view
            ListViewRecipes_Update();
            ListViewRecipes.SelectedIndex = 0;
            

            // Set selectedFavourite to binary
            favouriteSelection = FAVOURITE_BINARY;
        }
        private MenuDBContext dbContext;
        private ObservableCollection<Recipe> recipes;

        // Favourite variables
        private const string FAVOURITE_PATH = "FavouriteRecipes";
        private const int FAVOURITE_BINARY = 0;
        private const int FAVOURITE_JSON = 1;
        private const int FAVOURITE_XML = 2;
        private int favouriteSelection;

        // Sorting Variables
        private const int SORT_FAVOURITE = 0;
        private const int SORT_NAME = 1;
        private const int SORT_CATEGORY = 2;
        private const int SORT_PREP_TIME = 3;
        private const int SORT_SERVES = 4;
        private const int SORT_CAL_KJ_SERVE = 5;

        // Static Variables
        private const string SYMBOL_FAVOURITE = "\u2605";

        private bool sortDesc;
        private int sortSelection;

        /// <summary>
        /// Open a new window for creating a Recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var recipeWindow = new RecipeWindow();
            recipeWindow.Owner = this;
            recipeWindow.ShowDialog();
        }

        /// <summary>
        /// Open a window and send a recipe selected from ListViewRecipes
        /// for modification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            // Save current favourites to avoid losing data
            SaveFavourites();

            // Open new recipe window
            var recipeWindow = new RecipeWindow((Recipe)ListViewRecipes.SelectedItem);
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
        /// Prompt the user if they want to delete a recipe, and if so delete the recipe from dbContext and the list
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
                    dbContext.Recipes.Remove(selectedRecipe);
                    dbContext.SaveChanges();
                    recipes.Remove(selectedRecipe);
                    ListViewRecipes_Update(false);
                    return;
                }
            }

            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        /// <summary>
        /// Toggle the isFavourite variable of all selected variables.
        /// If one item is selected as the favourite then the rest will be selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFavourite_Click(object sender, RoutedEventArgs e)
        {
            // Check that there are items selected
            if (ListViewRecipes.Items.Count > 0)
            {
                List<Recipe> selectedRecipes = new List<Recipe>(ListViewRecipes.SelectedItems.OfType<Recipe>().ToList());
                // If there is atleast one of the selected recipes that isn't a favourite, make all the selected items favourites
                if (selectedRecipes.Where(Rec => Rec.IsFavourite != SYMBOL_FAVOURITE).Count() > 0)
                {
                    foreach (var recipe in selectedRecipes)
                    {
                        recipe.IsFavourite = "true";
                    }
                }
                // Otherwise set all the selected items to not favourites
                else
                {
                    foreach (var recipe in selectedRecipes)
                    {
                        recipe.IsFavourite = "false";
                    }
                }

                ListViewRecipes_Update(false);
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
        /// Should a GridColumn be selected, sort the ListViewRecipes by which column was selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string command = column.Content.ToString();

            switch(command)
            {
                case SYMBOL_FAVOURITE:
                    // Determine whether to sort by Ascending or Descending
                    // Is the column active, if not set it active then sort descending.
                    // Otherwise sort ascending
                    sortDesc = sortSelection == SORT_FAVOURITE ? !sortDesc : false;
                    sortSelection = SORT_FAVOURITE;
                    break;
                case "Name":
                    sortDesc = sortSelection == SORT_NAME ? !sortDesc : false;
                    sortSelection = SORT_NAME;
                    break;
                case "Category":
                    sortDesc = sortSelection == SORT_CATEGORY ? !sortDesc : false;
                    sortSelection = SORT_CATEGORY;
                    break;
                case "Prep Time":
                    sortDesc = sortSelection == SORT_PREP_TIME ? !sortDesc : false;
                    sortSelection = SORT_PREP_TIME;
                    break;
                case "Number of Serves":
                    sortDesc = sortSelection == SORT_SERVES ? !sortDesc : false;
                    sortSelection = SORT_SERVES;
                    break;
                case "Cal kJ per serve":
                    sortDesc = sortSelection == SORT_CAL_KJ_SERVE ? !sortDesc : false;
                    sortSelection = SORT_CAL_KJ_SERVE;
                    break;
                default:
                    sortDesc = false;
                    sortSelection = SORT_NAME;
                    break;
            }

            // Update the ListViewRecipes with the new sort
            ListViewRecipes_Update(false);
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
                // reload database data
                dbContext = new MenuDBContext();

                // Reset recipes
                recipes = null;
                recipes = new ObservableCollection<Recipe>(dbContext.Recipes.ToList());

                // Load favourites
                LoadFavourites();
            }

            ObservableCollection<Recipe> filteredRecipes = recipes;

            // If the search input is null or only contains white space then reset the ListViewRecipes to default
            if (string.IsNullOrWhiteSpace(TextBoxSearch.Text))
            {
                ListViewRecipes.ItemsSource = null;
                ListViewRecipes.ItemsSource = recipes;
            }
            // Otherwise remove any beginning white space, and ending white space then filter the list
            else
            {
                string searchText = TextBoxSearch.Text;

                // Format searchText for searching
                searchText = searchText.Trim();
                searchText = searchText.ToUpper();

                // Create a new list using searchText as the filter
                filteredRecipes = new ObservableCollection<Recipe>(
                    recipes.Where(Rec =>
                        (!string.IsNullOrWhiteSpace(Rec.Name) && Rec.Name.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(Rec.NumberOfServes) && Rec.NumberOfServes.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(Rec.PrepTime) && Rec.PrepTime.ToUpper().Contains(searchText))
                     || (!string.IsNullOrWhiteSpace(Rec.Cal_kJ_PerServe) && Rec.Cal_kJ_PerServe.ToUpper().Contains(searchText))
                     || (Rec.Category != null && Rec.Category.Name.ToUpper().Contains(searchText))));
            }

            // Sort the final listview based on sortSelection's value
            switch(sortSelection)
            {
                case SORT_FAVOURITE:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.IsFavourite))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.IsFavourite));
                    break;
                case SORT_NAME:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.Name))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.Name));
                    break;
                case SORT_CATEGORY:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.Category != null
                                ? Rec.Category.Name : "").DefaultIfEmpty())
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.Category != null
                                ? Rec.Category.Name : "").DefaultIfEmpty());
                    break;
                case SORT_PREP_TIME:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.PrepTime))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.PrepTime));
                    break;
                case SORT_SERVES:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.NumberOfServes))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.NumberOfServes));
                    break;
                case SORT_CAL_KJ_SERVE:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.Cal_kJ_PerServe))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.Cal_kJ_PerServe));
                    break;
                default:
                    filteredRecipes = sortDesc
                        ? new ObservableCollection<Recipe>(
                            filteredRecipes.OrderByDescending(Rec => Rec.Name))
                        : new ObservableCollection<Recipe>(
                            filteredRecipes.OrderBy(Rec => Rec.Name));
                    break;
            }

            // Update the ListView with sorted and filtered recipes
            ListViewRecipes.ItemsSource = filteredRecipes;
        }

        /// <summary>
        /// Open a Category window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCategory_Click(object sender, RoutedEventArgs e)
        {
            // Save favourites to avoid losing changes
            SaveFavourites();

            // Open a new category window
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

        /// <summary>
        /// Prompt the user and ask what file type they would like to write to.
        /// Output a new file based on their response (Binary, XML, or JSON).
        /// Then update the ListView, as it will now have favourites
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFavouriteConfig_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.ShowYesNoCancel(
                "Which type of file do you want to manage your favourites file as?",
                "Favourite file type",
                "Binary",
                "JSON",
                "XML");
            switch (result)
            {
                case MessageBoxResult.Yes:
                    favouriteSelection = FAVOURITE_BINARY;
                    break;
                case MessageBoxResult.No:
                    favouriteSelection = FAVOURITE_JSON;
                    break;
                case MessageBoxResult.Cancel:
                    favouriteSelection = FAVOURITE_XML;
                    break;
                default:
                    favouriteSelection = FAVOURITE_BINARY;
                    break;
            }
        }

        /// <summary>
        /// Run the SaveFavourites() method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFavouriteLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadFavourites();
        }

        /// <summary>
        /// Load favourites from file and update ListViewRecipes
        /// </summary>
        private void LoadFavourites()
        {
            var manager = GetFavouriteManager();

            // Read favourites from file
            try
            {
                Favourite favourite = manager.ReadFile(FAVOURITE_PATH);

                foreach (var favID in favourite.IDS)
                {
                    foreach (var recipe in recipes.Where(Rec => Rec.ID == favID))
                    {
                        recipe.IsFavourite = "true";
                    }
                }

                ListViewRecipes_Update(false);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Run the SaveFavourites() command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemFavouriteSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFavourites();
        }

        /// <summary>
        /// Save currently selected favourites to file
        /// </summary>
        private void SaveFavourites()
        {
            var manager = GetFavouriteManager();

            // Write favourites to file
            try
            {
                List<int> recipeIDs = new List<int>();
                foreach (var listItem in recipes.Where(Rec => Rec.IsFavourite == SYMBOL_FAVOURITE))
                {
                    Recipe recipe = listItem as Recipe;
                    recipeIDs.Add(recipe.ID);
                }

                Favourite favourite = new Favourite(recipeIDs.ToArray());
                manager.WriteFile(FAVOURITE_PATH, favourite);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Choose the Favourite Manager based on the favouriteSelection
        /// </summary>
        /// <returns></returns>
        private IFavouriteManager GetFavouriteManager()
        {
            IFavouriteManager manager = null;
            switch (favouriteSelection)
            {
                case FAVOURITE_BINARY:
                    manager = new BinaryFileFavouriteManager();
                    break;
                case FAVOURITE_JSON:
                    manager = new JSONFileFavouriteManager();
                    break;
                case FAVOURITE_XML:
                    manager = new XMLFileFavouriteManager();
                    break;
                default:
                    manager = new BinaryFileFavouriteManager();
                    break;
            }
            return manager;
        }

        /// <summary>
        /// When Exit is clicked from the file menu this will close the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
