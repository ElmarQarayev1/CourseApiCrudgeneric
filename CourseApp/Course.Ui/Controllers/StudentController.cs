using System;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Course.Ui.Exceptions;
using Course.Ui.Resources;
using Course.Ui.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
namespace Course.Ui.Controllers
{
	public class StudentController:Controller
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;
        public StudentController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }
        public async Task<IActionResult> Index(int page = 1, int size = 4)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<StudentListItemGetResponse>("students", page));
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<IActionResult> Create()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization,token);

            ViewBag.Groups = await getGroups();

            if (ViewBag.Groups == null) return RedirectToAction("error", "home");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] StudentCreateRequest createRequest)
         {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            MultipartFormDataContent content = new MultipartFormDataContent();

            var fileContent = new StreamContent(createRequest.FileName.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(createRequest.FileName.ContentType);

            content.Add(new StringContent(createRequest.FullName), "FullName");
            content.Add(new StringContent(createRequest.GroupId.ToString()), "GroupId");
            content.Add(new StringContent(createRequest.Email), "Email");
            content.Add(new StringContent(createRequest.BirthDate.ToLongDateString()), "BirthDate");
            content.Add(fileContent, "File", createRequest.FileName.FileName);

            using (var response = await _client.PostAsync("https://localhost:7064/api/students", content))
            {

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "account");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ViewBag.Groups = await getGroups();

                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

                    foreach (var item in errorResponse.Errors)
                    ModelState.AddModelError(item.Key, item.Message);
                    return View();
                }
                else
                {
                    TempData["Error"] = "Something went wrong!";
                }
            }
            return View(createRequest);

        }

        private async Task<List<StudentCreateWithGroupResponse>> getGroups()
        {
            using (var response = await _client.GetAsync("https://localhost:7064/api/groups/all"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<StudentCreateWithGroupResponse>>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);

            var response = await _client.GetAsync($"https://localhost:7064/api/students/{id}");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var student = JsonSerializer.Deserialize<StudentCreateRequest>(await response.Content.ReadAsStringAsync(), options);
                ViewBag.Groups = await getGroups();
                ViewBag.SelectedGroupId = student.GroupId;
                return View(student);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] StudentCreateRequest editRequest)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await getGroups();
                return View(editRequest);
            }
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(editRequest.FullName), "FullName");
                    content.Add(new StringContent(editRequest.Email), "Email");
                    content.Add(new StringContent(editRequest.BirthDate.ToString("o")), "BirthDate");
                    content.Add(new StreamContent(editRequest.FileName.OpenReadStream()), "FileName", editRequest.FileName.FileName);
                    content.Add(new StringContent(editRequest.GroupId.ToString()), "GroupId");

                    using (HttpResponseMessage response = await _client.PutAsync($"https://localhost:7064/api/Students/{id}", content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);

                            foreach (var item in errorResponse.Errors)
                                ModelState.AddModelError(item.Key, item.Message);

                            ViewBag.Groups = await getGroups();
                            return View(editRequest);
                        }
                        else
                        {
                            TempData["Error"] = "Something went wrong";
                            ViewBag.Groups = await getGroups();
                            return View(editRequest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Exception: {ex.Message}";
                ViewBag.Groups = await getGroups();
                return View(editRequest);
            }
        }


        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _client.DeleteAsync($"https://localhost:7064/api/students/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["Error"] = "Something went wrong";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Exception: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

    }
}

