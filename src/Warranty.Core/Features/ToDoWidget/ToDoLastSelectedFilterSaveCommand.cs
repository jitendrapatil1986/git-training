namespace Warranty.Core.Features.ToDoWidget
{
    public class ToDoLastSelectedFilterSaveCommand : ICommand
    {
        public string LastSelectedFilter { get; set; }
    }
}