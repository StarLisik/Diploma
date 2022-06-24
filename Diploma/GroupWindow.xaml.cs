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

            string sqlExpression = @"SELECT
                                        s.student_name 'Студент',
                                        p.subject 'Предмет',
                                        round(AVG(ls.mark), 2) 'Средняя оценка',
                                        ROUND(AVG(ls.visit) * 100, 2) 'Средний % посещаемости'
                                    FROM groups g
                                    LEFT JOIN students s ON s.group_id = g.id
                                    LEFT JOIN lesson_stats ls ON ls.student_id = s.id
                                    LEFT JOIN lessons l on ls.lesson_id = l.id
                                    LEFT JOIN professors p ON l.professor_id = p.id
                                    GROUP BY s.id, subject";

            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string subject = reader.GetString(1);
                            float mark = reader.GetFloat(2);
                            float visit = reader.GetFloat(3);

                            //grouplist.Add(new Group { Name = name, Subject = subject, Mark = mark, Visit = visit });
                        }
                    }
                }
            }
        }
    }
}
