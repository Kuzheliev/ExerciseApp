namespace ExerciseApp.Model
{
    public class QuoteResponse
    {
       public bool QuoteRequestValid { get; set; }
       public decimal Quote { get; set; }
       public string ErrorMessage { get; set; }
       /// <summary>
       /// Reference number for quote retrieval and tracking
       /// </summary>
       public string ReferenceNumber { get; set; }
       /// <summary>
       /// Quote expiry date (30 days from creation)
       /// </summary>
       public System.DateTime? ExpiryDate { get; set; }
    }
}

