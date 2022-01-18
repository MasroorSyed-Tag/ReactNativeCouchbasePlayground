using System;
using Couchbase.Lite;

namespace reactnativecouchbaseplayground
{
    public abstract class BaseRepository : IDisposable
    {
        private readonly string _databaseName;
        private DatabaseManager _databaseManager;

        protected BaseRepository(string databaseName)
        {
            _databaseName = databaseName;
        }

        private DatabaseManager DatabaseManager
        {
            get
            {
                if (_databaseManager == null) _databaseManager = new DatabaseManager(_databaseName);

                return _databaseManager;
            }
        }

        public virtual void Dispose()
        {
            DatabaseManager?.Dispose();
        }

        protected virtual Database GetDatabase()
        {
            return DatabaseManager?.GetDatabase();
        }

        protected virtual void StartReplicationAsync(string username,
            string password,
            string[] channels)
        {
            DatabaseManager?.StartReplicationAsync(username, password, channels);
        }
    }
}