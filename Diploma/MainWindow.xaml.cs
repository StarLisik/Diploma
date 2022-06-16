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
        ListBox lectorsList = new ListBox(); //content of tab

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string sqlExprssion = "SELECT * FROM Lectors";

            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(1);
                            lectorsList.Items.Add(name);
                        }
                    }
                }
                command.ExecuteNonQuery();
            }
            LectorsTabContent();
        }

        private void LectorClick(string name)
        {
            LectorWindow lectorWindow = new LectorWindow();
            lectorWindow.name = name;
            lectorWindow.Show();
        }

        private void LectorSelect()
        {
            lectorsList.PreviewMouseUp += PlaceholdersListBox_OnPreviewMouseUp;

        }

        void LectorsTabContent()
        {
            lectorsList.Name = "Lector";
            lectorsList.PreviewMouseUp += PlaceholdersListBox_OnPreviewMouseUp;

            Lectors.Content = lectorsList;

        }

        private void PlaceholdersListBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {
                string name = (string)lectorsList.SelectedItem;
                LectorClick(name);
            }
        }
    }
}
