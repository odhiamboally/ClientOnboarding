using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Shared.Validation.Validators.Common;


public abstract class Validator<T> : AbstractValidator<T>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
        async (model, propertyName) =>
        {
            try
            {
                if (model is not T validModel) return [];

                var result = await ValidateAsync(
                    ValidationContext<T>.CreateWithOptions(validModel,
                    x => x.IncludeProperties(propertyName)));

                return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
            }
            catch (Exception)
            {
                // Return a friendly string instead of letting the exception bubble up to the UI
                return ["Validation could not be performed."];
            }

            //var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));

            //return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
                
                
        };
}