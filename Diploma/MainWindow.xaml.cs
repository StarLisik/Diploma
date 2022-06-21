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
using System.Text.RegularExpressions;

namespace Diploma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListBox lectorsList = new ListBox();
        ListBox groupsList = new ListBox();
        ListBox lessonsList = new ListBox();

        public class Lesson
        {
            public string Date { get; set; }
            public string Subject { get; set; }
            public string Lector { get; set; }
            public string Group { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LectorsShow();
            LectorsTabContent();
            GroupsShow();
            GroupsTabContent();
            LessonsTabConent();
        }

        private void LectorsShow()
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
        }

        private void GroupsShow()
        {
            Regex regex = new Regex(@"Group(\w*)", RegexOptions.IgnoreCase);
            string sqlExpression = "SELECT name FROM sqlite_master WHERE type='table'";
            List<string> group = new List<string>();

            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (regex.IsMatch(reader.GetString(0)))
                            {
                                string name = reader.GetString(0);
                                group.Add(name);
                            }
                        }
                    }
                }

                command.ExecuteNonQuery();
            }

            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();
                SqliteCommand command;

                foreach (var member in group)
                {
                    sqlExpression = $"SELECT * FROM {member}";
                    command = new SqliteCommand(sqlExpression, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            groupsList.Items.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }

        private void LessonsShow()
        {
            string sqlExpression = "SELECT * FROM Lessons";
            List<Lesson> lessonslist = new List<Lesson>();

            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string date = reader.GetString(0);
                            string subject = reader.GetString(1);
                            string lector = reader.GetString(2);
                            string group = reader.GetString(3);

                            lessonslist.Add(new Lesson { Date = date, Subject = subject, Lector = lector, Group = group });

                            LessonsGrid.ItemsSource = lessonslist;
                        }
                    }
                }

                command.ExecuteNonQuery();
            }
        }

        private void LectorClick(string name)
        {
            LectorWindow lectorWindow = new LectorWindow();
            lectorWindow.name = name;
            lectorWindow.Show();
        }

        private void GroupClick(string name)
        {
            GroupWindow groupWindow = new GroupWindow();
            groupWindow.name = name;
            groupWindow.Show();
        }

        private void LectorsTabContent()
        {
            lectorsList.Name = "Lector";
            lectorsList.PreviewMouseUp += PlaceholdersListBox_OnPreviewMouseUp;
            lectorsList.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("",
                System.ComponentModel.ListSortDirection.Ascending));
            Lectors.Content = lectorsList;

        }

        private void GroupsTabContent()
        {
            groupsList.Name = "Group";
            groupsList.PreviewMouseUp += GroupsList_PreviewMouseUp;
            Groups.Content = groupsList;
        }

        private void LessonsTabConent()
        {
            LessonsGrid.Name = "Lesson";
            LessonsShow();
        }

        private void GroupsList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
                string name = (string)groupsList.SelectedItem;
                GroupClick(name);
            }
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
