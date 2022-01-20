using System;
using System.Collections.Generic;

namespace reactnativecouchbaseplayground
{
    public interface ITodoItemRepository: IDisposable
    {
        List<TodoItem> GetAllAsync();
        TodoItem GetAsync(string id);
        bool DeleteTodoItem(string id);
        bool SaveAsync(string id, string des);
        void StartReplicationForCurrentUser();
    }
}