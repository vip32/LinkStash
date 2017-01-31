using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
//using Microsoft.Azure.Documents.Client.TransientFaultHandling;


namespace LinkStash.Infrastructure.DocumentDb
{
    public interface IDocumentDbInitializer
    {
        IDocumentClient GetClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy = null);

        //IReliableReadWriteDocumentClient GetReliableClient(string endpointUrl, string authorizationKey, ConnectionPolicy connectionPolicy = null);
    }
}
