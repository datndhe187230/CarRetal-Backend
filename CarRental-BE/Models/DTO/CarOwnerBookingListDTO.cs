using CarRental_BE.Models.Enum;

namespace CarRental_BE.Models.DTO
{
    public class CarOwnerBookingListDTO
    {
        public Guid AccountId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? CarName { get; set; }
        public List<string>? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        // Normalize pagination and sorting defaults
        public void Normalize()
        {
            Page = Page < 1 ? 1 : Page;
            PageSize = PageSize < 1 ? 10 : PageSize > 100 ? 100 : PageSize;
            SortBy = string.IsNullOrWhiteSpace(SortBy) ? null : SortBy.Trim();
            SortDirection = string.IsNullOrWhiteSpace(SortDirection) ? "desc" : SortDirection.Trim().ToLower();
        }

        // Centralized validation for query params
        public bool TryValidate(out object errorPayload)
        {
            // accountId required
            if (AccountId == Guid.Empty)
            {
                errorPayload = new { code = 400, message = "accountId is required" };
                return false;
            }

            // date range
            if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
            {
                errorPayload = new { code = 400, message = "fromDate must be <= toDate" };
                return false;
            }

            // sortBy
            if (!string.IsNullOrEmpty(SortBy))
            {
                var allowedSortBy = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                { "pickupDate", "returnDate", "totalAmount", "status" };
                if (!allowedSortBy.Contains(SortBy))
                {
                    errorPayload = new
                    {
                        code = 400,
                        message = "Invalid sortBy value.",
                        errors = new { sortBy = "Allowed values: pickupDate, returnDate, totalAmount, status" }
                    };
                    return false;
                }
            }

            // statuses
            if (Status != null && Status.Any())
            {
                var allowedStatuses = new HashSet<string>(System.Enum.GetNames(typeof(BookingStatusEnum)).Select(s => s.ToLower()))
                {
                };
                foreach (var s in Status)
                {
                    if (!allowedStatuses.Contains((s ?? string.Empty).ToLower()))
                    {
                        errorPayload = new
                        {
                            code = 400,
                            message = $"Invalid status value: {s}",
                            errors = new { status = "Allowed values: confirmed, in_progress, pending_payment, pending_deposit, completed, cancelled" }
                        };
                        return false;
                    }
                }
            }

            errorPayload = null!;
            return true;
        }
    }
}
