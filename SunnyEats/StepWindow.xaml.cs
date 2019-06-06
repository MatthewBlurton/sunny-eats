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
    public partial class StepWindow : Window
    {
        public StepWindow(Recipe recipe)
        {
            InitializeComponent();

            this.recipe = recipe;
            step = new RecipeStep();
            step.RecipeID = recipe.ID;
        }

        public StepWindow(Recipe recipe, RecipeStep step) : this(recipe)
        {
            this.step = step;
            this.DataContext = step;

            isNew = false;

            ButtonAdd.Content = "Overwrite";
        }

        private Recipe recipe;
        private RecipeStep step;

        private bool saving = false;
        // Used to check if the current step is a new step
        private bool isNew = true;

        /// <summary>
        /// If there are changes to an existing ingredient then ask the user if they are sure they want to override the previous ingredient, and if so close.
        /// Otherwise if it is a new ingredient, just save without prompting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (isNew)
            {
                if (IsFormValid())
                {
                    step.Description = txbxDescription.Text;

                    var parent = Owner as RecipeWindow;
                    parent.UpdateStep(step);
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
                            step.Description = txbxDescription.Text;

                            var parent = Owner as RecipeWindow;
                            parent.UpdateStep(step);
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

        /// <summary>
        /// Close the window if cancel is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// If the application hasn't saved then check if there are changes, and if so warn the user before closing the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Validate the current form
        /// </summary>
        /// <returns>true if all the requirements of the form are met</returns>
        private bool IsFormValid()
        {
            var message = "";
            var caption = "Form not valid";
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Error;

            var valid = true;

            if (txbxDescription.Text == "")
            {
                message = "Step must have atleast 1 character";
                valid = false;
            }
            if (!valid) MessageBox.Show(message, caption, button, icon);

            return valid;
        }

        /// <summary>
        /// Checks if there are differences between the initial ingredient, and the form
        /// </summary>
        /// <returns>true if there are changes between the form and the initial ingredient</returns>
        private bool AreThereChanges()
        {
            string description = txbxDescription.Text;
            if (description != this.step.Description) return true;
            return false;
        }

        /// <summary>
        /// Display a message box warning the user that they are about to close without saving.
        /// </summary>
        /// <returns>true if the user agrees to lose there changes.</returns>
        private bool ExitWithoutSavingMessage()
        {
            string message = "You have unsaved changes, are you sure you want to close this step without saving?";
            string caption = "Quit without saving";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

            if (result.Equals(MessageBoxResult.No)) return false;
            return true;
        }

        /// <summary>
        /// Warn the user that they are about to replace an already existing step
        /// </summary>
        /// <returns>true if the user is willing to overwrite an already existing recipe</returns>
        private bool OverwriteExistingMessage()
        {
            string message = "Are you sure you wan't to overwrite the current step?";
            string caption = "Overwrite existing step";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(message, caption, button, icon);

            if (result.Equals(MessageBoxResult.Yes)) return true;
            return false;
        }

        /// <summary>
        /// Makes the Owner of this window the main focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Focus();
        }
    }
}