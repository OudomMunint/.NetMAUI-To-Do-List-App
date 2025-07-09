using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListApp.Models;

namespace ToDoListApp.Data
{
    public class TodoitemDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<TodoitemDatabase> Instance =
            new AsyncLazy<TodoitemDatabase>(async () =>
            {
                var instance = new TodoitemDatabase();
                try
                {
                    CreateTableResult result = await Database.CreateTableAsync<Todoitem>();
                }
                catch (Exception)
                {
                    throw;
                }
                return instance;
            });

        public TodoitemDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<Todoitem>> GetItemsAysnc()
        {
            return Database.Table<Todoitem>().ToListAsync();
        }

        public Task<List<Todoitem>> GetItemsPinnedAysnc()
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [IsPinned] = 1");
        }

        public async Task<int> GetPinnedCountAysnc(bool pinned)
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [IsPinned] = ?", pinned);
            return items.Count;
        }

        public Task<List<Todoitem>> GetItemsNotDoneAysnc()
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public Task<List<Todoitem>> GetItemsDoneAsync()
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Done] = 1");
        }

        public Task<List<Todoitem>> GetItemsByPriorityAsync(string priority)
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Priority] = '" + priority + "'");
        }

        public async Task<int> GetItemCountByPriority(string priority)
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Priority] = '" + priority + "'");
            return items.Count;
        }

        public Task<List<Todoitem>> GetItemsByDateAsync(DateTime date)
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Date] = '" + date + "'");
        }

        public Task<List<Todoitem>> GetItemsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Date] BETWEEN '" + startDate + "' AND '" + endDate + "'");
        }

        public async Task<long> GetAttachmentsSize()
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Attachment] IS NOT NULL AND LENGTH([Attachment]) > 0");
            return items.Sum(item => (long)(item.Attachment?.Length ?? 0));
        }

        public async Task<long> GetItemAttachmentSizeById(int id)
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [ID] = ?", id);
            if (items.Count > 0 && items[0].Attachment != null)
            {
                return items[0].Attachment.Length;
            }
            return 0;
        }

        public async Task<int> GetItemAttachmentStatus(bool hasAttachment)
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [HasAttachment] = ?", hasAttachment);
            return items.Count;
        }

        public async Task<int> GetItemAttachmentStatus()
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Attachment] IS NOT NULL AND LENGTH([Attachment]) > 0");
            return items.Count;
        }

        public async Task<int> GetTotalItems()
        {
            var items = await Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem]");
            return items.Count();
        }

        public Task<Todoitem> GetAllPinnedItemsAsync()
        {
            return Database.Table<Todoitem>().Where(i => i.IsPinned == true).FirstOrDefaultAsync();
        }

        public Task<Todoitem> GetItemAsync(int id)
        {
            return Database.Table<Todoitem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<List<Todoitem>> RefreshDataAsync()
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem]");
        }

        public Task<int> SaveItemAsync(Todoitem item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Todoitem item)
        {
            return Database.DeleteAsync(item);
        }
    }
}