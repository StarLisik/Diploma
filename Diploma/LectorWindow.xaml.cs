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
using System.Data.SqlClient;
using System.Data;


namespace Diploma
{
    /// <summary>
    /// Interaction logic for LectorWindow.xaml
    /// </summary>
    public partial class LectorWindow : Window
    {
        public string name;

        public class Lector
        {
            public string Name { get; set; }
            public string Mail { get; set; }
            public string Subject { get; set; }
            public int Hours { get; set; }
        }

        public LectorWindow()
        {
            InitializeComponent();

            this.Loaded += LectorWindow_Loaded;
        }

        private void LectorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string sqlExprssion = $"SELECT * FROM Lectors WHERE Name = '{name}'";

            using (var connection = new SqliteConnection("Data Source=OnlineSchool.db"))
            {
                connection.Open();
        
                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<Lector> lectorList = new List<Lector>();
                        while (reader.Read())
                        {
                            string name = reader.GetString(1);
                            string mail = reader.GetString(2);
                            string subject = reader.GetString(3);
                            int hours = reader.GetInt32(4);

                            lectorList.Add(new Lector { Name = name, Mail = mail, Subject = subject, Hours = hours });

                            LectorGrid.ItemsSource = lectorList;
                        }
                    }
                }
                command.ExecuteNonQuery();

            }
        }
    }
}
