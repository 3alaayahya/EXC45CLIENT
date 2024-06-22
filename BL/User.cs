using System.Collections.Generic;

namespace Excersice1.BL
{
    public class User1
    {
        int Id;
        string Name;
        string Email;
        string Password;
        bool IsAdmin;
        bool IsActive;

        public User1() { }

        public User1(int id, string name, string email, string password, bool isAdmin, bool isActive)
        {
            Id1 = id;
            Name1 = name;
            Email1 = email;
            Password1 = password;
            IsAdmin1 = isAdmin;
            IsActive1 = isActive;
        }

        // Method to insert a user into the database
        public bool Insert()
        {
            DBservices dbs = new DBservices();
            return dbs.InsertUser(Id1, Name1, Email1, Password1, IsAdmin1, IsActive1).Result;
        }

        // Method to retrieve all users from the database
        public static List<User1> Read()
        {
            DBservices dbs = new DBservices();
            return dbs.ReadUsers().Result;
        }

        // Method to add a course to the user in the database
        public static bool AddMyCourse(int userID, int courseID)
        {
            DBservices dbs = new DBservices();
            try
            {
                return dbs.AddCourseToUser(userID, courseID).Result;
            }
            catch
            {
                return false;
            }
        }

        // Method to get courses by duration range from the database
        public static List<Course> GetByDurationRange(int userId, double duration1, double duration2)
        {
            DBservices dbs = new DBservices();
            return dbs.GetCoursesByDurationRange(userId, duration1, duration2).Result;
        }

        // Method to get courses by rating range from the database
        public static List<Course> GetByRatingRange(int userId, double ratingFrom, double ratingTo)
        {
            DBservices dbs = new DBservices();
            return dbs.GetCoursesByRatingRange(userId, ratingFrom, ratingTo).Result;
        }

        // Method to delete a course from the user in the database
        public static void DeleteCourse(int userID, int courseID)
        {
            DBservices dbs = new DBservices();
            dbs.DeleteCourseFromUser(userID, courseID).Wait();
        }

        // Method to get courses by user from the database
        public static List<Course> GetUserCourses(int userID)
        {
            DBservices dbs = new DBservices();
            return dbs.GetCoursesByUser(userID).Result;
        }

        // Method to authenticate a user based on email and password
        public static User1 Login(string email, string password)
        {
            DBservices dbs = new DBservices();
            User1 user = dbs.LoginUser(email, password).Result;
            if (user != null)
            {
                user.IsActive1 = true;
                // Update user's activity status in the database
                // This might involve an update operation in a real-world scenario
            }
            return user;
        }

        // Call this method when a user logs out
        public static async Task Logout(int userId)
        {
            DBservices dbs = new DBservices();
            await dbs.LogoutUser(userId);
        }

        // Method to register a new user
        public bool Register(User1 user1)
        {
            DBservices dbs = new DBservices();
            // Call the stored procedure to register the user and return the result
            return dbs.RegisterUser(user1.Id, user1.Name, user1.Email1, user1.Password1).Result;
        }

        public int Id1 { get => Id; set => Id = value; }
        public string Name1 { get => Name; set => Name = value; }
        public string Email1 { get => Email; set => Email = value; }
        public string Password1 { get => Password; set => Password = value; }
        public bool IsAdmin1 { get => IsAdmin; set => IsAdmin = value; }
        public bool IsActive1 { get => IsActive; set => IsActive = value; }
    }
}
