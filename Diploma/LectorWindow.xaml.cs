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
            public string Subject { get; set; }
            public string Done { get; set; }
            public string Pending { get; set; }
        }

        public LectorWindow()
        {
            InitializeComponent();

            this.Loaded += LectorWindow_Loaded;
        }

        private void LectorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string sqlExprssion = @$"SELECT
                                        p.professor_name 'Преподаватель',
                                        p.subject 'Предмет',
	                                    count(CASE WHEN l.finished = 1 THEN l.id END) 'Проведено',
	                                    count(CASE WHEN l.finished IS NULL OR l.finished = 0 THEN l.id END) 'Предстоит'
                                    FROM professors p
                                    JOIN lessons l ON p.id = l.professor_id
                                    GROUP BY p.id";

            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                List<Lector> lectorList = new List<Lector>();
                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string namee = reader.GetString(0);
                            string subject = reader.GetString(1);
                            string done = reader.GetString(2);
                            string pending = reader.GetString(3);

                            lectorList.Add(new Lector { Name = namee, Subject = subject, Done = done, Pending = pending });
                        }
                    }
                }
                command.ExecuteNonQuery();
                LectorGrid.ItemsSource = lectorList;
            }
        }
    }
}
