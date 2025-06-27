namespace CarRental_BE.Models.Enum
{
    public enum PaymentType
    {
        Wallet,
        Cash,
        BankTransfer
    }

    public enum BookingStatus
    {
        WaitingConfirmed,
        PendingPayment,
        PendingDeposit,
        InProgress,
        Confirmed,
        Completed,
        Cancelled
    }

    public enum CarStatus
    {
        Verified,
        Stopped,
        NotVerified
    }

    public enum RoleName
    {
        Operator,
        Customer,
        CarOwner,
        Admin
    }

    public enum TransactionStatus
    {
        Successful,
        Processing,
        Failed
    }

    public enum TransactionType
    {
        Withdraw,
        Top_up,
        RefundDeposit,
        ReceiveDeposit,
        PayDeposit,
        OffsetFinalPayment
    }
}
