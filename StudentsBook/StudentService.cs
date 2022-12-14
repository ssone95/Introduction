using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentsBook.Helpers
{
    public class StudentService
    {
        private SqlConnection _connection;
        public StudentService(SqlConnection connection)
        {
            _connection = connection;
        }

        public List<Student> GetStudents(string filterString, DateTime startingDate, DateTime endingDate, bool includeInactive)
        {
            var students = new List<Student>();
            try
            {
                string searchQuery = "SELECT * FROM Students WHERE ";

                if (!string.IsNullOrEmpty(filterString))
                {
                    searchQuery += "(Name LIKE @FilterString OR Surname LIKE @FilterString OR UniqueNumber LIKE @FilterString) AND ";
                }

                searchQuery += "DateOfBirth BETWEEN @StartingDate AND @EndingDate ";

                if (!includeInactive)
                {
                    searchQuery += "AND Active = @Active";
                }

                using (var command = new SqlCommand(searchQuery, _connection))
                {
                    if (!string.IsNullOrEmpty(filterString))
                    {
                        command.Parameters.Add(new SqlParameter("@FilterString", "%" + filterString + "%"));
                    }
                    command.Parameters.Add(new SqlParameter("@StartingDate", startingDate));
                    command.Parameters.Add(new SqlParameter("@EndingDate", endingDate));

                    if (!includeInactive)
                    {
                        command.Parameters.Add(new SqlParameter("@Active", true));
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
                                    students.Add(student);
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
            return students;
        }

        public void CreateStudent(Student student)
        {
            try
            {
                if (student == null)
                {
                    throw new ArgumentNullException("Student is not valid!");
                }

                string sqlQuery = $"INSERT INTO Students (Id, UniqueNumber, Name, Surname, DateOfBirth, Active, UpdatedAt) VALUES (@Id, @UniqueNumber, @Name, @Surname, @DateOfBirth, @Active, @UpdatedAt);";

                using (var command = new SqlCommand(sqlQuery, _connection))
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
                    if (result < 1)
                    {
                        MessageBox.Show("Student was not added to the DB!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        public void UpdateStudent(Student student)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }


        public void DeleteStudent(Student student)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }
    }
}
