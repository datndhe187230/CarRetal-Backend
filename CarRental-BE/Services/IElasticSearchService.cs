namespace CarRental_BE.Services
{
    public interface IElasticSearchService<T>
    {
        Task<string> CreateDocumentAsync(T document);
        Task<T> GetDocumentAsync(int id);

        Task<IEnumerable<T>> GetAllDocuments();
        Task<string> UpdateDocumentAsync(T document);
        Task<string> DeleteDocumentAsync(int id);
    }
}
