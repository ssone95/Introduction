using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsBook
{
    public class Student
    {
        public Guid Id { get; set; }
        public string UniqueNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Active { get; set; }
        public DateTime UpdatedAt { get; set; }



        public Student(Guid id, string uniqueNumber, string name, string surname, DateTime dateOfBirth, bool active, DateTime updatedAt)
        {
            Id = id;
            UniqueNumber = uniqueNumber;
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            Active = active;
            UpdatedAt = updatedAt;
        }
    }
}
