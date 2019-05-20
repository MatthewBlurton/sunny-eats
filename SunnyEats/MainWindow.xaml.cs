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

            // LINQ Query inner join Recipe with Category
            //var recipeItems = from r in dBContext.Recipes
            //                  join c in dBContext.Categories
            //                  on r.CategoryID equals c.ID into rc
            //                  from subrecipe in rc.DefaultIfEmpty()
            //                  select new
            //                  {
            //                      r.Name,
            //                      CategoryName = subrecipe.Name ?? String.Empty,
            //                      r.PrepTime,
            //                      r.NumberOfServes
            //                  };

            // Query section without using LINQ
            //// Get data from MenuDB
            //var connectionString = ConfigurationManager.ConnectionStrings["SunnyEats.Properties.Settings.MenuDBConnectionString"].ConnectionString;
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    Console.WriteLine("Connection Successful");
            //    SqlCommand command = connection.CreateCommand();
            //    command.CommandType = System.Data.CommandType.Text;
            //    command.CommandText = "SELECT [Name], [CategoryID] FROM [dbo].[Recipe]";
            //    SqlDataReader reader = command.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        var recipe = new Recipe
            //        {
            //            Name = reader.GetString(0),
            //            Category = reader.GetInt32(1)
            //        };
            //        Recipes.Add(recipe);
            //        Console.WriteLine(reader.GetString(0));
            //    }
            //    reader.Close();
            //}

            //ListViewRecipes.ItemsSource = Recipes;
        }

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

        //public class Recipe
        //{
        //    public String Name { get; set; }
        //    public int Category { get; set; }
        //}
    }

    
}
