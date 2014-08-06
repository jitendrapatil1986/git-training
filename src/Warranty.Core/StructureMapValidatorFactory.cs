using System;
using System.Linq;
using FluentValidation;
using StructureMap;

namespace Warranty.Core
{
    public class StructureMapValidatorFactory : ValidatorFactoryBase
    {
        private readonly IContainer _container;

        public StructureMapValidatorFactory(IContainer container)
        {
            _container = container;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            var validationSubject = validatorType.GetGenericArguments().Single();
            var validators = _container.GetAllInstances(validatorType).Cast<IValidator>().ToArray();

            return (IValidator)Activator.CreateInstance(typeof(CompositeValidator), new object[] { validationSubject, validators });
        }
    }
}