using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.DTO.Models.DbEntities;
using Kozachok.Shared.DTO.Models.Result.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Handlers.Queries
{
    public class GetScriptProgressQuery : RequestCommand<ScriptProgress>
    {
    }
}
