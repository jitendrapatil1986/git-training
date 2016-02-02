using System;
using NPoco;
using Warranty.Core.Entities;

namespace Warranty.Server.Handlers.Jobs
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
            if (communityNumber == null)
                throw new ArgumentNullException("communityNumber");

            return _database.SingleOrDefault<Community>("WHERE CommunityNumber = @0", communityNumber);
        }
    }
}