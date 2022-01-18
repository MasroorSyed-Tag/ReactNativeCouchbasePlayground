using Microsoft.ReactNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if !USE_WINUI3
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

using Couchbase.Lite;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;
using System.Diagnostics;
using ReactNativeCouchbasePlayground.Core.Interfaces;
using ReactNativeCouchbasePlayground.Core.Repositories;
using ReactNativeCouchbasePlayground.Core.Models;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace reactnativecouchbaseplayground
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            var app = Application.Current as App;
            reactRootView.ReactNativeHost = app.Host;

            //// Get the database (and create it if it doesn't exist)
            TodoItemRepository todoItemRepository = new TodoItemRepository();

            Debug.WriteLine("==================================");
            Debug.WriteLine("Saving");
            // Create a new document (i.e. a record) in the database
            todoItemRepository.SaveAsync(
                new TodoItem
                {
                    Id = "abc",
                    Description = "Description"

                });

            Debug.WriteLine("Hopefully Saved");
            Debug.WriteLine("==================================");

            Debug.WriteLine("Getting A Single Todo");
            var singleTodo = todoItemRepository.GetAsync("abc");
            Debug.WriteLine(singleTodo.Id);
            Debug.WriteLine(singleTodo.Description);
            Debug.WriteLine("==================================");

            Debug.WriteLine("getting all todo");
            var alltodo = todoItemRepository.GetAllAsync();

            foreach (var todo in alltodo)
            {
                Debug.WriteLine(todo.Id);
                Debug.WriteLine(todo.Description);
                Debug.WriteLine("hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
            }


            todoItemRepository.StartReplicationForCurrentUser();

        }
    }
}
