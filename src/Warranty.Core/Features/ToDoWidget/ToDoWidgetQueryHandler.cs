using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure;

namespace Warranty.Core.Features.ToDoWidget
{
    using Security;
    using Services;

    public class ToDoWidgetQueryHandler : IQueryHandler<ToDoWidgetQuery, ToDoWidgetModel>
    {
        private readonly IToDoAggregator _toDoAggregator;
        private readonly IUserSession _userSession;
        private readonly IManageToDoFilterCookie _cookieManager;

        public ToDoWidgetQueryHandler(IToDoAggregator toDoAggregator, IUserSession userSession, IManageToDoFilterCookie cookieManager)
        {
            _toDoAggregator = toDoAggregator;
            _userSession = userSession;
            _cookieManager = cookieManager;
        }

        public ToDoWidgetModel Handle(ToDoWidgetQuery query)
        {
            var user = _userSession.GetCurrentUser();
            var selectedToDo = _cookieManager.Read();
            var model = new ToDoWidgetModel
            {
                ToDos = _toDoAggregator.Execute(),
                ToDoTypes = ToDoType.GetAccesibleToDos(user.Roles),
                SelectedToDo = selectedToDo
            };

            return model;
        }
    }
}
