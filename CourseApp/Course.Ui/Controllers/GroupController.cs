using System;
using Course.Ui.Resources;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Net.Http.Headers;
using Course.Ui.Filter;
using Course.Ui.Service;
using Course.Ui.Exceptions;
using System.Net;

namespace Course.Ui.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class GroupController:Controller
	{
        private readonly ICrudService _crudService;
        private readonly HttpClient _client;

        public GroupController(ICrudService crudService,HttpClient client)
        {
            _crudService = crudService;
            _client = client;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                return View(await _crudService.GetAllPaginated<GroupListItemGetResource>("groups", page));
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

        //public async Task<IActionResult> Index(int page = 1)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        using (var response = await client.GetAsync("https://localhost:7064/api/Groups?page=" + page + "&size=2"))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var bodyStr = await response.Content.ReadAsStringAsync();

        //                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        //                PaginatedResponseResource<GroupListItemGetResource> data = JsonSerializer.Deserialize<PaginatedResponseResource<GroupListItemGetResource>>(bodyStr, options);
        //                if (data.TotalPages < page) return RedirectToAction("index", new { page = data.TotalPages });
        //                return View(data);
        //            }
        //            else
        //            {
        //                return RedirectToAction("error", "home");
        //            }
        //        }
        //    }
        //}

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateRequest createRequest)
        {
            try
            {
                await _crudService.Create(createRequest, "groups");
                return RedirectToAction("index", "group");
            }
            catch (HttpException e)
            {
                if (e.Status == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else
                {
                    return RedirectToAction("error", "home");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                return View(await _crudService.Get<GroupCreateRequest>("groups/" + id));
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else if (e.Status == System.Net.HttpStatusCode.NotFound)
                    return RedirectToAction("error", "home", new { message = "Group not found" });

                else return RedirectToAction("error", "home");
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GroupCreateRequest editRequest, int id)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, token);

            if (!ModelState.IsValid) return View();

            var content = new StringContent(JsonSerializer.Serialize(editRequest), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PutAsync("https://localhost:7064/api/Groups/" + id, content))
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

            return View(editRequest);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crudService.Delete($"groups/{id}");
                return Ok(); 
            }
            catch (HttpException e)
            {
                if (e.Status == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("login", "auth");
                }
                else if (e.Status == System.Net.HttpStatusCode.NotFound)
                    return RedirectToAction("error", "home", new { message = "Group not found" });

                else return RedirectToAction("error", "home");
            }
            catch (Exception e)
            {
                return RedirectToAction("error", "home");
            }
        }

    }

}

