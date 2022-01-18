using System;
using System.Collections.Generic;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using ReactNativeCouchbasePlayground.Core.Interfaces;
using ReactNativeCouchbasePlayground.Core.Models;

namespace ReactNativeCouchbasePlayground.Core.Repositories
{
    public sealed class TodoItemRepository: BaseRepository, ITodoItemRepository
    {
        IQuery _itemQuery;

        public TodoItemRepository() : base("todos")
        { }

        public List<TodoItem> GetAllAsync()
        {
            var todoItems = new List<TodoItem>();

            try
            {
                var database = GetDatabase();

                if (database != null)
                {
                    _itemQuery = QueryBuilder.Select(SelectResult.All())
                        .From(DataSource.Database(database));
                }

                var resultSet = _itemQuery.Execute();
                foreach (var result in resultSet.AllResults())
                {
                    var dictionary = result.GetDictionary("Todos"); // <2>
                    if (dictionary != null)
                    {
                        var item = new TodoItem // <3>
                        {
                            Id = dictionary.GetString("Id"), // <4>
                            Description = dictionary.GetString("Description")
                        };
                        
                        todoItems.Add(item);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TodoItemRepository Exception: {ex.Message}");
            }
            
            return todoItems;
        }

        public TodoItem GetAsync(string id)
        {
            TodoItem todoItem = null;

            try
            {
                var database = GetDatabase();
                var document = database?.GetDocument(id);
                if (document != null)
                {
                    todoItem = new TodoItem()
                    {
                        Id = document.Id,
                        Description = document.GetString("Description")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TodoItemRepository Exception: {ex.Message}");
            }
            
            return todoItem;
        }

        public bool SaveAsync(TodoItem todoItem)
        {
            try
            {
                if (todoItem != null)
                {
                    var mutableDocument = new MutableDocument(todoItem.Id);
                    mutableDocument.SetString("Description", todoItem.Description);

                    var database = GetDatabase();

                    database.Save(mutableDocument);

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"TodoItemRepository Exception: {ex.Message}");
            }

            return false;
        }

        public void StartReplicationForCurrentUser()
        {
            base.StartReplicationAsync("alice",
                "Pass123$",
                new string[] { "admin_channel" });
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}