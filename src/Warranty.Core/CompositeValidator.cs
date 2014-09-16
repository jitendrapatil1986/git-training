using System.Threading.Tasks;

namespace Warranty.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using FluentValidation.Internal;
    using FluentValidation.Results;

    public class CompositeValidator : IValidator
    {
        private readonly Type _subject;
        private readonly IValidator[] _validators;

        public CompositeValidator(Type subject, params IValidator[] validators)
        {
            _subject = subject;
            _validators = validators;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IValidationRule> GetEnumerator()
        {
            return _validators.SelectMany(v => v).GetEnumerator();
        }

        public ValidationResult Validate(object instance)
        {
            return Validate(new ValidationContext(instance, new PropertyChain(), new DefaultValidatorSelector()));
        }

        public Task<ValidationResult> ValidateAsync(object instance)
        {
            throw new NotImplementedException();
        }

        public ValidationResult Validate(ValidationContext context)
        {
            return new ValidationResult(_validators.SelectMany(v => v.Validate(context).Errors));
        }

        public Task<ValidationResult> ValidateAsync(ValidationContext context)
        {
            throw new NotImplementedException();
        }

        public IValidatorDescriptor CreateDescriptor()
        {
            return (IValidatorDescriptor)Activator.CreateInstance(typeof(ValidatorDescriptor<>).MakeGenericType(_subject), _validators.SelectMany(v => v));
        }

        public bool CanValidateInstancesOfType(Type type)
        {
            return _subject.IsAssignableFrom(type);
        }
    }
}