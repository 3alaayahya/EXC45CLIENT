namespace Excersice1.BL
{
    public class Instructor
    {
        int id;
        string title;
        string name;
        string image;
        string jobTitle;
        static List<Instructor> instructorsList = new List<Instructor>();
        private static readonly SemaphoreSlim listLock = new SemaphoreSlim(1, 1);

        public Instructor() { }

        public Instructor(int id, string title, string name, string image, string jobTitle)
        {
            Id = id;
            Title = title;
            Name = name;
            Image = image;
            JobTitle = jobTitle;
        }
        public async Task<bool> Insert()
        {
            try
            {
                return await new DBservices().InsertInstructor(this) > 0;
            }
            catch (Exception ex)
            {
                // Handle exception
                return false;
            }
        }

                // New method to get courses by instructor
        public async Task<List<Course>> GetCourses()
        {
            try
            {
                return await new DBservices().GetCoursesByInstructor(Id);
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Course>();
            }
        }

        public static async Task<List<Instructor>> Read()
        {
            try
            {
                return await new DBservices().ReadInstructors();
            }
            catch (Exception ex)
            {
                // Handle exception
                return new List<Instructor>();
            }
        }

        public static async Task<Instructor> GetInstructorById(int id)
        {
            try
            {
                return await new DBservices().GetInstructorById(id);
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }
        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Name { get => name; set => name = value; }
        public string Image { get => image; set => image = value; }
        public string JobTitle { get => jobTitle; set => jobTitle = value; }
    }
}
