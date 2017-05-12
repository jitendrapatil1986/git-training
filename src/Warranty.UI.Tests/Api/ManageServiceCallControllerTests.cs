using System;
using Common.Test;
using Moq;
using NUnit.Framework;
using Should;
using Warranty.Core;
using Warranty.Core.Enumerations;
using Warranty.Core.Features.CompleteServiceCallLineItem;
using Warranty.UI.Api;

namespace Warranty.UI.Tests.Api
{
    [TestFixture]
    public class ManageServiceCallControllerTests : BaseTest
    {
        [Test]
        public void CompleteLineItem_WhenRootProblemAndRootCauseAreValid_SendsCompleteServiceCallLineItemCommand()
        {
            var mediator = new Mock<IMediator>();
            var controller = new ManageServiceCallController(mediator.Object, null, null);
            var model = new CompleteServiceCallLineItemModel
            {
                RootProblem = RootProblem.Appliances,
                RootCause = RootCause.BuilderIncomplete.ToString(),
            };

            controller.CompleteLineItem(model);

            mediator.Verify(x => x.Send(It.IsAny<CompleteServiceCallLineItemCommand>()));
        }

        [TestCase(null),
         TestCase(""),
         TestCase("invalid root problem")]
        public void CompleteLineItem_WhenRootProblemIsInvalid_ThrowsArgumentException(string rootProblem)
        {
            var controller = new ManageServiceCallController(null, null, null);
            var model = new CompleteServiceCallLineItemModel
            {
                RootProblem = rootProblem,
                RootCause = RootCause.BuilderIncomplete.ToString()
            };

            var result = Assert.Throws<ArgumentException>(() => controller.CompleteLineItem(model));

            result.ParamName.ShouldEqual("model.RootProblem");
        }

        [TestCase(null),
         TestCase(""),
         TestCase("invalid root problem")]
        public void CompleteLineItem_WhenRootCauseIsInvalid_ThrowsArgumentException(string rootCause)
        {
            var controller = new ManageServiceCallController(null, null, null);
            var model = new CompleteServiceCallLineItemModel
            {
                RootCause = rootCause,
                RootProblem = RootProblem.Appliances
            };
            
            var result = Assert.Throws<ArgumentException>(() => controller.CompleteLineItem(model));

            result.ParamName.ShouldEqual("model.RootCause");
        }
    }
}