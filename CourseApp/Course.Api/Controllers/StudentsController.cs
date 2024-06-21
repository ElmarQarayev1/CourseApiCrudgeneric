using System;
using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Implementations;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController:ControllerBase
	{
		private readonly IStudentService _studentService;

		public StudentsController(IStudentService studentService)
		{
			_studentService = studentService;
		}

        [HttpPost("")]
        public ActionResult Create([FromForm] StudentCreateDto createDto)
        {
                
                return StatusCode(201, new { id = _studentService.Create(createDto) });
              
        }

        [HttpGet("all")]
        public ActionResult<List<StudentGetDto>> GetAll()
        {
            return StatusCode(200, _studentService.GetAll());
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<StudentGetDto>> GetAll(int page = 1, int size = 10)
        {
            return Ok(_studentService.GetAllPaginated(page, size));
        }
        [HttpGet("{id}")]
        public ActionResult<StudentDetailsDto> GetById(int id)
        {
            return StatusCode(200, _studentService.GetById(id));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromForm] StudentUpdateDto updateDto)
        {


            _studentService.Update(id, updateDto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _studentService.Delete(id);
            return NoContent();
        }

    }
}

