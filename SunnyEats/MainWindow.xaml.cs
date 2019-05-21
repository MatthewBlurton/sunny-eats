using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.ObjectModel;
using SunnyEats.EntityDataModel;
using SunnyEats.EntityDataModel.Tables;
// using SunnyEats.Models;

namespace SunnyEats
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.dBContext = new MenuDBContext();

            foreach (var item in this.dBContext.Recipes)
            {
                Recipes.Add(item);
            }

            this.ListViewRecipes.ItemsSource = recipes;

            this.recipeWindow = new RecipeWindow();
        }

        private RecipeWindow recipeWindow;

        private MenuDBContext dBContext;

        private ObservableCollection<Recipe> recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get
            {
                if (recipes == null)
                    recipes = new ObservableCollection<Recipe>();
                return recipes;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            recipeWindow.Owner = this;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            recipeWindow.ShowDialog();
        }

        private void RecipeGridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            sizeChangedEventArgs.Handled = true;
            var column = (GridViewColumnHeader)sender;

            column.Column.Width = LimitWidth(54, column.Column.Width);
        }

        private double LimitWidth(double minWidth, double curWidth)
        {
            return curWidth > minWidth ? curWidth : minWidth;
        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)this.ListViewRecipes.SelectedItem ?? null;

            if (selectedRecipe != null)
            {
                string messageBoxText = "Are you sure you want to delete " + selectedRecipe.Name + "\r\nThis action cannot be undone.";
                string caption = "Delete " + selectedRecipe.Name;
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    this.dBContext.Recipes.Remove(selectedRecipe);
                    this.dBContext.SaveChanges();

                    this.Recipes.Remove(selectedRecipe);
                }
            }
        }
    }


}
