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
using Microsoft.Data.Sqlite;
using System.Text.RegularExpressions;

namespace Diploma
{
    /// <summary>
    /// Interaction logic for GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : Window
    {
        public class GroupGrid
        {
            public string ID { get; set; }
            public string Date { get; set; }
            public string Subject { get; set; }
            public string Lector { get; set; }
            public string Group { get; set; }
        }

        public class Student
        {

        }

        public GroupGrid groupGrid = new GroupGrid();

        public GroupWindow()
        {
            InitializeComponent();

            this.Loaded += GroupWindow_Loaded;
        }

        private void GroupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Date.Content = "Дата: " + groupGrid.Date;
            Subject.Content = "Предмет: " + groupGrid.Subject;
            Lector.Content = "Лектор: " + groupGrid.Lector;
            Group.Content = "Группа: " + groupGrid.Group;
        }


    }
}
