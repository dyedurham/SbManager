using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SbManager.Models
{
    public static class ModelExtensions
    {
        public static ValidationResult Validate<T>(this T model)
        {
            var ctx = new ValidationContext(model, null, null);
            var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (Validator.TryValidateObject(model, ctx, errors, true)) return new ValidationResult { IsValid = true };

            return new ValidationResult
            {
                IsValid = false,
                Exception = new AggregateException(errors.Select(e => new ValidationException(e.ErrorMessage)))
            };
        }
    }

    public class ValidationResult
    {
        public AggregateException Exception { get; set; }
        public bool IsValid { get; set; }
    }
}

