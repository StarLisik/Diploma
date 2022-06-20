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
        public string name;
        public class Group
        {
            public string Name { get; set; }
            public string FIO { get; set; }
            public string Subject { get; set; }
            public float Mark { get; set; }
            public float Visit { get; set; }
        }

        public GroupWindow()
        {
            InitializeComponent();

            this.Loaded += GroupWindow_Loaded;
        }

        private void GroupWindow_Loaded(object sender, RoutedEventArgs e)
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
                List<Group> groupslist = new List<Group>();

                foreach (var member in group)
                {
                    sqlExpression = $"SELECT * FROM {member}";
                    command = new SqliteCommand(sqlExpression, connection);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (name == reader.GetString(0))
                                {
                                    string groupname = reader.GetString(0);
                                    string fio = reader.GetString(1);
                                    string subject = reader.GetString(2);
                                    float mark = reader.GetFloat(3);
                                    float visit = reader.GetFloat(4);

                                    groupslist.Add(new Group { Name = groupname, FIO = fio, Subject = subject, Mark = mark, Visit = visit });
                                }
                            }
                        }
                    }
                }

                GroupGrid.ItemsSource = groupslist;
            }
        }
    }
}
