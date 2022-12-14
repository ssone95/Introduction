using StudentsBook.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.DataGridView;

namespace StudentsBook
{
    public partial class Form1 : Form
    {
        const string StudentDB = "C:\\Users\\nedel\\Documents\\Working with files\\students.csv";
        const string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=StudentsBook;User ID=sa;Password=1234";

        SqlConnection _connection = null;
        bool connectionIsEstablished = false;

        public List<Student> Students { get; set; }

        public Student SelectedStudent { get; set; }

        private StudentService _service;
        public Form1()
        {
            InitializeComponent();

            Students = new List<Student>();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var inputWindow = new UserInputWindow(_service);
                inputWindow.ShowDialog();
                RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);

                //string name = txtName.Text;
                //string surname = txtSurname.Text;
                //DateTime birthdate = dtBirthdate.Value;

                //if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                //{
                //    MessageBox.Show("Please make sure that you filled all the fields properly!", "Input data is not correct!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                //var student = new Student(Guid.NewGuid(), "1112022", name, surname, birthdate, true, DateTime.Now);
                ////Students.Add(student);

                //CreateStudent(student);

                //SaveStudents();
                //LoadStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _connection = new SqlConnection(connectionString);
                _connection.Open();

                connectionIsEstablished = true;

                _service = new StudentService(_connection);

                RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
            }
            catch(Exception ex)
            {
                connectionIsEstablished = false;
                MessageBox.Show($"{ex}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        void LoadStudents()
        {
            Students = new List<Student>();
            try
            {
                string[] studentList = File.ReadAllLines(StudentDB);
                foreach(var studentLine in studentList)
                {
                    string[] studentData = studentLine.Split(';');
                    if(studentData.Length < 4)
                    {
                        continue;
                    }

                    Guid userId = Guid.Parse(studentData[0]);
                    string name = studentData[1];
                    string surname = studentData[2];
                    string dateOfBirth = studentData[3];
                    var student = new Student(userId, "", name, surname, DateTime.Now, true, DateTime.Now);
                    Students.Add(student);
                }
            }
            catch(Exception ex)
            {
            }
            dgStudentsList.DataSource = null;
            dgStudentsList.DataSource = Students;
        }

        void RefreshData(string filterString, DateTime startingDate, DateTime endingDate, bool includeInactive)
        {
            Students = new List<Student>();
            try
            {
                Students = _service.GetStudents(filterString, startingDate, endingDate, includeInactive);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dgStudentsList.DataSource = null;
            dgStudentsList.DataSource = Students;
            dgStudentsList.ClearSelection();
        }

        void SaveStudents()
        {
            try
            {
                List<string> studentLines = new List<string>();
                foreach(Student s in Students)
                {
                    string studentLine = GetStudentLine(s);
                    studentLines.Add(studentLine);
                }

                File.WriteAllLines(StudentDB, studentLines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string GetStudentLine(Student s)
        {
            return $"{s.Id};{s.Name};{s.Surname};{s.DateOfBirth}";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(dgStudentsList.SelectedRows.Count > 0)
            {
                DataGridViewRow studentRow = dgStudentsList.SelectedRows[0];
                var foundStudent = studentRow.DataBoundItem as Student;

                var studentInList = Students.Where(x => x.Id == foundStudent.Id)
                    .FirstOrDefault();

                _service.DeleteStudent(studentInList);
            }
        }

        private void dgStudentsList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgStudentsList.SelectedRows.Count > 0)
            {
                DataGridViewRow studentRow = dgStudentsList.SelectedRows[0];
                var foundStudent = studentRow.DataBoundItem as Student;
                SelectedStudent = foundStudent;

                btnUpdate.Enabled = true;
                btnAdd.Enabled = false;
                btnDelete.Enabled = true;
            } 
            else
            {
                btnUpdate.Enabled = false;
                btnAdd.Enabled = true;
                btnDelete.Enabled = false;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(SelectedStudent == null)
            {
                return;
            }

            var userInput = new UserInputWindow(_service, SelectedStudent);
            userInput.ShowDialog();
            RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
            //var student = Students.Where(x => x.Id == SelectedStudent.Id).FirstOrDefault();
            //if (student == null)
            //{
            //    return;
            //}

            //string name = txtName.Text;
            //string surname = txtSurname.Text;
            //DateTime dateOfBirth = dtBirthdate.Value;

            //if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || dateOfBirth < dtBirthdate.MinDate || dateOfBirth > dtBirthdate.MaxDate)
            //{
            //    MessageBox.Show("Please make sure that you filled all the fields properly!", "Input data is not correct!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //student.Name = name;
            //student.Surname = surname;
            //student.DateOfBirth = dateOfBirth;

            //UpdateStudent(student);

            //SaveStudents();
            //LoadStudents();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(connectionIsEstablished && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        private void dgStudentsList_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                HitTestInfo hitTestInfo = dgStudentsList.HitTest(e.X, e.Y);
                if(hitTestInfo.Type == DataGridViewHitTestType.None)
                {
                    dgStudentsList.ClearSelection();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
        }

        private void dtStartingDate_ValueChanged(object sender, EventArgs e)
        {
            RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
        }

        private void dtEndingDate_ValueChanged(object sender, EventArgs e)
        {
            RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
        }

        private void cbIncludeInactive_CheckedChanged(object sender, EventArgs e)
        {
            RefreshData(txtFilter.Text, dtStartingDate.Value.Date, dtEndingDate.Value.Date, cbIncludeInactive.Checked);
        }
    }
}
