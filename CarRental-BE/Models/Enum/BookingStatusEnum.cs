using System.Runtime.Serialization;

namespace CarRental_BE.Models.Enum
{
    public enum BookingStatusEnum
    {
        /// <summary>
        /// Booking has been created but payment/deposit not yet confirmed
        /// </summary>
        pending_deposit,

        /// <summary>
        /// Deposit received, booking is confirmed
        /// </summary>
        confirmed,

        /// <summary>
        /// Rental period is currently active
        /// </summary>
        in_progress,

        /// <summary>
        /// Booking was cancelled by customer or admin
        /// </summary>
        cancelled,

        /// <summary>
        /// Final payment is pending (for pay-later bookings)
        /// </summary>
        pending_payment,

        /// <summary>
        /// Rental period has ended successfully
        /// </summary>
        completed,

        /// <summary>
        /// Waiting for confirmation (mapped to waiting_confirmed)
        /// </summary>
        waiting_confirmed,

        /// <summary>
        /// Booking is waiting for owner return confirmation (mapped to waiting_confirm_return)
        /// </summary>
        waiting_confirm_return,

        /// <summary>
        /// Booking is rejected return by owner (mapped to rejected_return)
        /// </summary>
        rejected_return
    }
}