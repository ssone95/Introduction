using StudentsBook.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentsBook
{
    public partial class UserInputWindow : Form
    {
        private Student student;
        private StudentService _service;
        public UserInputWindow(StudentService service)
        {
            InitializeComponent();
            student = new Student();
            _service = service;
        }

        public UserInputWindow(StudentService service, Student student)
        {
            InitializeComponent();
            _service = service;

            this.student = student;

            if (this.student == null)
            {
                this.student = new Student();
                MessageBox.Show("You somehow didn't provide the selected student, user input will be blank!");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string uniqueNumber = txtUniqueNumber.Text;
            string name = txtName.Text;
            string surname = txtSurname.Text;
            DateTime birthdate = dtBirthdate.Value;

            if (string.IsNullOrEmpty(uniqueNumber) || uniqueNumber.Length > 7 || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Please make sure that you filled all the fields properly!", "Input data is not correct!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            student.UniqueNumber = uniqueNumber;
            student.Name = name;
            student.Surname = surname;
            student.DateOfBirth = birthdate;

            if (student.Id == Guid.Empty)
            {
                student.Id = Guid.NewGuid();
                student.Active = true;
                student.UpdatedAt = DateTime.Now;
                _service.CreateStudent(student);
            }
            else
            {
                _service.UpdateStudent(student);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        


        private void UserInputWindow_Load(object sender, EventArgs e)
        {
            txtUniqueNumber.Text = student.UniqueNumber;
            txtName.Text = student.Name;
            txtSurname.Text = student.Surname;
            dtBirthdate.Value = student.DateOfBirth;
        }
    }
}
