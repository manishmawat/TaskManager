using Microsoft.Azure.Cosmos;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public class CosmosTaskRepository : ITaskRepository
    {
        private readonly Container _container;

        public CosmosTaskRepository(Container container)
        {
            _container = container;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<TaskItem>("SELECT * FROM c");
            var results = new List<TaskItem>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<TaskItem?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<TaskItem>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            var response = await _container.CreateItemAsync(task, new PartitionKey(task.id));
            return response.Resource;
        }

        public async Task<bool> UpdateAsync(string id, TaskItem task)
        {
            try
            {
                await _container.ReplaceItemAsync(task, id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<TaskItem>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
