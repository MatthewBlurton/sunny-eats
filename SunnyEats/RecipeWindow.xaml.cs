using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SunnyEats
{
    public partial class RecipeWindow : Window
    {
        /// <summary>
        /// This constructor is used when creating a new recipe
        /// </summary>
        public RecipeWindow()
        {
            InitializeComponent();

            dbContext = new MenuDBContext();

            categories = new ObservableCollection<Category>(dbContext.Categories.ToList());
            steps = new ObservableCollection<RecipeStep>();
            ingredients = new ObservableCollection<Ingredient>();

            cmbxCategory.ItemsSource = categories;
            ListViewSteps.ItemsSource = steps;
            listViewIngredients.ItemsSource = ingredients;

            recipe = new Recipe();
            isNew = true;
        }

        /// <summary>
        /// This constructor is used when updating an already existing recipe. It populates both steps and ingredients
        /// </summary>
        /// <param name="recipe"></param>
        public RecipeWindow(Recipe recipe) : this()
        {
            this.recipe = dbContext.Recipes.First(Rec => Rec.ID == recipe.ID);
            DataContext = this.recipe;

            // Populate steps and ingredients
            steps = new ObservableCollection<RecipeStep>(dbContext.RecipeSteps.Where(Step => Step.RecipeID == recipe.ID));
            ingredients = new ObservableCollection<Ingredient>(dbContext.Ingredients.Where(Ing => Ing.RecipeID == recipe.ID));

            // Poor performance, need to figure out a LINQ query to replace the for loop
            // Used to select the correct category
            for (int i = 0; i < cmbxCategory.Items.Count; i++)
            {
                Category currentCat = (Category)cmbxCategory.Items[i];
                if (currentCat.ID == this.recipe.CategoryID)
                {
                    cmbxCategory.SelectedIndex = i;
                    break;
                }
            }
            UpdateAndCorrectSteps();
            ListViewIngredients_Update();

            isNew = false;
        }

        // Used for updating current recipe
        private MenuDBContext dbContext;
        private Recipe recipe;
        private ObservableCollection<Category> categories;
        private ObservableCollection<RecipeStep> steps;
        private ObservableCollection<Ingredient> ingredients;

        // Used for verification checking
        bool stepsHaveChanged = false;
        bool ingredientChanged = false;
        bool isNew = false;

        // If an item is going to be overriden, these variables will be overriden
        private Ingredient selIngredient;
        private RecipeStep selStep;

        // Used when closing to check what condition the window is closing (is it from a submission, or just cancel/close)
        bool closeFromSave = false;

        /// <summary>
        /// Validates all the form inputs before submitting
        /// </summary>
        /// <returns>True if all inputs in the window are correct</returns>
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

        /// <summary>
        /// Checks the recipes for changes
        /// </summary>
        /// <returns>true if recipes data values does not equal the form input entries</returns>
        private bool AreThereChanges()
        {
            // Initialize variables
            string name;
            string desc;
            int? catId;
            string prepTime;
            string serves;
            string calkPerServe;

            if (!isNew)
            {
                // Populate variable data with the original recipe data
                name = recipe.Name;
                desc = recipe.Description;
                catId = recipe.CategoryID;
                prepTime = recipe.PrepTime;
                serves = recipe.NumberOfServes;
                calkPerServe = recipe.Cal_kJ_PerServe;

                // Specialised variables that can't be directly referred to
                var selCategory = cmbxCategory.SelectedItem as Category;
                int? selCategoryID = null;
                if (selCategory != null) selCategoryID = selCategory.ID;

                // Check for any changes between the source variables and the inputs in the window, if there are any differences return true.
                if (txbxName.Text != name) return true;
                if (txbxDescription.Text != desc) return true;
                if (selCategoryID != catId) return true;
                if (txbxPrepTime.Text != prepTime) return true;
                if (txbxNumServes.Text != serves) return true;
                if (txbxCalkJPerServe.Text != calkPerServe) return true;
                if (ingredientChanged) return true;
                if (stepsHaveChanged) return true;
            }
            else
            {
                if (txbxName.Text != "") return true;
                if (txbxDescription.Text != "") return true;
                if (cmbxCategory.SelectedItem != null) return true;
                if (txbxPrepTime.Text != "") return true;
                if (txbxNumServes.Text != "") return true;
                if (txbxCalkJPerServe.Text != "") return true;
                if (listViewIngredients.Items.Count > 0) return true;
                if (ListViewSteps.Items.Count > 0) return true;
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
            if (!isNew)
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

        /// <summary>
        /// The cancel button upon being clicked closes the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Grabs all the inputs from the info and fills out the recipe with that data
        /// </summary>
        private void SaveRecipe()
        {
            recipe = !isNew ? recipe : new Recipe();
            recipe.Name = txbxName.Text;
            recipe.Description = txbxDescription.Text;
            recipe.NumberOfServes = txbxNumServes.Text;
            recipe.PrepTime = txbxPrepTime.Text;
            recipe.Cal_kJ_PerServe = txbxCalkJPerServe.Text;

            var category = cmbxCategory.SelectedItem as Category;
            if (category != null) recipe.CategoryID = category.ID;
            recipe.Category = category;
            //recipe.Ingredients = ingredients;
            //recipe.RecipeSteps = steps;
        }

        /// <summary>
        /// On submit ensure that all the necessary fields have data, and that the user want's to overwrite the existing fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (AreThereChanges())
            {
                // If all input fields are valid, then proceed with the saving process
                if (AreInputsValid())
                {
                    // If recipe is not null, then overwrite an already existing recipe
                    if (!isNew)
                    {
                        if (UserWantsToOverwriteMessage())
                        {
                            SaveRecipe();

                            dbContext.SaveChanges();

                            // Apply changes to MainWindow
                            MainWindow main = Owner as MainWindow;
                            main.ListViewRecipes_Update();

                            closeFromSave = true;
                            Close();
                        }
                    }
                    // Otherwise save a new recipe
                    else
                    {
                        // Save a new recipe
                        SaveRecipe();
                        dbContext.Recipes.Add(recipe);
                        dbContext.SaveChanges();

                        MainWindow main = Owner as MainWindow;
                        main.ListViewRecipes_Update();
                        closeFromSave = true;
                        Close();
                    }
                }
            }
            else
            {
                MainWindow main = Owner as MainWindow;
                main.ListViewRecipes_Update();
                Close();
            }
        }

        /// <summary>
        /// If there are changes in data ensure the user wants to close the window without saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Show a window specifically for adding a new ingredient to the current recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            // Set the currently selected ingredient to null as we will be adding a new ingredient
            selIngredient = null;
            IngredientWindow window = new IngredientWindow(this.recipe);
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Show a window and include an already existing ingredient for editing with the new window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditIngredient_Click(object sender, RoutedEventArgs e)
        {
            Ingredient ingredient = (Ingredient)listViewIngredients.SelectedItem;
            if (ingredient != null)
            {
                // Set the currently selected ingredient to the ListView's currently selected ingredient. This will be overriden
                selIngredient = new Ingredient();

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

        /// <summary>
        /// Remove an ingredient from the current recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                if (MessageBox.Show(message, caption, button, icon) == MessageBoxResult.Yes)
                {
                    // Delete the ingredient
                    ingredients.Remove(ingredient);
                    dbContext.Ingredients.Remove(ingredient);
                    ingredientChanged = true;
                    return;
                }
            }

            // Show an error message
            MessageBox.Show(message, caption, button, icon);
        }

        /// <summary>
        /// Submit a new ingredient to the current recipe
        /// </summary>
        /// <param name="ingredient"></param>
        public void UpdateIngredient(Ingredient ingredient)
        {
            // set ingredientChanged to true
            ingredientChanged = true;
            // Check if the new ingredient has an id, and if so replace the already existing ingredient
            if (selIngredient == null)
            {
                // Add a brand new ingredient
                dbContext.Ingredients.Add(ingredient);
                ingredients.Add(ingredient);
            }
            else
            {
                try
                {
                    var dbIngredient = dbContext.Ingredients.Where(Ing => Ing.ID == selIngredient.ID).First();
                    dbIngredient = ingredient;
                }
                // If there is no dbIngredient in the database that means that the current ingredient is new, so add it instead
                catch (InvalidOperationException)
                {
                    dbContext.Ingredients.Add(ingredient);
                }

            }
            // Update ingredients list view
            ListViewIngredients_Update();
        }

        /// <summary>
        /// Update the ingredient ListView
        /// </summary>
        public void ListViewIngredients_Update()
        {
            listViewIngredients.ItemsSource = null;
            listViewIngredients.ItemsSource = ingredients;
        }
        #endregion

        #region RecipeStep Manipulation
        /// <summary>
        /// Once pressed opens a window where the user can add their own step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StepAddButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure that selected step is null so RecipeWindow adds a new step.
            selStep = null;
            StepWindow window = new StepWindow(this.recipe);
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Once pressed if a step is selected for editing, opens a window where the user can modify the step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Once pressed removes a step from the recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    dbContext.RecipeSteps.Remove(step);
                    UpdateAndCorrectSteps();
                    stepsHaveChanged = true;
                }
                return;
            }

            // Show an error message if no step is selected
            MessageBox.Show(message, caption, button, icon);
        }

        /// <summary>
        /// When returned from the step sindow this method is responsible for either adding a new step
        /// or adjusting an already existing step
        /// </summary>
        /// <param name="step">The new or modified step</param>
        public void UpdateStep(RecipeStep step)
        {
            // Set stepChanged to true
            stepsHaveChanged = true;

            // Check if a step is selected, and if so replace the already existing step
            if (selStep == null)
            {
                // Set the number to one above the last step number
                step.Number = (from Step in steps select Step.Number).DefaultIfEmpty(0).Max() + 1;

                // Add a brand new step
                steps.Add(step);
                dbContext.RecipeSteps.Add(step);
            }
            // If an already existing step is selected, update it
            else
            {
                try
                {
                    var dbStep = dbContext.RecipeSteps.Where(Ing => Ing.ID == selIngredient.ID).First();
                    dbStep = step;
                }
                // If there is no dbStep in the database that means that the current step is new, so add it instead
                catch (InvalidOperationException)
                {
                    dbContext.RecipeSteps.Add(step);
                }
            }

            // Update ingredients list view
            UpdateAndCorrectSteps();
        }

        /// <summary>
        /// Go throgh the steps and re-order them into numerical form, one after the other
        /// </summary>
        public void UpdateAndCorrectSteps()
        {
            // Order steps by their number (this prevents a step with an number of 8 being reduced down to 2 because the step with the previous ID is one below the current)
            var orderedSteps = new ObservableCollection<RecipeStep>(steps.OrderBy(Step => Step.Number));

            for (var i = 0; i < orderedSteps.Count - 1; i++)
            {
                // Initialise step table to be updated in dbContext
                var step = orderedSteps[i];
                var count = dbContext.RecipeSteps.Count();
                var dbStep = steps.Where(Stp => Stp.ID == step.ID).First();
                if (i == 0)
                {
                    var curStep = orderedSteps[i];
                    var curNumber = curStep.Number;

                    orderedSteps[i].Number = i + 1 == curNumber ? curNumber : i + 1;

                }
                var nextNum = orderedSteps[i].Number + 1;
                var nextStep = orderedSteps[i + 1];

                // Update step number and apply changes to dbSteps
                if (nextStep.Number != nextNum)
                {
                    orderedSteps[i + 1].Number = nextNum;
                }

                // Update step table as we go through the collection
                dbStep = orderedSteps[i];
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
                UpdateAndCorrectSteps();
                return;
            }

            // Update step number
            int tempNumber = step.Number;
            steps[stepID].Number = otherStep.Number;
            steps[otherStepID].Number = tempNumber;

            // Update step and otherStep
            step = steps[stepID];
            otherStep = steps[otherStepID];

            // Add to steps modified
            var dbStep = steps.Where(Stp => Stp.ID == step.ID).First();
            var dbOtherStep = steps.Where(Stp => Stp.ID == otherStep.ID).First();
            dbStep = step;
            dbOtherStep = otherStep;

            stepsHaveChanged = true;

            // Sort list
            var newsteps = steps.OrderBy(Step => Step.Number);
            steps = new ObservableCollection<RecipeStep>(newsteps);

            // Update listview
            ListViewSteps.ItemsSource = steps;
            ListViewSteps.SelectedItem = step;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Focus();
        }
    }
}
