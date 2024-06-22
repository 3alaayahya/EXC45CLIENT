using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;
using Excersice1.BL;
using Microsoft.Extensions.Configuration;

public class DBservices
{
    private readonly string connectionString;

    public DBservices()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();
        connectionString = configuration.GetConnectionString("myProjDB");
    }

    private SqlConnection Connect()
    {
        SqlConnection con = new SqlConnection(connectionString);
        con.Open();
        return con;
    }

    public async Task<List<dynamic>> GetTopCourses()
    {
        List<dynamic> topCourses = new List<dynamic>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetTopCourses", con);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                dynamic courseInfo = new ExpandoObject();
                courseInfo.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                courseInfo.Name = reader.GetString(reader.GetOrdinal("Name"));
                courseInfo.Rating = reader.GetDouble(reader.GetOrdinal("Rating"));
                courseInfo.NumberOfUsers = reader.GetInt32(reader.GetOrdinal("NumberOfUsers"));
                topCourses.Add(courseInfo);
            }
        }

        return topCourses;
    }

    public async Task<bool> UpdateCourseIsActive(int courseId, bool isActive)
    {
        int rowsAffected;
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("UpdateCourseIsActive", con);
            cmd.Parameters.AddWithValue("@id", courseId);
            cmd.Parameters.AddWithValue("@isActive", isActive);

            rowsAffected = await cmd.ExecuteNonQueryAsync();
        }

        return rowsAffected == 0;
    }

    // Insert User
    public async Task<bool> InsertUser(int id, string name, string email, string password, bool isAdmin, bool isActive)
    {
        int rowsAffected;
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("InsertUser", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@isAdmin", isAdmin);
            cmd.Parameters.AddWithValue("@isActive", isActive);

            rowsAffected = await cmd.ExecuteNonQueryAsync();
        }

        // If any rows were affected, consider it as successful insertion
        return rowsAffected > 0;
    }

    public async Task<List<Course>> GetCoursesByInstructor(int instructorID)
    {
        List<Course> coursesList = new List<Course>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetCoursesByInstructor", con);
            cmd.Parameters.AddWithValue("@instructorID", instructorID);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive"))  
                };
                coursesList.Add(course);
            }
        }

        return coursesList;
    }

    // Read Users
    public async Task<List<User1>> ReadUsers()
    {
        List<User1> usersList = new List<User1>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("ReadUsers", con);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                User1 user = new User1
                {
                    Id1 = reader.GetInt32(reader.GetOrdinal("id")),
                    Name1 = reader.GetString(reader.GetOrdinal("name")),
                    Email1 = reader.GetString(reader.GetOrdinal("email")),
                    Password1 = reader.GetString(reader.GetOrdinal("password")),
                    IsAdmin1 = reader.GetBoolean(reader.GetOrdinal("isAdmin")),
                    IsActive1 = reader.GetBoolean(reader.GetOrdinal("isActive"))
                };
                usersList.Add(user);
            }
        }

        return usersList;
    }

    // Login User
    public async Task<User1> LoginUser(string email, string password)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("LoginUser", con);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (reader.Read())
            {
                User1 user = new User1
                {
                    Id1 = reader.GetInt32(reader.GetOrdinal("id")),
                    Name1 = reader.GetString(reader.GetOrdinal("name")),
                    Email1 = reader.GetString(reader.GetOrdinal("email")),
                    Password1 = reader.GetString(reader.GetOrdinal("password")),
                    IsAdmin1 = reader.GetBoolean(reader.GetOrdinal("isAdmin")),
                    IsActive1 = true // Set user as active upon login
                };
                return user;
            }

            return null;
        }
    }

    // Logout User
    public async Task<bool> LogoutUser(int userId)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("LogoutUser", con);
            cmd.Parameters.AddWithValue("@userId", userId);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    // Register User
    public async Task<bool> RegisterUser(int id, string name, string email, string password)
    {
        int rowsAffected;
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("RegisterUser", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            rowsAffected = await cmd.ExecuteNonQueryAsync();
        }

        // If any rows were affected, consider it as successful registration
        return rowsAffected > 0;
    }


    // Add Course to User
    public async Task<bool> AddCourseToUser(int userID, int courseID)
    {
        int rowsAffected;
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("AddCourseToUser", con);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@courseID", courseID);

            rowsAffected = await cmd.ExecuteNonQueryAsync();
        }

        // If any rows were affected, consider it as successful addition
        return rowsAffected > 0;
    }


    // Get Courses by Duration Range
    public async Task<List<Course>> GetCoursesByDurationRange(int userId, double duration1, double duration2)
    {
        List<Course> coursesList = new List<Course>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetCoursesByDurationRange", con);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@duration1", duration1);
            cmd.Parameters.AddWithValue("@duration2", duration2);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive"))
                };
                coursesList.Add(course);
            }
        }

        return coursesList;
    }

    // Get Courses by Rating Range
    public async Task<List<Course>> GetCoursesByRatingRange(int userId, double ratingFrom, double ratingTo)
    {
        List<Course> coursesList = new List<Course>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetCoursesByRatingRange", con);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@ratingFrom", ratingFrom);
            cmd.Parameters.AddWithValue("@ratingTo", ratingTo);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")) 
                };
                coursesList.Add(course);
            }
        }

        return coursesList;
    }

    // Delete Course from User
    public async Task<int> DeleteCourseFromUser(int userID, int courseID)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("DeleteCourseFromUser", con);
            cmd.Parameters.AddWithValue("@userID", userID);
            cmd.Parameters.AddWithValue("@courseID", courseID);

            return await cmd.ExecuteNonQueryAsync();
        }
    }

    // Get Courses by User
    public async Task<List<Course>> GetCoursesByUser(int userID)
    {
        List<Course> coursesList = new List<Course>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetCoursesByUser", con);
            cmd.Parameters.AddWithValue("@userID", userID);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")) 
                };
                coursesList.Add(course);
            }
        }

        return coursesList;
    }

    public async Task<int> InsertInstructor(Instructor instructor)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("InsertInstructor", con);
            cmd.Parameters.AddWithValue("@id", instructor.Id);
            cmd.Parameters.AddWithValue("@title", instructor.Title);
            cmd.Parameters.AddWithValue("@displayName", instructor.Name);
            cmd.Parameters.AddWithValue("@jobTitle", instructor.JobTitle);
            cmd.Parameters.AddWithValue("@image", instructor.Image);

            return await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Instructor>> ReadInstructors()
    {
        List<Instructor> instructorsList = new List<Instructor>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("ReadInstructors", con);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (reader.Read())
            {
                Instructor instructor = new Instructor
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title = reader.GetString(reader.GetOrdinal("title")),
                    Name = reader.GetString(reader.GetOrdinal("displayName")),
                    JobTitle = reader.GetString(reader.GetOrdinal("job_title")),
                    Image = reader.GetString(reader.GetOrdinal("image"))
                };
                instructorsList.Add(instructor);
            }
        }

        return instructorsList;
    }

    public async Task<Instructor> GetInstructorById(int id)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetInstructorById", con);
            cmd.Parameters.AddWithValue("@id", id);

            // Assuming your stored procedure returns a SqlDataReader
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    // Map SqlDataReader to Instructor object
                    return new Instructor
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Title = reader.GetString(reader.GetOrdinal("title")),
                        Name = reader.GetString(reader.GetOrdinal("displayName")),
                        JobTitle = reader.GetString(reader.GetOrdinal("job_title")),
                        Image = reader.GetString(reader.GetOrdinal("image"))
                    };
                }
                else
                {
                    // Handle case where no instructor found with the given id
                    return null; // or throw an exception as needed
                }
            }
        }
    }


    public async Task<int> InsertCourse(string title, double rating, int numberOfReviews, double duration, int instructorsId, string URL, string imageReference)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = new SqlCommand("InsertCourse", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameters required by the stored procedure
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@rating", rating);
            cmd.Parameters.AddWithValue("@numberOfReviews", numberOfReviews);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@instructorsId", instructorsId);
            cmd.Parameters.AddWithValue("@URL", URL);
            cmd.Parameters.AddWithValue("@imageReference", imageReference);

            // Execute the command asynchronously
            return await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> UpdateCourse(Course updatedCourse)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("UpdateCourse", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", updatedCourse.Id);
            cmd.Parameters.AddWithValue("@title", updatedCourse.Title1);
            cmd.Parameters.AddWithValue("@rating", updatedCourse.Rating1);
            cmd.Parameters.AddWithValue("@numberOfReviews", updatedCourse.NumberOfReviews1);
            cmd.Parameters.AddWithValue("@duration", updatedCourse.Duration1);
            cmd.Parameters.AddWithValue("@instructorsId", updatedCourse.InstructorsId1);
            cmd.Parameters.AddWithValue("@URL", updatedCourse.URL1);
            cmd.Parameters.AddWithValue("@imageReference", updatedCourse.ImageReference1);

            return await cmd.ExecuteNonQueryAsync();
        }
    }

    public void DeleteCourseById(int id)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("DeleteCourseById", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public List<Course> ReadCourses()
    {
        List<Course> coursesList = new List<Course>();

        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("ReadCourses", con);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive"))
                };
                coursesList.Add(course);
            }
        }

        return coursesList;
    }

    public Course GetCourseById(int id)
    {
        using (SqlConnection con = Connect())
        {
            SqlCommand cmd = CreateCommandWithStoredProcedure("GetCourseById", con);
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Course course = new Course
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Title1 = reader.GetString(reader.GetOrdinal("title")),
                    URL1 = reader.GetString(reader.GetOrdinal("url")),
                    Rating1 = reader.GetDouble(reader.GetOrdinal("rating")),
                    NumberOfReviews1 = reader.GetInt32(reader.GetOrdinal("num_reviews")),
                    LastUpdate1 = reader.GetDateTime(reader.GetOrdinal("last_update_date")).ToString(),
                    InstructorsId1 = reader.GetInt32(reader.GetOrdinal("instructors_id")),
                    ImageReference1 = reader.GetString(reader.GetOrdinal("image")),
                    Duration1 = reader.GetDouble(reader.GetOrdinal("duration")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("isActive"))
                };
                return course;
            }

            return null;
        }
    }

    private SqlCommand CreateCommandWithStoredProcedure(string spName, SqlConnection con)
    {
        SqlCommand cmd = new SqlCommand
        {
            Connection = con,
            CommandText = spName,
            CommandTimeout = 10,
            CommandType = CommandType.StoredProcedure
        };

        return cmd;
    }
}
