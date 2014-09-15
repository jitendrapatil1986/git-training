using System;
using Mvc.Mailer;

namespace Warranty.UI.Mailer
{
    public interface IWarrantyMailer
    {
        MvcMailMessage NewServiceCallAssignedToWsr(Guid serviceCallId);
    }
}