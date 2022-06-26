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
using System.Data;

namespace Diploma
{
    /// <summary>
    /// Interaction logic for GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : Window
    {
        List<Student> students = new List<Student>();

        public class GroupList
        {
            public string ID { get; set; }
            public string Date { get; set; }
            public string Subject { get; set; }
            public string Lector { get; set; }
            public string Group { get; set; }
        }

        public class Student
        {
            public string FIO { get; set; }
            public int Visit { get; set; }
            public int? Mark { get; set; }
        }

        public GroupList groupGrid = new GroupList();

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

            GroupDataGridFill();
        }

        private void GroupDataGridFill()
        {
            string sqlExprssion = $@"SELECT s.student_name 'Имя ученика', ls.visit 'Посещаемость', ls.mark 'Оценка'
                                    FROM lesson_stats ls
                                    JOIN students s ON s.id = ls.student_id
                                    WHERE ls.lesson_id = {groupGrid.ID}";

            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string namee = reader.GetString(0);
                            int visit = reader.GetInt32(1);
                            int? mark = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                            students.Add(new Student { FIO = namee, Visit = visit, Mark = mark });
                        }
                    }
                    CreateLessonGrid.ItemsSource = students;
                }
            }
        }

        private void UpdateLessons(object sender, DataGridRowEditEndingEventArgs e)
        {
            var selectedItem = (Student)CreateLessonGrid.SelectedItem;
            int studentNameId = 0;
            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                string sqlExprssion = @$"SELECT * FROM students";
                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(1) == selectedItem.FIO)
                            {
                                studentNameId = reader.GetInt32(0);
                                break;
                            }
                        }
                    }
                }

                int visit = selectedItem.Visit;
                int? mark = selectedItem.Mark;
                if (mark != null)
                    sqlExprssion = @$"UPDATE lesson_stats
                                    SET visit = {visit},
                                        mark = {mark}
                                    WHERE lesson_id = {groupGrid.ID} AND student_id = {studentNameId}";
                else
                    sqlExprssion = @$"UPDATE lesson_stats
                                    SET visit = {visit},
                                        mark = NULL
                                    WHERE lesson_id = {groupGrid.ID} AND student_id = {studentNameId}";




                command = new SqliteCommand(sqlExprssion, connection);
                command.ExecuteNonQuery();
            }
        }

        private void LessonFinished(object sender, RoutedEventArgs e)
        {
            int lesson_id = int.Parse(groupGrid.ID);
            string sqlExprssion = @$"UPDATE lessons
                                    SET finished = 1
                                    WHERE id = {lesson_id}";

            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
