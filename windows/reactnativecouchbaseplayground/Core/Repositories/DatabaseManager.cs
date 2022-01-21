using System;
using System.Linq;
using Couchbase.Lite;
using Couchbase.Lite.DI;
using Couchbase.Lite.Sync;
using System.Diagnostics;

namespace reactnativecouchbaseplayground
{
    public class DatabaseManager : IDisposable
    {
        // Note: User 'localhost' when using a simulator
        private readonly Uri _remoteSyncUrl = new Uri("ws://localhost:4984");

        // Note: Use '10.0.2.2' when using an emulator
        //readonly Uri _remoteSyncUrl = new Uri("ws://10.0.2.2:4984");

        private readonly string _databaseName;
        private Replicator _replicator;
        private ListenerToken _replicatorListenerToken;
        private Database _database;

        public DatabaseManager(string databaseName)
        {
            _databaseName = databaseName;
        }

        public Database GetDatabase()
        {
            if (_database == null)
            {
                if (_databaseName == "todos")
                {
                    var defaultDirectory = Service.GetInstance<IDefaultDirectoryResolver>().DefaultDirectory();

                    Debug.WriteLine("=====hahahaha");
                    Debug.WriteLine(defaultDirectory);


                    // Todo Configure the location
                    var databaseConfig = new DatabaseConfiguration
                    {
                        Directory = defaultDirectory
                    };

                    _database = new Database(_databaseName, databaseConfig);
                }
            }

            return _database;
        }

        public void StartReplicationAsync(string username,
                                                string password,
                                                string[] channels,
                                                ReplicatorType replicationType = ReplicatorType.PushAndPull,
                                                bool continuous = true)
        {
            var database = GetDatabase();

            var targetUrlEndpoint = new URLEndpoint(new Uri(_remoteSyncUrl, _databaseName));

            var configuration = new ReplicatorConfiguration(database, targetUrlEndpoint) // <1>
            {
                ReplicatorType = replicationType, // <2>
                Continuous = continuous, // <3>
                Authenticator = new BasicAuthenticator(username, password), // <4>
                Channels = channels?.Select(x => $"channel.{x}").ToArray(), // <5>
                Heartbeat = new TimeSpan(0, 0, 1)

            };

            _replicator = new Replicator(configuration);

            _replicatorListenerToken = _replicator.AddChangeListener(OnReplicatorUpdate);

            _replicator.Start();
        }

        private void OnReplicatorUpdate(object sender, ReplicatorStatusChangedEventArgs e)
        {
            var status = e.Status;

            switch (status.Activity)
            {
                case ReplicatorActivityLevel.Busy:
                    Debug.WriteLine("Busy transferring data.");
                    break;
                case ReplicatorActivityLevel.Connecting:
                    Debug.WriteLine("Connecting to Sync Gateway.");
                    break;
                case ReplicatorActivityLevel.Idle:
                    Debug.WriteLine("Replicator in idle state.");
                    break;
                case ReplicatorActivityLevel.Offline:
                    Debug.WriteLine("Replicator in offline state.");
                    break;
                case ReplicatorActivityLevel.Stopped:
                    Debug.WriteLine("Completed syncing documents.");
                    break;
                default:
                    Debug.WriteLine("Bad Replicator Activity");
                    throw new ArgumentOutOfRangeException();
            }

            Debug.WriteLine(status.Progress.Completed == status.Progress.Total
                ? "All documents synced."
                : $"Documents {status.Progress.Total - status.Progress.Completed} still pending sync");
        }

        private void StopReplication()
        {
            _replicator.RemoveChangeListener(_replicatorListenerToken);
            _replicator.Stop();
        }


        public void Dispose()
        {
            if (_replicator != null)
            {
                StopReplication();
                // Because the 'Stop' method for a Replicator instance is asynchronous 
                // we must wait for the status activity to be stopped before closing the database. 
                while (true)
                {
                    if (_replicator.Status.Activity == ReplicatorActivityLevel.Stopped)
                    {
                        break;
                    }
                }
                _replicator.Dispose();
            }
            
            _database.Close();
            _database = null;
        }
    }
}
