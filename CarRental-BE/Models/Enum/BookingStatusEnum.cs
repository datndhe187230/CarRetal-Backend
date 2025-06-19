namespace CarRental_BE.Models.Enum
{
    public enum BookingStatusEnum
    {
        /// <summary>
        /// Booking has been created but payment/deposit not yet confirmed
        /// </summary>
        PendingDeposit,

        /// <summary>
        /// Deposit received, booking is confirmed
        /// </summary>
        Confirmed,

        /// <summary>
        /// Rental period is currently active
        /// </summary>
        InProgress,

        /// <summary>
        /// Booking was cancelled by customer or admin
        /// </summary>
        Cancelled,

        /// <summary>
        /// Final payment is pending (for pay-later bookings)
        /// </summary>
        PendingPayment,

        /// <summary>
        /// Rental period has ended successfully
        /// </summary>
        Completed
    }
}
