namespace Api.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public DateTime TransactionDate { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal Amount { get; set; }

    public int CardId { get; set; }

    public virtual Card Card { get; set; } = null!;
}
