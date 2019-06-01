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
    /// Interaction logic for StepWindow.xaml
    /// </summary>
    public partial class StepWindow : Window
    {
        public StepWindow(Recipe recipe)
        {
            InitializeComponent();

            this.recipe = recipe;
            step = new RecipeStep();
        }

        public StepWindow(Recipe recipe, RecipeStep step) : this(recipe)
        {
            this.step = step;
            this.DataContext = step;

            isNew = false;

            btnAdd.Content = "Overwrite";
        }

        private Recipe recipe;
        private RecipeStep step;

        private bool saving = false;
        // Used to check if the current step is a new step
        private bool isNew = true;

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
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

            if (txbxDescription.Text == "")
            {
                message = "Step must have atleast 1 character";
                valid = false;
            }
            if (!valid) MessageBox.Show(message, caption, button, icon);

            return valid;
        }

        private bool AreThereChanges()
        {
            string description = txbxDescription.Text;
            if (description != this.step.Description) return true;
            return false;
        }

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
    }
}