using System;
using System.Collections.Generic;
using ReactNativeCouchbasePlayground.Core.Models;

namespace ReactNativeCouchbasePlayground.Core.Interfaces
{
    public interface ITodoItemRepository: IDisposable
    {
        List<TodoItem> GetAllAsync();
        TodoItem GetAsync(string id);
        bool SaveAsync(TodoItem todoItem);
        void StartReplicationForCurrentUser();
    }
}