using ExerciseApp.Model;
using System;

namespace ExerciseApp.Service
{
    public class QuoteService
    {
        public QuoteDetail GetQuoteDetail()
        {
            var quoteDetail = new QuoteDetail();

            quoteDetail.Makes.Add("Ford");
            quoteDetail.Makes.Add("Audi");
            quoteDetail.Makes.Add("BMW");

            var modelSpec = new ModelSpec { Make = "Ford" };
            modelSpec.Models.AddRange(new []{ "Fiesta", "Focus", "Puma", "S Max" });
            quoteDetail.Models.Add(modelSpec);

            modelSpec = new ModelSpec { Make = "Audi" };
            modelSpec.Models.AddRange(new[] { "A3", "A4", "A5" });
            quoteDetail.Models.Add(modelSpec);

            modelSpec = new ModelSpec { Make = "BMW" };
            modelSpec.Models.AddRange(new[] { "X5", "3 Series", "5 Series" });
            quoteDetail.Models.Add(modelSpec);

            return quoteDetail;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public decimal PerformQuote(QuoteRequest request)
        {
            if (request.InsuranceType == InsuranceType.FullyComprehensive)
            {
                if (request.Make == "Ford")
                    return 200;
                if (request.Make == "BMW")
                {
                    if (request.Model == "X5")
                        return 500;
                    else
                        return 400;
                }
                return 300;
            }
            if (request.InsuranceType == InsuranceType.ThirdPartyFireAndTheft) {
                if (request.Make == "Ford")
                    return 180;
                if (request.Make == "BMW")
                {
                    if (request.Model == "X5")
                        return 510;
                    else
                        return 400;
                }
                return 300;
            }
            if (request.InsuranceType == InsuranceType.ThirdPartyOnly)
            {
                if (request.Make == "Ford")
                    return 180;
                if (request.Make == "Audi")
                {
                    return 250;
                }
                return 300;
            }
            return 0;
        }

        public bool IsEligibleForQuote(QuoteRequest request)
        {
            if (!request.DateOfBirth.HasValue)
                return false;

            int age = CalculateAge(request.DateOfBirth.Value);
            return age >= 17 && age <= 80;
        }

        public string GetAgeEligibilityError(QuoteRequest request)
        {
            if (!request.DateOfBirth.HasValue)
                return "Date of birth is required.";

            int age = CalculateAge(request.DateOfBirth.Value);

            if (age < 17)
                return "You must be at least 17 years old to receive a quote.";

            if (age > 80)
                return "You must be 80 years old or younger to receive a quote.";

            return null;
        }
    }
}
