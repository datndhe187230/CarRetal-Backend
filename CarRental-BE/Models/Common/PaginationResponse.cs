namespace CarRental_BE.Models.Common
{
    //using Generic type T to allow for different data types in the response
    public class PaginationResponse<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; }

        // Constructor to initialize the pagination response with data and pagination details
        public PaginationResponse(List<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            Pagination = new PaginationMetadata(pageNumber, pageSize, totalRecords);
        }
    }


    public class PaginationMetadata
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public PaginationMetadata()
        {
            PageNumber = 1;
            PageSize = 10;
            TotalRecords = 0;
            TotalPages = 1;
            HasPreviousPage = false;
            HasNextPage = false;
        }

        public PaginationMetadata(int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            HasPreviousPage = pageNumber > 1;
            HasNextPage = pageNumber < TotalPages;
        }
    }
}
