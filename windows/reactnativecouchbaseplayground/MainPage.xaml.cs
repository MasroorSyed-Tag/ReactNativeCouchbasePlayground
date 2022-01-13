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

            // Get the database (and create it if it doesn't exist)
            var database = new Database("mydb");
            // Create a new document (i.e. a record) in the database
            string id = null;
            using (var mutableDoc = new MutableDocument())
            {
                mutableDoc.SetFloat("version", 2.0f)
                    .SetString("type", "SDK");

                // Save it to the database
                database.Save(mutableDoc);
                id = mutableDoc.Id;
            }

            // Update a document
            using (var doc = database.GetDocument(id))
            using (var mutableDoc = doc.ToMutable())
            {
                mutableDoc.SetString("language", "C#");
                database.Save(mutableDoc);

                using (var docAgain = database.GetDocument(id))
                {
                    Debug.WriteLine($"Document ID :: {docAgain.Id}");
                    Debug.WriteLine($"Learning {docAgain.GetString("language")}");
                }
            }

            // Create a query to fetch documents of type SDK
            // i.e. SELECT * FROM database WHERE type = "SDK"
            using (var query = QueryBuilder.Select(SelectResult.All())
                .From(DataSource.Database(database))
                .Where(Expression.Property("type").EqualTo(Expression.String("SDK"))))
            {
                // Run the query
                var result = query.Execute();
                Debug.WriteLine($"Number of rows :: {result.AllResults().Count}");
            }

            // Create replicator to push and pull changes to and from the cloud
            //var targetEndpoint = new URLEndpoint(new Uri("ws://localhost:4984/getting-started-db"));
            //var replConfig = new ReplicatorConfiguration(database, targetEndpoint);

            //// Add authentication
            //replConfig.Authenticator = new BasicAuthenticator("john", "pass");

            //// Create replicator (make sure to add an instance or static variable
            //// named _Replicator)
            //var _Replicator = new Replicator(replConfig);
            //_Replicator.AddChangeListener((sender, args) =>
            //{
            //    if (args.Status.Error != null)
            //    {
            //        Debug.WriteLine($"Error :: {args.Status.Error}");
            //    }
            //});

            //_Replicator.Start();

            // Later, stop and dispose the replicator *before* closing/disposing the
        }
    }
}
