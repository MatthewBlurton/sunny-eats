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
            this.ingredient = ingredient; // if a ingredient is provided, then the provided ingredient will be overwritten
            isNew = false;

            DataContext = this.ingredient;

            btnSubmit.Content = "Overwrite";
        }

        public readonly Recipe recipe;

        private Ingredient ingredient;

        private bool saving = false;
        // Used to check if the current ingredient is a new ingredient
        private bool isNew = true;

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                if (IsFormValid())
                {
                    ingredient.Name = txbxIngredient.Text;
                    ingredient.Quantity = txbxQuantity.Text;

                    var parent = Owner as RecipeWindow;
                    parent.UpdateIngredient(ingredient);
                    parent.ListViewIngredients_Update();
                }
                else return;
            }
            else
            {
                if (IsFormValid())
                {
                    if (AreThereChanges())
                    {
                        if (OverwriteExistingMessage())
                        {
                            ingredient.Name = txbxIngredient.Text;
                            ingredient.Quantity = txbxQuantity.Text;

                            var parent = Owner as RecipeWindow;
                            parent.UpdateIngredient(ingredient);
                            parent.ListViewIngredients_Update();
                        }
                        else return;
                    }
                    else return;
                }
                else return;
            }

            saving = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saving)
            {
                if (AreThereChanges())
                {
                    if (!ExitWithoutSavingMessage())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private bool IsFormValid()
        {
            var message = "";
            var caption = "Form not valid";
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Error;

            var valid = true;

            if (txbxIngredient.Text == "")
            {
                message = "Ingredient must have atleast 1 character";
                valid = false;
            }
            if (!valid) MessageBox.Show(message, caption, button, icon);

            return valid;
        }

        private bool AreThereChanges()
        {
            string ingredient = txbxIngredient.Text;
            string quantity = txbxQuantity.Text;
            if (ingredient != this.ingredient.Name) return true;
            if (quantity != this.ingredient.Quantity) return true;
            return false;
        }

        private bool ExitWithoutSavingMessage()
        {
            string message = "You have unsaved changes, are you sure you want to close this ingredient without saving?";
            string caption = "Quit without saving";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

            if (result.Equals(MessageBoxResult.No)) return false;
            return true;
        }

        private bool OverwriteExistingMessage()
        {
            string message = "Are you sure you wan't to overwrite the current ingredient?";
            string caption = "Overwrite existing ingredient";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

            if (result.Equals(MessageBoxResult.Yes)) return true;
            return false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Focus();
        }
    }
}
