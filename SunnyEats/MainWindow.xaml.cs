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

            MenuDBContext dBContext = new MenuDBContext();

            foreach (var item in dBContext.Recipes)
            {
                Recipes.Add(item);
            }

            this.ListViewRecipes.ItemsSource = recipes;

            this.recipeWindow = new RecipeWindow();
        }

        private RecipeWindow recipeWindow;

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
    }

    
}
