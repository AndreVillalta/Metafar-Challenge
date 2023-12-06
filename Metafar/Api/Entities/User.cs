namespace Api.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public decimal CurrentBalance { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public void UpdateCurrentBalance(User user, decimal amount)
    {
        user.CurrentBalance -= amount;
    }
}
