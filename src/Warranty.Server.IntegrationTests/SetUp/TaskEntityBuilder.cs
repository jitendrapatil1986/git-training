namespace Warranty.Server.IntegrationTests.SetUp
{
    using System;
    using System.Globalization;
    using Core.Entities;
    using Core.Enumerations;
    using NPoco;

    public class TaskEntityBuilder : EntityBuilder<Task>
    {
        public TaskEntityBuilder(IDatabase database)
            : base(database)
        {
        }

        public override Task GetSaved(Action<Task> action)
        {
            var job = GetSaved<Job>();
            var emp = GetSaved<Employee>();

            var entity = new Task
            {
                TaskType = TaskType.JobStage7,
                IsNoAction = true,
                IsComplete = false,
                Description = "Test Task",
                EmployeeId = emp.EmployeeId,
                ReferenceId = job.JobId,
                CreatedBy = "Test",
                CreatedDate = DateTime.UtcNow
            };

            return Saved(entity, action);
        }
    }
}