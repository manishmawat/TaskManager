using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

        public TaskService(HttpClient httpClient, string? baseUrl = null)
        {
            _httpClient = httpClient;
            _baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:5041/tasks" : baseUrl;
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TaskItem>>(_baseUrl);
        }

        public async Task<TaskItem> GetTaskAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<TaskItem>($"{_baseUrl}/{id}");
        }

        public async Task CreateTaskAsync(TaskItem task)
        {
            await _httpClient.PostAsJsonAsync(_baseUrl, task);
        }

        public async Task UpdateTaskAsync(string id, TaskItem task)
        {
            await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", task);
        }

        public async Task DeleteTaskAsync(string id)
        {
            await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
        }

        public async Task PatchTaskStatusAsync(string id, TaskManager.Web.Models.TaskStatus status)
        {
            var url = $"{_baseUrl}/{id}/status?status={(int)status}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            await _httpClient.SendAsync(request);
        }
    }
}
