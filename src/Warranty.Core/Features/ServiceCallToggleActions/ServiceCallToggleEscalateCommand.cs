﻿using System;

namespace Warranty.Core.Features.ServiceCallToggleActions
{
    public class ServiceCallToggleEscalateCommand : ICommand<ServiceCallToggleEscalateCommandResult>
    {
        public Guid ServiceCallId { get; set; }
        public string Text { get; set; }
    }
}