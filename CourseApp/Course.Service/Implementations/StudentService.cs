using System;
using Course.Core.Entities;
using Course.Data;
using Course.Service.Interfaces;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Course.Data.Repostories.Interfaces;
using Course.Service.Dtos.GroupDtos;
using Microsoft.AspNetCore.Mvc;
using Course.Service.Helpers;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Course.Service.Dtos;
using Azure.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Course.Service.Implementations
{
	public class StudentService:IStudentService
	{
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _env;
        public StudentService(IGroupRepository groupRepository,IStudentRepository studentRepository, IWebHostEnvironment env,IMapper mapper)
        {
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
            _env = env;
            _mapper = mapper;

        }
        public int Create([FromForm] StudentCreateDto createDto)
        {
            
            Group group = _groupRepository.Get(x => x.Id == createDto.GroupId, "Students");
            if (group == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given Id");
            }

            if (group.Limit <= group.Students.Count)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full!");
            }

            if (_studentRepository.Exists(x => x.Email.ToUpper() == createDto.Email.ToUpper() && !x.IsDeleted))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Student already exists by given Email");
            }

            Student student = new Student
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                BirthDate = createDto.Birthdate,
                GroupId = createDto.GroupId,
               FileName = FileManager.Save(createDto.File, _env.WebRootPath, "uploads/students")
        };
            _studentRepository.Add(student);
            _studentRepository.Save();

            return student.Id;
        }
        public void Delete(int id)
        {
            Student entity = _studentRepository.Get(x => x.Id == id);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Student not found");

            _studentRepository.Delete(entity);
            _groupRepository.Save();
        }
        public List<StudentGetDto> GetAll(string? search = null)
        {
            var students = _studentRepository.GetAll(x => search == null || x.FullName.Contains(search)).ToList();
           return  _mapper.Map<List<StudentGetDto>>(students);      
        }

        public PaginatedList<StudentGetDto> GetAllPaginated(int page = 1, int size = 10)
        {
            var query = _studentRepository.GetAll(x => !x.IsDeleted);

            PaginatedList<Student> students = PaginatedList<Student>.Create(query, page, size);

            return new PaginatedList<StudentGetDto>(_mapper.Map<List<StudentGetDto>>(students.Items), students.TotalPages, students.PageIndex, students.PageSize);
        }

        public StudentDetailsDto GetById(int id)
        {
            Student student = _studentRepository.Get(x => x.Id == id && !x.IsDeleted,"Group");

            if (student == null) throw new RestException(StatusCodes.Status404NotFound, "Student not found");

            return _mapper.Map<StudentDetailsDto>(student);
           

        }

        public void Update(int id, StudentUpdateDto studentUpdate)
        {
            
            var existingStudent = _studentRepository.Get(x => x.Id == id);
            if (existingStudent == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Student not found by given Id");
            }

            string deletedFile = null;

            Group group = _groupRepository.Get(x => x.Id == studentUpdate.GroupId, "Students");
            if (group == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given Id");
            }

            if (group.Limit <= group.Students.Count)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full!");
            }

            deletedFile = existingStudent.FileName;

            existingStudent.FileName = FileManager.Save(studentUpdate.File, _env.WebRootPath, "uploads/students");

            existingStudent.FullName = studentUpdate.FullName;
            existingStudent.Email = studentUpdate.Email;
            existingStudent.BirthDate = studentUpdate.Birthdate;
           
            existingStudent.GroupId = studentUpdate.GroupId;
            existingStudent.ModifiedAt = DateTime.Now;   
            _studentRepository.Save();

            if (deletedFile != null)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/students", deletedFile);
            }

        }
       


    }
}
//public class GenericService<T>
//{
//    private readonly HttpClient _client;
//    private readonly JsonSerializerOptions _serializerOptions;

//    public GenericService(HttpClient client)
//    {
//        _client = client;
//        _serializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
//    }

//    public async Task<T> GetAsync(string uri)
//    {
//        var response = await _client.GetAsync(uri);
//        response.EnsureSuccessStatusCode();
//        var responseBody = await response.Content.ReadAsStringAsync();
//        return JsonSerializer.Deserialize<T>(responseBody, _serializerOptions);
//    }

//    public async Task<PaginatedResponseResource<T>> GetPaginatedAsync(string uri)
//    {
//        var response = await _client.GetAsync(uri);
//        response.EnsureSuccessStatusCode();
//        var responseBody = await response.Content.ReadAsStringAsync();
//        return JsonSerializer.Deserialize<PaginatedResponseResource<T>>(responseBody, _serializerOptions);
//    }

//    public async Task<HttpResponseMessage> PostAsync(string uri, T item)
//    {
//        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
//        return await _client.PostAsync(uri, content);
//    }

//    public async Task<HttpResponseMessage> PutAsync(string uri, T item)
//    {
//        var content = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
//        return await _client.PutAsync(uri, content);
//    }

//    public async Task<HttpResponseMessage> DeleteAsync(string uri)
//    {
//        return await _client.DeleteAsync(uri);
//    }
//}

//using System.Threading.Tasks;
//using Course.Ui.Resources;
//using Course.Ui.Service;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Net.Http.Headers;
//using Course.Ui.Filter;

//namespace Course.Ui.Controllers
//{
//    [ServiceFilter(typeof(AuthFilter))]
//    public class GroupController : Controller
//    {
//        private readonly ICrudService _crudService;

//        public GroupController(ICrudService crudService)
//        {
//            _crudService = crudService;
//        }

//        public async Task<IActionResult> Index(int page = 1)
//        {
//            var data = await _crudService.GetAllPaginated<GroupListItemGetResource>(page, "https://localhost:7064/api/Groups");
//            if (data.TotalPages < page) return RedirectToAction("Index", new { page = data.TotalPages });
//            return View(data);
//        }

//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(GroupCreateRequest createRequest)
//        {
//            var token = Request.Cookies["token"];
//            if (string.IsNullOrEmpty(token))
//            {
//                return RedirectToAction("Login", "Account");
//            }
//            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);

//            if (!ModelState.IsValid) return View();

//            try
//            {
//                await _crudService.Create(createRequest, "https://localhost:7064/api/Groups");
//                return RedirectToAction("Index");
//            }
//            catch (HttpRequestException)
//            {
//                TempData["Error"] = "Something went wrong";
//                return View(createRequest);
//            }
//        }

//        public async Task<IActionResult> Edit(int id)
//        {
//            var token = Request.Cookies["token"];
//            if (string.IsNullOrEmpty(token))
//            {
//                return RedirectToAction("Login", "Account");
//            }
//            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);

//            try
//            {
//                var request = await _crudService.Get<GroupCreateRequest>($"https://localhost:7064/api/Groups/{id}");
//                return View(request);
//            }
//            catch (HttpRequestException)
//            {
//                TempData["Error"] = "Something went wrong!";
//                return RedirectToAction("Index");
//            }
//        }

//        [HttpPost



