using System.Collections.Generic;
using Common.Security.Session;
using Moq;
using NUnit.Framework;
using Warranty.Core;
using Warranty.Core.Features.QuickSearch;
using Warranty.UI.Api;
using Common.Test;
using Should;

namespace Warranty.UI.Tests.Api
{
    [TestFixture]
    public class QuickSearchControllerTests : BaseTest
    {
        [Test]
        public void QuickSearchControllerTests_When_MarketIsNull_QueriesWithAllUserMarkets()
        {
            var mediator = new Mock<IMediator>();
            var userSession = new Mock<IUserSession>();
            var user = new Mock<IUser>();

            // set up mediator to capture the actual markets sent in the query

            string actualMarkets = null;
            mediator.Setup(x => x.Request(It.IsAny<QuickSearchVendorsQuery>()))
                .Callback<IQuery<IEnumerable<QuickSearchCallVendorModel>>>(query => actualMarkets = ((QuickSearchVendorsQuery) query).CityCode);

            // set up session

            userSession.Setup(x => x.GetCurrentUser())
                .Returns(user.Object);

            // set up user to return a random list of markets (3-letter strings)

            var expectedMarkets = GenerateRandom(i => Random(3));
            user.Setup(x => x.Markets)
                .Returns(expectedMarkets);
            var expectedMarketsString = string.Join(",", expectedMarkets);

            // build controller

            var controller = new QuickSearchController(mediator.Object, userSession.Object);

            // run Vendors method with a null market

            var searchTerm = Random();
            var nullMarket = (string) null;
            var invoicePayableCode = Random();
            controller.Vendors(searchTerm, nullMarket, invoicePayableCode);
            
            // We queried on the user's markets

            actualMarkets.ShouldEqual(expectedMarketsString);
        }

        [Test]
        public void QuickSearchControllerTests_When_MarketGiven_QueriesWithThatMarket()
        {
            var mediator = new Mock<IMediator>();
            var userSession = new Mock<IUserSession>();
            var user = new Mock<IUser>();

            // set up mediator to capture the actual markets sent in the query

            string actualMarkets = null;
            mediator.Setup(x => x.Request(It.IsAny<QuickSearchVendorsQuery>()))
                .Callback<IQuery<IEnumerable<QuickSearchCallVendorModel>>>(query => actualMarkets = ((QuickSearchVendorsQuery)query).CityCode);

            // set up session

            userSession.Setup(x => x.GetCurrentUser())
                .Returns(user.Object);

            // set up user to return a random list of markets (3-letter strings)

            var userMarkets = GenerateRandom(i => Random(3));
            user.Setup(x => x.Markets)
                .Returns(userMarkets);
            
            // build controller

            var controller = new QuickSearchController(mediator.Object, userSession.Object);

            // run Vendors method with a non-null market

            var market = Random(3);
            var searchTerm = Random();
            var invoicePayableCode = Random();
            controller.Vendors(searchTerm, market, invoicePayableCode);

            // We queried on the given market

            actualMarkets.ShouldEqual(market);
        }
    }
}
