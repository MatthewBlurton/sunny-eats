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
    /// Interaction logic for RecipeViewWindow.xaml
    /// </summary>
    public partial class RecipeViewWindow : Window
    {
        public RecipeViewWindow(EntityDataModel.Tables.Recipe recipe)
        {
            InitializeComponent();

            this.DataContext = recipe;
        }
    }
}
