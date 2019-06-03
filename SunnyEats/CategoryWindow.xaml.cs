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
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        public CategoryWindow()
        {
            InitializeComponent();

            dbContext = new MenuDBContext();
            categories = new ObservableCollection<Category>(dbContext.Categories.ToList());

            ListViewCategories.ItemsSource = categories;
        }

        private MenuDBContext dbContext;
        private ObservableCollection<Category> categories;

        /// <summary>
        /// Opens a input dialogue where the user can manually add a category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CategoryInput window = new CategoryInput();
            window.Owner = this;
            window.Show();
        }

        /// <summary>
        /// Opens a delete dialogue where the user can delete a category.
        /// It only opens if a category is selected/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewCategories.SelectedItem != null)
            {
                var category = ListViewCategories.SelectedItem as Category;
                var message = "Are you sure you want to delete\r\n" + category.Name + "?";
                var caption = "Delete Category";
                var button = MessageBoxButton.YesNo;
                var icon = MessageBoxImage.Warning;

                if (MessageBox.Show(message, caption, button, icon) == MessageBoxResult.Yes)
                {
                    categories.Remove(category);
                }
            }
        }

        /// <summary>
        /// Close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Set the Owner of this Window as the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow window = new MainWindow();
            dbContext.SaveChanges();
            window.ListViewRecipes_Update();
            Owner.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
