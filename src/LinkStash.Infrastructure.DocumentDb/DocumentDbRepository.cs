using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkStash.Core;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace LinkStash.Infrastructure.DocumentDb
{
    public class DocumentDbRepository<T> : IRepository<T> where T: Entity
    {
        private readonly IDocumentClient _client;
        private readonly string _databaseId;
        private readonly AsyncLazy<Database> _database;
        private AsyncLazy<DocumentCollection> _collection;

        public DocumentDbRepository(
            IDocumentClient client,
            string databaseId,
            string collectionName = null)
        {
            _client = client;
            _databaseId = databaseId;
            _database = new AsyncLazy<Database>(async () => await GetOrCreateDatabaseAsync());
            _collection = new AsyncLazy<DocumentCollection>(async () => await GetOrCreateCollectionAsync(collectionName ?? typeof(T).Name));
        }

        public async Task<IEnumerable<T>> GetAllAsync() // TODO: support maxitemcount (now defaults to 100)
        {
            return _client.CreateDocumentQuery<T>(await GetCollectionUriAsync())
                .Where(e => e.Type == typeof(T).Name)
                .AsEnumerable();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var doc = await GetDocumentByIdAsync(id);
            var retVal = (T)(dynamic)doc;
            if (retVal == null || retVal.Type != typeof(T).Name) return null;
            return retVal;
        }

        public async Task<T> GetByEtagAsync(string etag)
        {
            var doc = await GetDocumentByEtagAsync(etag);
            var retVal = (T)(dynamic)doc;
            if (retVal == null || retVal.Type != typeof(T).Name) return null;
            return retVal;
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression) // FAST
        {
            return
                _client.CreateDocumentQuery<T>(await GetCollectionUriAsync())
                    .Where(e => e.Type == typeof(T).Name)
                    .Where(expression)
                    .AsEnumerable()
                    .FirstOrDefault();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return _client.CreateDocumentQuery<T>(await GetCollectionUriAsync())
                .Where(e => e.Type == typeof(T).Name)
                .Where(expression)
                .AsEnumerable();
        }

        public async Task<T> AddOrUpdateAsync(T entity)
        {
            var doc = await _client.UpsertDocumentAsync((await _collection).SelfLink, entity);

            return JsonConvert.DeserializeObject<T>(doc.Resource.ToString());
        }

        public async Task<long> CountAsync()
        {
            return _client.CreateDocumentQuery<T>((await _collection).SelfLink)
                .Where(e => e.Type == typeof(T).Name)
                .AsEnumerable().LongCount();
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string collectionName)
        {
            DocumentCollection collection = _client.CreateDocumentCollectionQuery((await _database).SelfLink).Where(c => c.Id == collectionName).ToArray().FirstOrDefault();

            if (collection == null)
            {
                collection = new DocumentCollection { Id = collectionName };

                collection = await _client.CreateDocumentCollectionAsync((await _database).SelfLink, collection);
            }

            return collection;
        }

        private async Task<Database> GetOrCreateDatabaseAsync()
        {
            Database database = _client.CreateDatabaseQuery()
                .Where(db => db.Id == _databaseId).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await _client.CreateDatabaseAsync(
                    new Database { Id = _databaseId });
            }

            return database;
        }

        private async Task<string> GetCollectionUriAsync()
        {
            return (await _collection).DocumentsLink;
        }

        private async Task<Document> GetDocumentByIdAsync(object id)
        {
            return _client.CreateDocumentQuery<Document>((await _collection).SelfLink)
                .Where(d => d.Id == id.ToString()).AsEnumerable().FirstOrDefault();
        }

        private async Task<Document> GetDocumentByEtagAsync(string etag)
        {
            return _client.CreateDocumentQuery<Document>((await _collection).SelfLink)
                .Where(d => d.ETag == etag).AsEnumerable().FirstOrDefault();
        }
    }
}
