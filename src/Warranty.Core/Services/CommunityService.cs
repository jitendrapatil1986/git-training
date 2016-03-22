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

        public string GetWarrantyCommunityNumber(string communityNumber)
        {
            if (communityNumber.Length > 4)
                return communityNumber.Substring(0, 4);

            if (communityNumber.Length < 4)
                return communityNumber.PadRight(4, '0');

            return communityNumber;
        }

        public Community Create(Community newCommunity)
        {
            if(newCommunity == null)
                throw new ArgumentNullException("newCommunity");

            _database.Insert(newCommunity);
            return newCommunity;
        }

        public Community GetCommunityByNumber(string communityNumber)
        {
            if (string.IsNullOrWhiteSpace(communityNumber))
                throw new ArgumentNullException("communityNumber");

            return _database.SingleOrDefault<Community>("WHERE CommunityNumber = @0", GetWarrantyCommunityNumber(communityNumber));
        }
    }
}