namespace Api.Entities;

public partial class Card
{
    public int CardId { get; set; }

    public string CardNumber { get; set; } = string.Empty;

    public int? Pin { get; set; }

    public int FailedAttempts { get; private set; }

    public bool IsBlocked { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;

    public void UpdateFailedAttempts(Card card)
    {
        card.FailedAttempts++;

        if (card.FailedAttempts >= 4)
        {
            card.IsBlocked = true;
        }
    }
}
