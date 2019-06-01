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

            btnAdd.Content = "Overwrite";
        }

        private Recipe recipe;
        private RecipeStep step;

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            step.Description = txbxDescription.Text;

            var parent = Owner as RecipeWindow;
            parent.UpdateStep(step);

            this.Close();
        }
    }
}
