using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UDP.View.ViewModels.ValidationAttributes
{
    public class NotCompareAttribute : ValidationAttribute
    {
        private string _otherProperty;

        public NotCompareAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = value;
            var property = validationContext.ObjectType.GetProperty(_otherProperty);

            if (property == null)
                return new ValidationResult(string.Format("Property {0} not found", _otherProperty));

            var otherValue = property.GetValue(validationContext.ObjectInstance);

            if (!currentValue.Equals(otherValue))
                return ValidationResult.Success;

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public override string FormatErrorMessage(string name)
        {
            return $"Field {name} must be different from field {_otherProperty}.";
        }
    }
}
