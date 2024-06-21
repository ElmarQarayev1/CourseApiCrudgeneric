using System;
using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Implementations;
using Course.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController:ControllerBase
	{
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("")]
        public ActionResult Create(GroupCreateDto createDto)
        {
            return StatusCode(201, new { Id = _groupService.Create(createDto) });

        }
        [HttpGet("")]
        public ActionResult<PaginatedList<GroupGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            return StatusCode(200, _groupService.GetAllByPage(search, page, size));
        }
        [HttpGet("all")]
        public ActionResult<List<GroupGetDto>> GetAll()
        {
            return StatusCode(200, _groupService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<GroupDetailsDto> GetById(int id)
        {
            return StatusCode(200, _groupService.GetById(id));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, GroupUpdateDto updateDto)
        {


            _groupService.Update(id, updateDto);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _groupService.Delete(id);
            return NoContent();
        }


    

}
}

