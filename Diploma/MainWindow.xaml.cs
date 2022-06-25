﻿using System;
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
        //ListBox lectorsList = new ListBox();
        ListBox groupsListbox = new ListBox();
        //ListBox lessonsList = new ListBox();

        List<Lector> lectorList = new List<Lector>();
        List<Lesson> lessonslist = new List<Lesson>();
        List<Group> grouplist = new List<Group>();

        public class Lesson
        {
            public string Date { get; set; }
            public string Subject { get; set; }
            public string Lector { get; set; }
            public string Group { get; set; }
        }

        public class Lector
        {
            public string Name { get; set; }
            public string Subject { get; set; }
            public string Done { get; set; }
            public string Pending { get; set; }
        }

        public class Group
        {
            public string Name { get; set; }
            public string Subject { get; set; }
            public float Mark { get; set; }
            public float Visit { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LectorsShow();
            GroupsShow();
            LessonsShow();
            LectorsChoose();
            GroupsChoose();
        }

        private void LectorsShow()
        {
            lectorList.Clear();
            LectorGrid.ItemsSource = null;
            LectorGrid.Items.Refresh();

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

        private void GroupsShow()
        {
            groupsListbox.Items.Clear();
            Groups.Children.Remove(groupsListbox);

            using (var connection = new SqliteConnection("Data Source=app_db.db"))
            {
                connection.Open();

                string sqlExprssion = "SELECT * FROM groups";
                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            groupsListbox.Items.Add(reader.GetString(1));
                        }
                    }
                }
                Groups.Children.Add(groupsListbox);
                groupsListbox.PreviewMouseUp += GroupsList_PreviewMouseUp;
            }

        }

        private void GroupsGridShow(string name)
        {
            grouplist.Clear();
            GroupsGrid.ItemsSource = null;

            string sqlExpression = $@"SELECT
                                        s.student_name 'Студент',
                                        p.subject 'Предмет',
                                        round(AVG(ls.mark), 2) 'Средняя оценка',
                                        ROUND(AVG(ls.visit) * 100, 2) 'Средний % посещаемости'
                                    FROM groups g
                                    LEFT JOIN students s ON s.group_id = g.id
                                    LEFT JOIN lesson_stats ls ON ls.student_id = s.id
                                    LEFT JOIN lessons l on ls.lesson_id = l.id
                                    LEFT JOIN professors p ON l.professor_id = p.id
                                    WHERE g.group_name = '{name}'
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
                            string namee = reader.GetString(0);
                            string subject = reader.GetString(1);
                            float mark = reader.GetFloat(2);
                            float visit = reader.GetFloat(3);

                            grouplist.Add(new Group { Name = namee, Subject = subject, Mark = mark, Visit = visit });
                        }
                    }
                }
                GroupsGrid.ItemsSource = grouplist;
                groupsListbox.Visibility = Visibility.Collapsed;
                GroupsGrid.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Visible;
                command.ExecuteNonQuery();
            }
        }

        private void LessonsShow()
        {
            lessonslist.Clear();
            LessonsGrid.ItemsSource = null;
            LessonsGrid.Items.Refresh();

            string sqlExpression = @"SELECT l.lesson_date 'Дата урока',
                                            p.subject 'Предмет',
                                            p.professor_name 'Преподаватель',
                                            g.group_name 'Группа'
                                    FROM lessons l
                                    JOIN professors p ON p.id = l.professor_id
                                    JOIN groups g ON g.id = l.group_id
                                    ORDER BY 1, 2";


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
                            string date = reader.GetString(0);
                            string subject = reader.GetString(1);
                            string lector = reader.GetString(2);
                            string group = reader.GetString(3);

                            lessonslist.Add(new Lesson { Date = date, Subject = subject, Lector = lector, Group = group });
                        }
                    }

                    LessonsGrid.ItemsSource = lessonslist;
                }

                command.ExecuteNonQuery();
            }
        }

        //private void GroupClick(string name)
        //{
        //    GroupWindow groupWindow = new GroupWindow();
        //    groupWindow.name = name;
        //    groupWindow.Show();
        //}

        private void GroupsList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
                string name = (string)groupsListbox.SelectedItem;
                GroupsGridShow(name);
            }
        }

        //private void PlaceholdersListBox_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
        //    if (item != null)
        //    {
        //        string name = (string)lectorsList.SelectedItem;
        //        LectorClick(name);
        //    }
        //}

        private void FindLectorClick(object sender, RoutedEventArgs e)
        {
            lectorList.Clear();
            LectorGrid.ItemsSource = null;
            LectorGrid.Items.Refresh();

            string text = LectorName.Text;
            Regex regex = new Regex($@"(\w*){text}(\w*)", RegexOptions.IgnoreCase);

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

                SqliteCommand command = new SqliteCommand(sqlExprssion, connection);

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (regex.IsMatch(reader.GetString(0)))
                            {
                                string namee = reader.GetString(0);
                                string subject = reader.GetString(1);
                                string done = reader.GetString(2);
                                string pending = reader.GetString(3);

                                lectorList.Add(new Lector { Name = namee, Subject = subject, Done = done, Pending = pending });
                            }
                        }
                    }
                }
                command.ExecuteNonQuery();
                LectorGrid.ItemsSource = lectorList;
            }
        }

        private void FindStudentClick(object sender, RoutedEventArgs e)
        {
            lessonslist.Clear();
            LessonsGrid.ItemsSource = null;
            LessonsGrid.Items.Refresh();

            string text = StudentFind.Text;
            Regex regex = new Regex($@"(\w*){text}(\w*)", RegexOptions.IgnoreCase);

            string sqlExpression = @"SELECT l.lesson_date 'Дата урока',
                                            p.subject 'Предмет',
                                            p.professor_name 'Преподаватель',
                                            g.group_name 'Группа'
                                    FROM lessons l
                                    JOIN professors p ON p.id = l.professor_id
                                    JOIN groups g ON g.id = l.group_id
                                    ORDER BY 1, 2";


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
                            if (regex.IsMatch(reader.GetString(2)))
                            {
                                string date = reader.GetString(0);
                                string subject = reader.GetString(1);
                                string lector = reader.GetString(2);
                                string group = reader.GetString(3);

                                lessonslist.Add(new Lesson { Date = date, Subject = subject, Lector = lector, Group = group });
                            }
                        }
                    }

                    LessonsGrid.ItemsSource = lessonslist;
                }

                command.ExecuteNonQuery();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button.Name == "LectorCancel")
                LectorsShow();
            else if (button.Name == "SudentCancel")
                LessonsShow();
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            GroupsGrid.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            groupsListbox.Visibility = Visibility.Visible;

            GroupsShow();
        }

        private void CreateLesson(object sender, RoutedEventArgs e)
        {
            string lessonsDate = LessonDate.SelectedDate.Value.Date.ToShortDateString();
            string lector = (string)LectorPick.SelectedItem;
            string group = (string)GroupPick.SelectedItem;
        }

        private void LectorsChoose()
        {
            if (LectorPick.Items.Count == 0)
            {
                string sqlExprssion = "SELECT * FROM professors";

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
                                LectorPick.Items.Add(reader.GetString(1));
                            }
                        }

                        LectorPick.SelectedIndex = 0;
                    }
                    command.ExecuteNonQuery();
                }
            }
        }

        private void GroupsChoose()
        {
            if (GroupPick.Items.Count == 0)
            {
                string sqlExprssion = "SELECT * FROM groups";

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
                                GroupPick.Items.Add(reader.GetString(1));
                            }
                        }

                        GroupPick.SelectedIndex = 0;
                    }
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
