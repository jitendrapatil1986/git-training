using Warranty.Core.Enumerations;
using Warranty.Core.ToDoInfrastructure;

namespace Warranty.Core.Features.ToDoWidget
{
    public class ToDoWidgetQueryHandler : IQueryHandler<ToDoWidgetQuery, ToDoWidgetModel>
    {
        private readonly IToDoAggregator _toDoAggregator;

        public ToDoWidgetQueryHandler(IToDoAggregator toDoAggregator)
        {
            _toDoAggregator = toDoAggregator;
        }

        public ToDoWidgetModel Handle(ToDoWidgetQuery query)
        {
            var model = new ToDoWidgetModel
            {
                ToDos = _toDoAggregator.Execute(),
                ToDoTypes = ToDoType.GetAll()
            };

            return model;
        }
    }
}
