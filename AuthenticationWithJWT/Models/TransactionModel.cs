namespace AuthenticationWithJWT.Models
{
    public class TransactionModel
{
    public int TransactionID { get; set; }
    public int AccountID { get; set; }
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    // Add other properties as needed
}
}
