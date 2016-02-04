using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Core.Services
{
    public class CommunityService : ICommunityService
    {
        private IDatabase _database;

        public CommunityService(IDatabase database)
        {
            _database = database;
        }

        public Community GetCommunityByNumber(string communityNumber)
        {
            if (string.IsNullOrWhiteSpace(communityNumber))
                throw new ArgumentNullException("communityNumber");

            return _database.SingleOrDefault<Community>("WHERE CommunityNumber = @0", communityNumber);
        }
    }
}