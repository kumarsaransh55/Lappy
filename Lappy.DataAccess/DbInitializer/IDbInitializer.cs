using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lappy.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        void InitializeAsync();
    }
}
