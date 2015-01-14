namespace Warranty.Core.Features.ToDoWidget
{
    using Services;

    public class ToDoLastSelectedFilterSaveCommandHandler : ICommandHandler<ToDoLastSelectedFilterSaveCommand>
    {
        private readonly IManageToDoFilterCookie _cookieManager;

        public ToDoLastSelectedFilterSaveCommandHandler(IManageToDoFilterCookie cookieManager)
        {
            _cookieManager = cookieManager;
        }

        public void Handle(ToDoLastSelectedFilterSaveCommand message)
        {
            _cookieManager.Write(message.LastSelectedFilter);
        }
    }
}