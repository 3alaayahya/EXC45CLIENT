using Microsoft.AspNetCore.Mvc;
using Excersice1.BL;

namespace Excersice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User1> Get()
        {
            return User1.Read();
        }

        [HttpGet("getCourses/{id}")]
        public IEnumerable<Course> GetUserCourses(int id)
        {
            return User1.GetUserCourses(id);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public User1 Get(int id)
        {
            return User1.Read().FirstOrDefault(u => u.Id1 == id);
        }

        // POST api/<UserController>/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] User1 user)
        {
            if (user.Register(user))
            {
                return Ok(new { message = "Registration successful." });
            }
            else
            {
                return BadRequest(new { message = "Registration failed. Email already in use." });
            }
        }

        // POST api/<UserController>/login
        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = User1.Login(email, password);
            if (user != null)
            {
                return Ok(new { message = "Login successful.", user });
            }
            else
            {
                return Unauthorized(new { message = "Login failed. Invalid email or password." });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout(int userId)
        {
            User1.Logout(userId).Wait();
            return Ok(new { message = "Logout successfully.", userId });
        }

        // POST api/<CoursesController>
        [HttpPost("addCourseToUser/userId/{userId}/courseId/{courseId}")]
        public bool Post(int userId,int courseId)
        {
            return User1.AddMyCourse(userId,courseId);
        }

        [HttpGet("searchByDuration/userId/{userId}/durationFrom/{durationFrom}/DurationTo/{DurationTo}")] // this uses the QueryString
        public IEnumerable<Course> GetByDurationRange(int userId,double durationFrom, double DurationTo)
        {
            User1 user = new User1();
            return User1.GetByDurationRange(userId,durationFrom, DurationTo);

        }

        [HttpGet("searchByRating/userId/{userId}/ratingFrom/{ratingFrom}/ratingTo/{ratingTo}")]
        public IEnumerable<Course> GetByRatingRange(int userId, double ratingFrom, double ratingTo)
        {
            User1 user = new User1();
            return User1.GetByRatingRange(userId, ratingFrom, ratingTo);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("DeleteCourseByIdForUser/userId/{userId}/courseId/{courseId}")]
        public IActionResult Delete(int userId,int courseId)
        {
            try
            {
                User1.DeleteCourse(userId, courseId);
                return Ok(new { massage = "Course with id: " + courseId + " has been deleted successfully." }); // Return a descriptive message
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}