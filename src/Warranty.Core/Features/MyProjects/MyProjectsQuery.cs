using System;
using System.Collections.Generic;

namespace Warranty.Core.Features.MyProjects
{
    public class MyProjectsQuery : IQuery<Dictionary<Guid, string>>
    {
         public Guid? DivisionId { get; set; }
    }
}