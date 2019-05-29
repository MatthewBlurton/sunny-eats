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

        public RecipeWindow(Recipe recipe) : this()
        {
            this.recipe = recipe;
            this.DataContext = this.recipe;
        }

        private MenuDBContext dbContext;
        public readonly Recipe recipe;
        ObservableCollection<Category> categories;
        ObservableCollection<RecipeStep> steps;

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
                    steps.Add(step);
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

        private bool AreThereChanges()
        {
            // Initialize variables
            string name;
            string desc;
            int? catId;
            string prepTime;
            string serves;
            string calkPerServe;
            List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
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

            return false;
        }


        /// <summary>
        /// Ask the user if they would like to close without saving, if there have been changes made
        /// </summary>
        /// <returns>Did the user say yes or no?</returns>
        private bool AskToCloseNoSave()
        {
            string message = "You have unsaved changes, are you sure you want to stop?";
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
        private bool AskToCloseSave()
        {
            if (recipe != null)
            {
                string message = "Are you sure you wan't to overwrite " + recipe.Name + "?";
                string caption = "Overwrite existing recipe";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

                if (result.Equals(MessageBoxResult.Yes)) return false;
            }
            return true;
        }


        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            //if (AskToCloseSave()) this.Close();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = !AskToCloseNoSave();
        }

        
    }
}
