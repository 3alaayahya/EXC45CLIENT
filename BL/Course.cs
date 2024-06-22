using Excersice1.BL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Excersice1.BL
{
    public class Course
    {
        int id;
        string title;
        string URL;
        double rating;
        int numberOfReviews;
        int instructorsId;
        string imageReference;
        double duration;
        string lastUpdate;
        bool isActive;

        public Course() { }

        public Course(string title, int id, double rating, int numberOfReviews, string lastUpdate, double duration, int instructorsId, string URL, string imageReference, bool isActive)
        {
            Id = id;
            Title1 = title;
            URL1 = URL;
            Rating1 = rating;
            NumberOfReviews1 = numberOfReviews;
            InstructorsId1 = instructorsId;
            ImageReference1 = imageReference;
            Duration1 = duration;
            LastUpdate1 = lastUpdate;
            this.isActive = isActive;
        }

        public static async Task<List<dynamic>> GetTopCourses()
        {
            DBservices dbServices = new DBservices();
            return await dbServices.GetTopCourses(); 
        }

        public static async Task<bool> UpdateCourseIsActive(int courseId, bool isActive)
        {
            DBservices dbServices = new DBservices();
            return await dbServices.UpdateCourseIsActive(courseId, isActive);
        }

        public async Task<bool> Insert()
        {
            DBservices dbServices = new DBservices();

            // Call the DBservices method to insert the course
            int rowsAffected = await dbServices.InsertCourse(Title1, Rating1, NumberOfReviews1, Duration1, InstructorsId1, URL1, ImageReference1);

            // Return true if insertion was successful (rowsAffected > 0)
            return rowsAffected > 0;
        }

        public async Task<bool> Update(Course updatedCourse)
        {
            DBservices dbServices = new DBservices();
            int result = await dbServices.UpdateCourse(updatedCourse);
            return result > 0;
        }

        public static void DeleteById(int id)
        {
            DBservices dbServices = new DBservices();
            dbServices.DeleteCourseById(id);
        }

        public static async Task<List<Course>> Read()
        {
            DBservices dbServices = new DBservices();
            return await Task.FromResult(dbServices.ReadCourses());
        }

        public static Course GetCourseById(int id)
        {
            DBservices dbServices = new DBservices();
            return dbServices.GetCourseById(id);
        }

        public int Id { get => id; set => id = value; }
        public string Title1 { get => title; set => title = value; }
        public string URL1 { get => URL; set => URL = value; }
        public double Rating1 { get => rating; set => rating = value; }
        public int NumberOfReviews1 { get => numberOfReviews; set => numberOfReviews = value; }
        public int InstructorsId1 { get => instructorsId; set => instructorsId = value; }
        public double Duration1 { get => duration; set => duration = value; }
        public string LastUpdate1 { get => lastUpdate; set => lastUpdate = value; }
        public string ImageReference1 { get => imageReference; set => imageReference =  value; }
        public bool IsActive { get => isActive ; set => isActive = value; }
    }
}
