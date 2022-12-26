using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Repository.Config
{
    public interface IDbEntityConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
