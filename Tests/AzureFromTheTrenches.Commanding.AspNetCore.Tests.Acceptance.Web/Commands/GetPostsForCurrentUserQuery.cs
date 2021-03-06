﻿using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands.Responses;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Tests.Acceptance.Web.Commands
{
    public class GetPostsForCurrentUserQuery : ICommand<IReadOnlyCollection<Post>>
    {
        [SecurityProperty]
        public Guid UserId { get; set; }
    }
}
