using Kozachok.Shared.DTO.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Shared.DTO.Models.DbEntities
{
    public class ScriptProgress : Entity
    {
        public ScriptProgress()
        {

        }

        public int CurrentPage { get; set; }
        public int MaxPages { get; set; }
        public int NumOfFilmsInserted { get; set; }
        public bool IsScriptRunning { get; set; }
    }
}
