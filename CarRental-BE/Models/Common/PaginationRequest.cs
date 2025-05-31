namespace CarRental_BE.Models.Common
{
    public class PaginationRequest
    {

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        //Validate

        // retrns the validated page number, ensuring it is at least 1
        public int ValidatePageNumber => PageNumber < 1 ? 1 : PageNumber;

        // returns the validated page size, ensuring it is between 1 and 100
        public int ValidatePageSize => PageSize < 1 ? 10 : PageSize > 100 ? 100 : PageSize;
    }
}
