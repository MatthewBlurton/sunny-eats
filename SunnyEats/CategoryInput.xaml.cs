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
    /// Interaction logic for CategoryInput.xaml
    /// </summary>
    public partial class CategoryInput : Window
    {
        public CategoryInput()
        {
            InitializeComponent();
        }

        public CategoryInput(Category category) : this()
        {
            this.category = category;
        }

        private Category category;

        private bool AreThereChanges()
        {
            string name = TextBoxName.Text;
            if (name != category.Name) return true;
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
    }
}
