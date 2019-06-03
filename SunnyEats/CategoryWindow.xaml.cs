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


        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CategoryInput window = new CategoryInput();
            window.Owner = this;
            window.Show();
        }

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

                    dbContext.SaveChanges();
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.ListViewRecipes_Update();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Focus();
        }
    }
}
