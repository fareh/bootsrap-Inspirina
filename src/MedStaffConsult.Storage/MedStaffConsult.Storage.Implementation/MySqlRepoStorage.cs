using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Dommel;
using MedStaffConsult.Storage.Abstraction;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MedStaffConsult.Storage.Implementation
{
    namespace Front.Core.Storage.KeyDocument
    {
        public class CustomTableNameResolver : DommelMapper.ITableNameResolver
        {
            public string ResolveTableName(Type type) => $"{type.Name}Table";
        }

        public class MySqlRepoStorage<TEntity> : IRepoStorage<TEntity> where TEntity : AbstractEntity
        {
            private const int EventId = 1;
            private readonly ILogger _logger;
            private readonly string _connectionString;
            private readonly string _schema;
            private readonly string _tableName;

            static MySqlRepoStorage()
            {
                SqlMapperExtensions.TableNameMapper = type => $"{type.Name}Table";
                DommelMapper.SetTableNameResolver(new CustomTableNameResolver());
            }

            public MySqlRepoStorage(ILogger logger, string connectionString, string schema, string tableName = null)
            {
                _logger = logger;
                _connectionString = connectionString;
                _schema = schema;
                _tableName = string.IsNullOrWhiteSpace(tableName) ? $"{typeof(TEntity).Name}Table" : tableName;
            }

            private string GetFullQualifiedTableName() => $@"{_schema}.{_tableName}";

            public virtual async Task<TEntity> Get(int uid)
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
                        return await cnx.QueryFirstOrDefaultAsync<TEntity>(query, param, commandType: CommandType.Text);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get {_tableName} uid : {uid}");
                    throw;
                }
            }

            public virtual async Task<IEnumerable<TEntity>> Get(IEnumerable<int> uids)
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
                        return await cnx.QueryAsync<TEntity>(query, param);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get {_tableName} uids : {keys}");
                    throw;
                }
            }

            public virtual async Task<IEnumerable<TEntity>> GetAll()
            {
                try
                {
                    var query = $@"SELECT * FROM {GetFullQualifiedTableName()};";
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        return await cnx.QueryAsync<TEntity>(query);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to get all {_tableName} keys ");
                    throw;
                }
            }

            public virtual async Task Remove(int uid)
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
                        await cnx.ExecuteAsync(query, param);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to remove  {_tableName} key {uid}");
                    throw;
                }
            }

            public virtual async Task Set(TEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                try
                {
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        await SqlMapperExtensions.InsertAsync(cnx, entity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to add {_tableName} key {entity.UId}");
                    throw;
                }
            }

            public virtual async Task Set(IEnumerable<TEntity> entities)
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
                        await SqlMapperExtensions.InsertAsync(cnx, list);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to add {_tableName} keys", list.Select(p => p.UId));
                    throw;
                }
            }

            public virtual async Task Update(TEntity entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                try
                {
                    using (var cnx = new MySqlConnection(_connectionString))
                    {
                        cnx.Open();
                        await SqlMapperExtensions.UpdateAsync(cnx, entity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to add {_tableName} key {entity.UId}");
                    throw;
                }
            }

            public virtual async Task Update(IEnumerable<TEntity> entities)
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
                        await SqlMapperExtensions.UpdateAsync(cnx, list);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to add {_tableName} keys", list.Select(p => p.UId));
                    throw;
                }
            }

            public virtual async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate)
            {
                try
                {
                    using (var cn = new MySqlConnection(_connectionString))
                    {
                        cn.Open();
                        return await cn.SelectAsync(predicate);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(EventId, ex, $"failed to Find {_tableName} ");
                    throw;
                }
            }
        }
    }
}

