using System.ComponentModel.DataAnnotations;

namespace TWeb.Common.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValid<T>(T model, out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            
            if (model == null)
            {
                validationResults.Add(new ValidationResult("Model cannot be null"));
                return false;
            }
            
            var validationContext = new ValidationContext(model);
            return Validator.TryValidateObject(model, validationContext, validationResults, true);
        }
    }
}