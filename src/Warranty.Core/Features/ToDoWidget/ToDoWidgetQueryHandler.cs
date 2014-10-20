using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure;

namespace Warranty.Core.Features.ToDoWidget
{
    using Security;

    public class ToDoWidgetQueryHandler : IQueryHandler<ToDoWidgetQuery, ToDoWidgetModel>
    {
        private readonly IToDoAggregator _toDoAggregator;
        private readonly IUserSession _userSession;

        public ToDoWidgetQueryHandler(IToDoAggregator toDoAggregator, IUserSession userSession)
        {
            _toDoAggregator = toDoAggregator;
            _userSession = userSession;
        }

        public ToDoWidgetModel Handle(ToDoWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var model = new ToDoWidgetModel
            {
                ToDos = _toDoAggregator.Execute(),
                ToDoTypes = ToDoType.GetAccesibleToDos(user.Roles)
            };

            return model;
        }
    }
}
