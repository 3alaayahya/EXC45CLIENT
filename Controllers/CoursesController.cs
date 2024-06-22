using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Excersice1.BL;
using static System.Net.Mime.MediaTypeNames;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Excersice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IEnumerable<Course>> Get()
        {
            return await Course.Read();
        }

        // GET api/<CoursesController>/5
        [HttpGet("getCourseById/{id}")]
        public Course Get(int id)
        {
            return Course.GetCourseById(id);
        }

        // GET: api/courses/topCourses
        [HttpGet("GetTop5Courses")]
        public async Task<IEnumerable<dynamic>> GetTopCourses()
        {
            return await Course.GetTopCourses();
        }

        // PUT api/courses/updateIsActive/{id}
        [HttpPut("updateIsActive/{id}")]
        public async Task<IActionResult> UpdateCourseIsActive(int id, [FromBody] bool isActive)
        {
            try
            {
                Course.UpdateCourseIsActive(id, isActive);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate response
                return StatusCode(500, $"Error updating course isActive status: {ex.Message}");
            }
        }

        // POST api/<CoursesController>
        [HttpPost]
        public async Task<IActionResult> Post(int id, [FromForm] Course course, [FromForm] List<IFormFile> files)
        {
            List<string> imageLinks = new List<string>();

            string path = System.IO.Directory.GetCurrentDirectory();

            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(path, "uploadedFiles/" + formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    imageLinks.Add(formFile.FileName);
                }
            }

            if (imageLinks.Any())
            {
                string url = "https://proj.ruppin.ac.il/cgroup88/test2/tar1/Images/";
                course.ImageReference1 = url + string.Join(",", imageLinks);
            }
            try
            {
                await course.Insert();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to insert course: {ex.Message}");
            }
        }

        // PUT api/<CoursesController>/5
        [HttpPut("update/{id}")]
        public async Task<bool> Put(int id, [FromForm] Course updatedCourse, [FromForm] List<IFormFile> files)
        {
            List<string> imageLinks = new List<string>();
            string path = System.IO.Directory.GetCurrentDirectory();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(path, "uploadedFiles/" + formFile.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    imageLinks.Add(formFile.FileName);
                }
            }

            if (imageLinks.Any())
            {
                string url = "https://proj.ruppin.ac.il/cgroup88/test2/tar1/Images/";
                updatedCourse.ImageReference1 = url + string.Join(",", imageLinks);
            }

            if (await updatedCourse.Update(updatedCourse))
            {
                return true;
            }

            return false;
        }



        // POST api/<CoursesController>/uploadImage/5
        [HttpPost("uploadImage/{id}")]
        public async Task<bool> UploadImage(int id, IFormFile image)
        {
            try
            {
                var courses = await Course.Read();
                var course = courses.FirstOrDefault(c => c.Id == id);
                if (course == null)
                {
                    return false;
                }

                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        var imageBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(imageBytes);
                        var dataUrl = $"data:{image.ContentType};base64,{base64String}";

                        // Update the image reference with the Data URL of the uploaded image
                        course.ImageReference1 = dataUrl;
                        course.Update(course);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                //Console.Error.WriteLine($"Error uploading image: {ex.Message}");
                return false;
            }
        }




        // DELETE api/<CoursesController>/5
        [HttpDelete("DeleteById/id/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Course.DeleteById(id);
                return Ok(new { massage = "Course with id: " + id + " has been deleted successfully." }); // Return a descriptive message
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
