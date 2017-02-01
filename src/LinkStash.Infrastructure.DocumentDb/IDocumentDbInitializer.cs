//using Microsoft.Azure.Documents.Client.TransientFaultHandling;

namespace LinkStash.Infrastructure.DocumentDb
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    public interface IDocumentDbInitializer
    {
        IDocumentClient GetClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy = null);

        //IReliableReadWriteDocumentClient GetReliableClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy = null);
    }
}
