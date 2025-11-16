using System;
using System.Collections.Generic;

namespace CarRental_BE.Helpers
{
    // Centralizes booking status constants and allowed transitions
    public static class BookingStatusFlow
    {
        public const string WaitingConfirm = "waiting_confirmed";
        public const string PendingDeposited = "pending_deposit";
        public const string PendingPayment = "pending_payment";
        public const string Confirmed = "confirmed";
        public const string InProgress = "in_progress";
        public const string WaitingConfirmReturn = "waiting_confirm_return";
        public const string RejectedReturn = "rejected_return";
        public const string Completed = "completed";
        public const string Cancelled = "cancelled";

        // Allowed transitions map including cancellation rules
        public static readonly Dictionary<string, string[]> AllowedTransitions = new()
        {
            { WaitingConfirm, new[] { PendingDeposited, Confirmed, Cancelled } },
            { PendingDeposited, new[] { Confirmed, Cancelled } },
            { Confirmed, new[] { InProgress } },
            { InProgress, new[] { WaitingConfirmReturn, PendingPayment } },
            { PendingPayment, new[] { WaitingConfirmReturn } },
            { WaitingConfirmReturn, new[] { Completed, RejectedReturn } },
            { RejectedReturn, new[] { WaitingConfirmReturn } },
            { Completed, Array.Empty<string>() },
            { Cancelled, Array.Empty<string>() }
        };

        public static bool IsValidTransition(string? current, string target)
        {
            if (current == null) return false;
            if (!AllowedTransitions.TryGetValue(current, out var allowed)) return false;
            foreach (var s in allowed)
            {
                if (string.Equals(s, target, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
    }
}
