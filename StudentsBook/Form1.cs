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
        public Form1()
        {
            InitializeComponent();

            Students = new List<Student>();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string surname = txtSurname.Text;
                DateTime birthdate = dtBirthdate.Value;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                {
                    MessageBox.Show("Please make sure that you filled all the fields properly!", "Input data is not correct!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var student = new Student(Guid.NewGuid(), "1112022", name, surname, birthdate, true, DateTime.Now);
                //Students.Add(student);

                CreateStudent(student);

                //SaveStudents();
                //LoadStudents();
            } 
            catch(Exception ex)
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

                RefreshData(txtFilter.Text);
            }
            catch(Exception ex)
            {
                connectionIsEstablished = false;
                MessageBox.Show($"{ex}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CreateStudent(Student student)
        {
            try
            {
                if(student == null)
                {
                    throw new ArgumentNullException("Student is not valid!");
                }

                string sqlQuery = $"INSERT INTO Students (Id, UniqueNumber, Name, Surname, DateOfBirth, Active, UpdatedAt) VALUES (@Id, @UniqueNumber, @Name, @Surname, @DateOfBirth, @Active, @UpdatedAt);";

                using(var command = new SqlCommand(sqlQuery, _connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", student.Id));
                    command.Parameters.Add(new SqlParameter("@UniqueNumber", student.UniqueNumber));
                    command.Parameters.Add(new SqlParameter("@Name", student.Name));
                    command.Parameters.Add(new SqlParameter("@Surname", student.Surname));
                    command.Parameters.Add(new SqlParameter("@DateOfBirth", student.DateOfBirth));
                    command.Parameters.Add(new SqlParameter("@Active", student.Active));
                    command.Parameters.Add(new SqlParameter("@UpdatedAt", student.UpdatedAt));

                    int result = command.ExecuteNonQuery();

                    Console.WriteLine($"Insert completed, number of rows changed: {result}");
                    if(result < 1)
                    {
                        MessageBox.Show("Student was not added to the DB!");
                    }
                }
                RefreshData(txtFilter.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        void DeleteStudent(Student student)
        {
            try
            {
                if (student == null)
                {
                    throw new ArgumentNullException("Student is not valid!");
                }

                student.Active = false;
                student.UpdatedAt = DateTime.Now;

                string sqlQuery = $"UPDATE Students SET Active = @Active, UpdatedAt = @UpdatedAt WHERE Id = @Id;";

                using (var command = new SqlCommand(sqlQuery, _connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", student.Id));
                    command.Parameters.Add(new SqlParameter("@Active", student.Active));
                    command.Parameters.Add(new SqlParameter("@UpdatedAt", student.UpdatedAt));

                    int result = command.ExecuteNonQuery();

                    Console.WriteLine($"Delete completed, number of rows changed: {result}");
                    if (result < 1)
                    {
                        MessageBox.Show("Student was not deactivated in the DB!");
                    }
                }
                RefreshData(txtFilter.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        void UpdateStudent(Student student)
        {
            try
            {
                if (student == null)
                {
                    throw new ArgumentNullException("Student is not valid!");
                }

                student.UpdatedAt = DateTime.Now;

                string sqlQuery = $"UPDATE Students SET Name = @Name, Surname = @Surname, DateOfBirth = @DateOfBirth, UpdatedAt = @UpdatedAt WHERE Id = @Id;";

                using (var command = new SqlCommand(sqlQuery, _connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", student.Id));
                    command.Parameters.Add(new SqlParameter("@Name", student.Name));
                    command.Parameters.Add(new SqlParameter("@Surname", student.Surname));
                    command.Parameters.Add(new SqlParameter("@DateOfBirth", student.DateOfBirth));
                    command.Parameters.Add(new SqlParameter("@UpdatedAt", student.UpdatedAt));

                    int result = command.ExecuteNonQuery();

                    Console.WriteLine($"Delete completed, number of rows changed: {result}");
                    if (result < 1)
                    {
                        MessageBox.Show("Student was not updated in the DB!");
                    }
                }
                RefreshData(txtFilter.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
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

        void RefreshData(string filterString)
        {
            Students = new List<Student>();
            try
            {
                string searchQuery = "SELECT * FROM Students;";

                if(!string.IsNullOrEmpty(filterString))
                {
                    searchQuery = "SELECT * FROM Students WHERE Name LIKE @FilterString OR Surname LIKE @FilterString OR UniqueNumber LIKE @FilterString;";
                }
                using (var command = new SqlCommand(searchQuery, _connection))
                {
                    if (!string.IsNullOrEmpty(filterString))
                    {
                        command.Parameters.Add(new SqlParameter("@FilterString", "%" + filterString + "%"));
                    }
                    using (var reader = command.ExecuteReader())
                    {

                        if (reader.HasRows == false)
                        {
                            MessageBox.Show($"No data found in the Students table!");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    var id = reader.GetGuid(0);
                                    var uniqueNumber = string.Empty;

                                    if (!reader.IsDBNull(1))
                                    {
                                        uniqueNumber = reader.GetString(1);
                                    }
                                    var name = reader.GetString(2);
                                    var surname = reader.GetString(3);
                                    var dateOfBirth = reader.GetDateTime(4);
                                    var active = reader.GetBoolean(5);
                                    var lastUpdatedAt = reader.GetDateTime(6);

                                    var student = new Student(id, uniqueNumber, name, surname, dateOfBirth, active, lastUpdatedAt);
                                    Students.Add(student);
                                }
                                catch (Exception ex2)
                                {
                                    MessageBox.Show($"{ex2}", "An unexpected error has occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }

                            }
                        }
                    }
                }
                
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

                DeleteStudent(studentInList);
            }
        }

        private void dgStudentsList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgStudentsList.SelectedRows.Count > 0)
            {
                DataGridViewRow studentRow = dgStudentsList.SelectedRows[0];
                var foundStudent = studentRow.DataBoundItem as Student;
                SelectedStudent = foundStudent;


                txtName.Text = SelectedStudent.Name;
                txtSurname.Text = SelectedStudent.Surname;
                dtBirthdate.Value = SelectedStudent.DateOfBirth;
                btnUpdate.Enabled = true;
                btnAdd.Enabled = false;
                btnDelete.Enabled = true;
            } 
            else
            {
                txtName.Text = string.Empty;
                txtSurname.Text = string.Empty;
                dtBirthdate.Value = dtBirthdate.MinDate;
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
            var student = Students.Where(x => x.Id == SelectedStudent.Id).FirstOrDefault();
            if (student == null)
            {
                return;
            }

            string name = txtName.Text;
            string surname = txtSurname.Text;
            DateTime dateOfBirth = dtBirthdate.Value;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || dateOfBirth < dtBirthdate.MinDate || dateOfBirth > dtBirthdate.MaxDate)
            {
                MessageBox.Show("Please make sure that you filled all the fields properly!", "Input data is not correct!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            student.Name = name;
            student.Surname = surname;
            student.DateOfBirth = dateOfBirth;

            UpdateStudent(student);

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
            RefreshData(txtFilter.Text);
        }
    }
}
