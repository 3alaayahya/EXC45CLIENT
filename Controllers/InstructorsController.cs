using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Excersice1.BL;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Excersice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        // GET: api/<InstructorsController>
        [HttpGet]
        public async Task<IEnumerable<Instructor>> Get()
        {
            return await Instructor.Read();
        }

        [HttpGet("{id}/courses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses(int id)
        {
            Instructor instructor = new Instructor { Id = id };
            List<Course> courses = await instructor.GetCourses();

            if (courses == null || courses.Count == 0)
            {
                return NotFound("No courses found for this instructor.");
            }

            return Ok(courses);
        }

        // GET api/<InstructorsController>/5
        [HttpGet("{id}")]
        public Task<Instructor> Get(int id)
        {
            Instructor instructor = new Instructor();
            return Instructor.GetInstructorById(id);
        }

        // POST api/<InstructorsController>
        [HttpPost]
        public async Task<bool> Post([FromBody] Instructor instructor)
        {
            return await instructor.Insert();
        }

        // PUT api/<InstructorsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InstructorsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
