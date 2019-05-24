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
            // if a recipe is included, populate the steps with the recipe's steps
            if (this.recipe != null)
            {
                foreach (RecipeStep step in this.recipe.RecipeSteps)
                {
                    steps.Add(step);
                }
            }

            // Load all the categories
            foreach (Category category in dbContext.Categories)
            {
                this.categories.Add(category);
            }
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
            if (AskToCloseSave()) this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !AskToCloseNoSave();
        }

        
    }
}
