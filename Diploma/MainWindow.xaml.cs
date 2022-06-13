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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;

namespace Diploma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Lectors (Name, Mail, Subject, Hours) VALUES ('Бурмистров Родион Александрович', 'rodion3000000@gmail.com', 'программирование', 45)";
                command.ExecuteNonQuery();
            }
            LectorsTabContent();
        }

        private void LectorClick()
        {
            LectorWindow lectorWindow = new LectorWindow();
            lectorWindow.Show();
        }

        void LectorsTabContent()
        {
            ListBox lectorsList = new ListBox(); //content of tab
            lectorsList.Name = "Lector";
            lectorsList.PreviewMouseUp += PlaceholdersListBox_OnPreviewMouseUp;
            lectorsList.Items.Add("Тут будут лекторы");

            Lectors.Content = lectorsList;

        }

        private void PlaceholdersListBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                LectorClick();
            }
        }
    }
}
