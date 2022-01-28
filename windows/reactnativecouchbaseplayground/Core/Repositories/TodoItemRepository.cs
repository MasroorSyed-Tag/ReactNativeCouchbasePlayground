using System;
using System.Collections.Generic;
using System.Diagnostics;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Microsoft.ReactNative.Managed;

namespace reactnativecouchbaseplayground
{
    [ReactModule]
    class TodoItemRepository: BaseRepository, ITodoItemRepository
    {
        IQuery _itemQuery;

        public TodoItemRepository() : base("todos")
        { }

        [ReactMethod("getTodos")]
        public List<TodoItem> GetAllAsync()
        {
            var todoItems = new List<TodoItem>();

            try
            {
                var database = GetDatabase();

                if (database != null)
                {
                    _itemQuery = QueryBuilder.Select(
                        SelectResult.Expression(Meta.ID),
                        SelectResult.All())
                        .From(DataSource.Database(database));
                }

                var resultSet = _itemQuery.Execute();
                var results = resultSet.AllResults();
                // Debug.WriteLine("Count");
                // Debug.WriteLine(results.Count);
                // Debug.WriteLine("----------------------------------------------");
                // Debug.WriteLine(resultSet.AllResults().Count);
                Debug.WriteLine("----------------------------------------------");

                foreach (var result in results)
                {
                    //Debug.WriteLine(result.ToJSON());
                    var dictionary = result.GetDictionary("todos"); // <2>

                    Debug.WriteLine(result.GetString("id"));
                    Debug.WriteLine(dictionary.GetString("Description"));

                    //Debug.WriteLine(dictionary);
                    if (dictionary != null)
                    {
                        var item = new TodoItem // <3>
                        {
                            Id = result.GetString("id"), // <4>
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

            // Debug.WriteLine("returning");
            todoItems.ForEach(Console.WriteLine);


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

        [ReactMethod("deleteTodos")]

        public bool DeleteTodoItem(string id)
        {
            bool delStatus = false;
            try {
                var database = GetDatabase();
                Debug.WriteLine("Deleting a document with id:\t" + id);
                var document = database?.GetDocument(id);
                Debug.WriteLine(document);
                database.Delete(document);
                delStatus = true;

            } catch (CouchbaseLiteException e) {
                
            }
            return delStatus;
        }

        [ReactMethod("save")]
        public bool SaveAsync(string id, string des)
        {
            Debug.WriteLine("Inside Save");
            var todoItem = new TodoItem
            {
                Id = id,
                Description = des
            };
            try
            {
                if (todoItem != null)
                {
                    var mutableDocument = new MutableDocument(todoItem.Id);
                    mutableDocument.SetString("Description", todoItem.Description);

                    var database = GetDatabase();
                    Debug.WriteLine("The database");
                    Debug.WriteLine(database);

                    database.Save(mutableDocument);
                    Debug.WriteLine("Saved");

                    SaveEvent(true);

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"TodoItemRepository Exception: {ex.Message}");
            }

            return false;
        }

        [ReactEvent]
        public Action<bool> SaveEvent { get; set; }

        public void StartReplicationForCurrentUser()
        {
            base.StartReplicationAsync("admin",
                "Decisive2021.",
                new string[] { "admin_channel" });
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}