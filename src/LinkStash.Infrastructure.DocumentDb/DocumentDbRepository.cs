namespace LinkStash.Infrastructure.DocumentDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using LinkStash.Core;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;

    public class DocumentDbRepository<T> : IRepository<T>
        where T: Entity
    {
        private readonly IDocumentClient client;
        private readonly string databaseId;
        private readonly AsyncLazy<Database> database;
        private AsyncLazy<DocumentCollection> collection;

        public DocumentDbRepository(
            IDocumentClient client,
            string databaseId,
            string collectionName = null)
        {
            this.client = client;
            this.databaseId = databaseId;
            this.database = new AsyncLazy<Database>(async () => await this.GetOrCreateDatabaseAsync());
            this.collection = new AsyncLazy<DocumentCollection>(async () => await this.GetOrCreateCollectionAsync(collectionName ?? typeof(T).Name));
        }

        public async Task<IEnumerable<T>> GetAllAsync() // TODO: support maxitemcount (now defaults to 100)
        {
            return this.client.CreateDocumentQuery<T>(await this.GetCollectionUriAsync())
                .Where(e => e.Type == typeof(T).Name)
                .AsEnumerable();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var doc = await this.GetDocumentByIdAsync(id);
            var retVal = (T)(dynamic)doc;
            if (retVal == null || retVal.Type != typeof(T).Name)
            {
                return null;
            }

            return retVal;
        }

        public async Task<T> GetByEtagAsync(string etag)
        {
            var doc = await this.GetDocumentByEtagAsync(etag);
            var retVal = (T)(dynamic)doc;
            if (retVal == null || retVal.Type != typeof(T).Name)
            {
                return null;
            }

            return retVal;
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression) // FAST
        {
            return
                this.client.CreateDocumentQuery<T>(await this.GetCollectionUriAsync())
                    .Where(e => e.Type == typeof(T).Name)
                    .Where(expression)
                    .AsEnumerable()
                    .FirstOrDefault();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return this.client.CreateDocumentQuery<T>(await this.GetCollectionUriAsync())
                .Where(e => e.Type == typeof(T).Name)
                .Where(expression)
                .AsEnumerable();
        }

        public async Task<T> AddOrUpdateAsync(T entity)
        {
            var doc = await this.client.UpsertDocumentAsync((await this.collection).SelfLink, entity);

            return JsonConvert.DeserializeObject<T>(doc.Resource.ToString());
        }

        public async Task<long> CountAsync()
        {
            return this.client.CreateDocumentQuery<T>((await this.collection).SelfLink)
                .Where(e => e.Type == typeof(T).Name)
                .AsEnumerable().LongCount();
        }

        private async Task<DocumentCollection> GetOrCreateCollectionAsync(string collectionName)
        {
            DocumentCollection collection = this.client.CreateDocumentCollectionQuery((await this.database).SelfLink).Where(c => c.Id == collectionName).ToArray().FirstOrDefault();

            if (collection == null)
            {
                collection = new DocumentCollection { Id = collectionName };

                collection = await this.client.CreateDocumentCollectionAsync((await this.database).SelfLink, collection);
            }

            return collection;
        }

        private async Task<Database> GetOrCreateDatabaseAsync()
        {
            Database database = this.client.CreateDatabaseQuery()
                .Where(db => db.Id == this.databaseId).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await this.client.CreateDatabaseAsync(
                    new Database { Id = this.databaseId });
            }

            return database;
        }

        private async Task<string> GetCollectionUriAsync()
        {
            return (await this.collection).DocumentsLink;
        }

        private async Task<Document> GetDocumentByIdAsync(object id)
        {
            return this.client.CreateDocumentQuery<Document>((await this.collection).SelfLink)
                .Where(d => d.Id == id.ToString()).AsEnumerable().FirstOrDefault();
        }

        private async Task<Document> GetDocumentByEtagAsync(string etag)
        {
            return this.client.CreateDocumentQuery<Document>((await this.collection).SelfLink)
                .Where(d => d.ETag == etag).AsEnumerable().FirstOrDefault();
        }
    }
}
