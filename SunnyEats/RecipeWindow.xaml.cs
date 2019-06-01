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
            ingredients = new ObservableCollection<Ingredient>();

            cmbxCategory.ItemsSource = categories;
            ListViewSteps.ItemsSource = steps;
            listViewIngredients.ItemsSource = ingredients;
        }

        public RecipeWindow(Recipe recipe) : this()
        {
            this.recipe = recipe;
            this.DataContext = this.recipe;
        }

        private MenuDBContext dbContext;
        private Recipe recipe;
        private ObservableCollection<Category> categories;
        private ObservableCollection<RecipeStep> steps;
        private ObservableCollection<Ingredient> ingredients;

        // If an item is going to be overriden, these variables will be overriden
        private Ingredient selIngredient;
        private RecipeStep selStep;

        // Used when closing to check what condition the window is closing (is it from a submission, or just cancel/close)
        bool closeFromSave = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load all the categories
            foreach (Category category in dbContext.Categories)
            {
                this.categories.Add(category);
            }

            // check if a recipe is being updated
            if (this.recipe != null)
            {
                // pull all the recipe steps from the currently selected recipe
                foreach (RecipeStep step in this.recipe.RecipeSteps)
                {
                    this.steps.Add(step);
                }

                foreach (Ingredient ingredient in this.recipe.Ingredients)
                {
                    this.ingredients.Add(ingredient);
                }

                // Poor performance, need to figure out a LINQ query to replace the for loop
                for (int i = 0; i < cmbxCategory.Items.Count; i++)
                {
                    Category currentCat = (Category)cmbxCategory.Items[i];
                    if (currentCat.ID == this.recipe.CategoryID )
                    {
                        cmbxCategory.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // Returns true if all inputs in the window are correct
        private bool AreInputsValid()
        {
            // Variables to validate
            var name = txbxName.Text;
            var prep = txbxPrepTime.Text;
            var numS = txbxNumServes.Text;
            var ingCount = ingredients.Count;
            var stepCount = steps.Count;

            // Swapped to false when any variable is flagged as invalid
            bool valid = true;

            // Dialogue settings
            string message = "";
            string caption = "error occurred";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;

            if (name.Length <= 0)
            {
                message = "The length of the recipe name must be above 1";
                valid = false;
            }
            if (ingCount <= 0)
            {
                if (message != "") message += "\r\n";
                message += "There must be atleast 1 ingredient";
                valid = false;
            }
            if (stepCount <= 0)
            {
                if (message != "") message += "\r\n";
                message += "There must be atleast 1 step";
                valid = false;
            }
            if (!valid) MessageBox.Show(message, caption, button, icon);

            return valid;
        }

        private bool AreThereChanges()
        {
            // Initialize variables
            string name;
            string desc;
            int? catId;
            string prepTime;
            string serves;
            string calkPerServe;
            List<RecipeStep> steps = new List<RecipeStep>();

            if (this.recipe != null)
            {
                // Add data to variables
                name = this.recipe.Name;
                desc = this.recipe.Description;
                catId = this.recipe.CategoryID;
                prepTime = this.recipe.PrepTime;
                serves = this.recipe.NumberOfServes;
                calkPerServe = this.recipe.Cal_kJ_PerServe;

                Category selCategory = (Category)cmbxCategory.SelectedItem;

                if (txbxName.Text != name)                  return true;
                if (txbxDescription.Text != desc)           return true;
                if (selCategory.ID != catId)                return true;
                if (txbxPrepTime.Text != prepTime)          return true;
                if (txbxNumServes.Text != serves)           return true;
                if (txbxCalkJPerServe.Text != calkPerServe) return true;
            }
            else
            {
                if (txbxName.Text != "") return true;
                if (txbxDescription.Text != "") return true;
                if (cmbxCategory.SelectedItem != null) return true;
                if (txbxPrepTime.Text != "") return true;
                if (txbxNumServes.Text != "") return true;
                if (txbxCalkJPerServe.Text != "") return true;
            }

            return false;
        }


        /// <summary>
        /// Ask the user if they would like to close without saving, if there have been changes made
        /// </summary>
        /// <returns>Did the user say yes or no?</returns>
        private bool CloseNoSave()
        {
            string message = "You have unsaved changes, are you sure you want to close this recipe without saving?";
            string caption = "Quit without saving";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

            if (result.Equals(MessageBoxResult.No)) return false;
            return true;
        }

        /// <summary>
        /// Ask the user if they would like to overwrite the existing recipe, if the recipe isn't new
        /// </summary>
        /// <returns></returns>
        private bool UserWantsToOverwriteMessage()
        {
            if (recipe != null)
            {
                string message = "Are you sure you wan't to overwrite " + recipe.Name + "?";
                string caption = "Overwrite existing recipe";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

                if (result.Equals(MessageBoxResult.Yes)) return true;
            }
            return false;
        }


        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Grabs all the inputs from the info and fills out the recipe with that data
        private void SaveRecipe()
        {
            recipe = recipe != null ? recipe : new Recipe();
            recipe.Name = txbxName.Text;
            recipe.Description = txbxDescription.Text;
            recipe.NumberOfServes = txbxNumServes.Text;
            recipe.PrepTime = txbxPrepTime.Text;

            var category = cmbxCategory.SelectedItem as Category;
            if (category != null) recipe.CategoryID = category.ID;
            recipe.Category = category;
            recipe.Ingredients = ingredients;
            recipe.RecipeSteps = steps;
        }

        // On submit ensure that all the necessary fields have data, and that the user want's to overwrite the existing fields
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (AreThereChanges())
            {
                // If all input fields are valid, then proceed with the saving process
                if (AreInputsValid())
                {
                    // If recipe is not null, then overwrite an already existing recipe
                    if (recipe != null)
                    {
                        if (UserWantsToOverwriteMessage())
                        {
                            SaveRecipe();

                            dbContext.SaveChanges();
                            closeFromSave = true;
                            Close();
                        }
                    }
                    // Otherwise save a new recipe
                    else
                    {
                        MainWindow main = Owner as MainWindow;
                        SaveRecipe();
                        dbContext.Recipes.Add(recipe);
                        dbContext.SaveChanges();
                        main.UpdateRecipeListView();
                        closeFromSave = true;
                        Close();
                    }
                }
            }
            else
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeFromSave)
            {
                if (AreThereChanges())
                {
                    if (!CloseNoSave())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        #region Ingredient Manipulation
        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            // Set the currently selected ingredient to null as we will be adding a new ingredient
            selIngredient = null;
            IngredientWindow window = new IngredientWindow(this.recipe);
            window.Owner = this;
            window.Show();
        }

        private void EditIngredient_Click(object sender, RoutedEventArgs e)
        {
            Ingredient ingredient = (Ingredient) listViewIngredients.SelectedItem;
            if (ingredient != null)
            {
                // Set the currently selected ingredient to the ListView's currently selected ingredient. This will be overriden
                selIngredient = listViewIngredients.SelectedItem as Ingredient;
                IngredientWindow window = new IngredientWindow(this.recipe, selIngredient);
                window.Owner = this;
                window.Show();
            }
            else
            {
                var messageBoxText = "Can't modify an ingredient that doesn't exist";
                var caption = "No ingredient selected";
                var button = MessageBoxButton.OK;
                var icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
            
        }

        private void RemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            var message = "Can't delete an ingredient if there is none selected";
            var caption = "No ingredient selected";
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Error;

            // Ensure that there is an ingredient selected
            var ingredient = listViewIngredients.SelectedItem as Ingredient;
            if (ingredient != null)
            {
                message = "Are you sure you wan't to delete " + ingredient.Name + "\r\nThis action cannot be undone";
                caption = "Deletion warning";
                button = MessageBoxButton.YesNo;
                icon = MessageBoxImage.Warning;

                // Double check with the user before deleting the ingredient
                if (MessageBox.Show(message,caption,button,icon) == MessageBoxResult.Yes)
                {
                    ingredients.Remove(ingredient);
                    return;
                }
            }

            // Show an error message
            MessageBox.Show(message, caption, button, icon);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            // Check if the new ingredient has an id, and if so replace the already existing ingredient
            if (selIngredient == null)
            {
                // Add a brand new ingredient
                ingredients.Add(ingredient);
            }

            // Update ingredients list view
            listViewIngredients.ItemsSource = ingredients;
        }
        #endregion

        #region RecipeStep Manipulation
        private void StepAddButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that selected step is null so RecipeWindow adds a new step.
            selStep = null;
            StepWindow window = new StepWindow(this.recipe);
            window.Owner = this;
            window.Show();
        }

        private void StepModifyButton_Click(object sender, RoutedEventArgs e)
        {
            RecipeStep step = (RecipeStep)ListViewSteps.SelectedItem;
            if (step != null)
            {
                selStep = step;
                StepWindow window = new StepWindow(this.recipe, selStep);
                window.Owner = this;
                window.Show();
            }
            else
            {
                var messageBoxText = "Can't modify an step that doesn't exist";
                var caption = "No steps selected";
                var button = MessageBoxButton.OK;
                var icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        private void StepRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var message = "Can't delete a step if there is none selected";
            var caption = "No step selected";
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Error;

            // Ensure that there is a step selected
            var step = ListViewSteps.SelectedItem as RecipeStep;
            if (step != null)
            {
                message = "Are you sure you wan't to delete " + step.Description + "\r\nThis action cannot be undone";
                caption = "Deletion warning";
                button = MessageBoxButton.YesNo;
                icon = MessageBoxImage.Warning;

                // Double check with the user before deleting the step
                if (MessageBox.Show(message, caption, button, icon) == MessageBoxResult.Yes)
                {
                    steps.Remove(step);
                    FixSteps();
                }
                return;
            }

            // Show an error message if no step is selected
            MessageBox.Show(message, caption, button, icon);
        }

        public void UpdateStep(RecipeStep step)
        {
            // Check if a step is selected, and if so replace the already existing step
            if (selStep == null)
            {
                // Set the number to one above the last step number
                step.Number = (from Step in steps select Step.Number).DefaultIfEmpty(0).Max() + 1;

                // Add a brand new step
                steps.Add(step);
            }

            // Update ingredients list view
            ListViewSteps.ItemsSource = steps;
        }

        private void FixSteps()
        {
            var orderedSteps = new ObservableCollection<RecipeStep>(steps.OrderBy(Step => Step.Number));

            for (var i = 0; i < orderedSteps.Count - 1; i++)
            {
                var nextNum = orderedSteps[i].Number + 1;
                var step = orderedSteps[i + 1];

                if (step.Number != nextNum) orderedSteps[i + 1].Number = nextNum;
            }

            steps = orderedSteps;
            ListViewSteps.ItemsSource = steps;
        }
        #endregion

        #region Move recipe steps
        private void SwapSteps(bool isNext, RecipeStep step)
        {
            // Shorthand if else, if isNext is true then swap with the next number otherwise swap with the previous number
            var otherNumber = isNext ? step.Number + 1 : step.Number - 1;
            var otherStep = steps.Where(QStep => QStep.Number == otherNumber).FirstOrDefault();

            var stepID = steps.IndexOf(step);
            var otherStepID = steps.IndexOf(otherStep);

            // Fix the steps if an error is encountered then exit the method
            if (otherStep == null)
            {
                FixSteps();
                return;
            }

            // Update step number
            int tempNumber = step.Number;
            steps[stepID].Number = otherStep.Number;
            steps[otherStepID].Number = tempNumber;

            // Set currently selected step
            var curStep = steps[stepID];

            // Sort list
            var newsteps = steps.OrderBy(Step => Step.Number);
            steps = new ObservableCollection<RecipeStep>(newsteps);

            // Update listview
            ListViewSteps.ItemsSource = steps;
            ListViewSteps.SelectedItem = curStep;
        }

        private void ButtonStepMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            SwapSteps(false, button.CommandParameter as RecipeStep);
        }

        private void ButtonStepMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            SwapSteps(true, button.CommandParameter as RecipeStep);
        }
        #endregion
    }
}
