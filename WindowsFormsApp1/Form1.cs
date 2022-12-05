using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class User
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgUsers.DataSource = new List<User>()
            {
                new User
                {
                    Name = "AAA",
                    Surname = "BBB",
                    DateOfBirth = "1.1.1990",
                }
            };


            dgUsers.DataSource = new List<User>()
            {
                new User
                {
                    Name = "CCC",
                    Surname = "DDD",
                    DateOfBirth = "1.1.1990",
                }
            };
        }
    }
}
