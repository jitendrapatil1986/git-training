namespace Warranty.Core.Services
{
    using System;
    using Features;

    public interface IHomeownerAdditionalContactsService
    {
        AdditionalContactsModel Get(Guid homeownerId);
    }
}