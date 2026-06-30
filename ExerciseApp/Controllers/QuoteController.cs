using ExerciseApp.Model;
using ExerciseApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExerciseApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : ControllerBase
    {

        private readonly QuoteService _quoteService = new QuoteService();
        private readonly QuoteStorageService _storageService = new QuoteStorageService();

        public QuoteController()
        {

        }

        [HttpGet]
        public QuoteDetail Get()
        {
            return _quoteService.GetQuoteDetail();
        }

        [HttpPost]
        public QuoteResponse Post(QuoteRequest request)
        {
            var returnObject = new QuoteResponse() { QuoteRequestValid = false };
            if (TryValidateModel(request))
            {
                string ageError = _quoteService.GetAgeEligibilityError(request);
                if (ageError != null)
                {
                    returnObject.QuoteRequestValid = false;
                    returnObject.ErrorMessage = ageError;
                }
                else if (_quoteService.IsEligibleForQuote(request))
                {
                    var quoteAmount = _quoteService.PerformQuote(request);
                    returnObject.QuoteRequestValid = true;
                    returnObject.Quote = quoteAmount;

                    // Store the quote for later retrieval
                    returnObject.ReferenceNumber = _storageService.StoreQuote(request, quoteAmount);
                    returnObject.ExpiryDate = System.DateTime.Now.AddDays(30);
                }
            }

            return returnObject;
        }

        /// <summary>
        /// Retrieves a quote by reference number
        /// </summary>
        [HttpGet("{referenceNumber}")]
        public ActionResult<StoredQuote> GetQuoteByReference(string referenceNumber)
        {
            var quote = _storageService.GetQuoteByReference(referenceNumber);
            if (quote == null)
                return NotFound($"Quote with reference '{referenceNumber}' not found.");

            return Ok(quote);
        }

        /// <summary>
        /// Gets all valid quotes
        /// </summary>
        [HttpGet("valid/all")]
        public ActionResult<System.Collections.Generic.List<StoredQuote>> GetValidQuotes()
        {
            var quotes = _storageService.GetValidQuotes();
            return Ok(quotes);
        }

        /// <summary>
        /// Gets statistics about stored quotes
        /// </summary>
        [HttpGet("statistics/summary")]
        public ActionResult<QuoteStatistics> GetStatistics()
        {
            var stats = _storageService.GetStatistics();
            return Ok(stats);
        }

        /// <summary>
        /// Accepts/purchases a quote
        /// </summary>
        [HttpPut("{referenceNumber}/accept")]
        public ActionResult AcceptQuote(string referenceNumber)
        {
            var success = _storageService.AcceptQuote(referenceNumber);
            if (!success)
                return BadRequest($"Could not accept quote '{referenceNumber}'. Quote may not exist or may have expired.");

            return Ok(new { message = $"Quote {referenceNumber} has been accepted.", referenceNumber });
        }

        /// <summary>
        /// Cancels a quote
        /// </summary>
        [HttpDelete("{referenceNumber}")]
        public ActionResult CancelQuote(string referenceNumber)
        {
            var success = _storageService.CancelQuote(referenceNumber);
            if (!success)
                return NotFound($"Quote with reference '{referenceNumber}' not found.");

            return Ok(new { message = $"Quote {referenceNumber} has been cancelled.", referenceNumber });
        }

        /// <summary>
        /// Updates notes for a quote
        /// </summary>
        [HttpPut("{referenceNumber}/notes")]
        public ActionResult UpdateQuoteNotes(string referenceNumber, [FromBody] QuoteNotesRequest notesRequest)
        {
            var success = _storageService.UpdateQuoteNotes(referenceNumber, notesRequest.Notes);
            if (!success)
                return NotFound($"Quote with reference '{referenceNumber}' not found.");

            return Ok(new { message = $"Notes updated for quote {referenceNumber}.", referenceNumber });
        }
    }

    /// <summary>
    /// Request model for updating quote notes
    /// </summary>
    public class QuoteNotesRequest
    {
        public string Notes { get; set; }
    }
}

