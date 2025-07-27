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
        verified,
        stopped,
        not_verified
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
        refund_deposit,
        receive_deposit,
        pay_deposit,
        offset_final_payment
    }
}
