public class BookingVO
{
    public string BookingNumber { get; set; }
    public string CarName { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public int NumberOfDays { get; set; }             
    public long? BasePrice { get; set; }
    public long? Deposit { get; set; }
    public long? TotalPrice { get; set; }            
    public string? PickUpLocation { get; set; }
    public string? DropOffLocation { get; set; }
    public string? PaymentType { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CarImageUrl { get; set; }             
}
