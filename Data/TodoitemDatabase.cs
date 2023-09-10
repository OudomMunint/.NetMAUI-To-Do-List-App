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

        public Task<List<Todoitem>> GetItemsNotDoneAysnc()
        {
            return Database.QueryAsync<Todoitem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public Task<Todoitem> GetItemAsync(int id)
        {
            return Database.Table<Todoitem>().Where(i => i.ID == id).FirstOrDefaultAsync();
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