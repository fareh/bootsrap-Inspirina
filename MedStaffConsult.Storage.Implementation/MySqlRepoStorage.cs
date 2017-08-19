using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MedStaffConsult.Storage.Abstraction;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MedStaffConsult.Storage.Implementation
{
    namespace Front.Core.Storage.KeyDocument
    {
        public class MySqlRepoStorage<TEntity> : IRepoStorage<TEntity> where TEntity : AbstractEntity
        {
            private const int EventId = 1;
            private readonly ILogger _logger;
            private readonly string _connectionString;
            private readonly string _schema;
            private readonly string _tableName;
            private readonly string _entityName = typeof(TEntity).Name;

            public MySqlRepoStorage(ILogger logger, string connectionString, string schema, string tableName = null)
            {
                _logger = logger;
                _connectionString = connectionString;
                _schema = schema;
                _tableName = string.IsNullOrWhiteSpace(tableName) ? $"{_entityName}Table" : tableName;
            }

            private string GetFullQualifiedTableName() => $@"{_schema}.{_tableName}";

            public virtual TEntity Get(int uid)
            {
                try
                {
                    var query = $@"SELECT * FROM {GetFullQualifiedTableName()} WHERE UId = @uid;";
                    var param = new
                    {
                        uid
                    };
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        return cnx.QueryFirstOrDefault<TEntity>(query, param, commandType: CommandType.Text);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get {_entityName} uid : {uid}");
                    throw;
                }
            }

            public virtual List<TEntity> Get(IEnumerable<int> uids)
            {
                var keys = string.Join(",", uids);
                try
                {
                    var query = $@"SELECT * FROM {GetFullQualifiedTableName()} WHERE UId IN (@uids);";
                    var param = new
                    {
                        uids = keys
                    };
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        return cnx.Query<TEntity>(query, param).AsList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get {_entityName} uids : {keys}");
                    throw;
                }
            }

            public virtual List<TEntity> GetAll()
            {
                try
                {
                    var query = $@"SELECT * FROM {GetFullQualifiedTableName()};";
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        return cnx.Query<TEntity>(query).AsList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get all {_entityName} keys ");
                    throw;
                }
            }

            public virtual void Remove(int uid)
            {
                try
                {
                    var query = $@"DELETE FROM {GetFullQualifiedTableName()} WHERE uid = @uid;";

                    var param = new
                    {
                        uid
                    };

                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        cnx.Execute(query, param);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to remove  {_entityName} key {uid}");
                    throw;
                }
            }

            public virtual void Set(TEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                string query = $@"
INSERT INTO {GetFullQualifiedTableName()} 

           ";

                //"INSERT INTO table (id, name, age) VALUES(1, "A", 19) ON DUPLICATE KEY UPDATE    
                //name = "A", age = 19"
                try
                {
                    var param = new
                    {
                    };

                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        cnx.Execute(query, param);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"NPG failed to add {_entityName} key {entity.UId}");
                    throw;
                }
            }

            public virtual void Set(IEnumerable<TEntity> entities)
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                var list = entities.ToList();
                if (!list.Any())
                {
                    return;
                }

                try
                {
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();

                        var param = new
                        {

                        };

                        string query = $@" INSERT INTO {GetFullQualifiedTableName()} 
                                        ";
                        cnx.Execute(query, param);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to add {_entityName} keys", list.Select(p => p.UId));
                    throw;
                }
            }
        }
    }
}
