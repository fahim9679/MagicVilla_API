using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Logging
{
    public interface ILogging
    {
        public void Log(string message, string type);
    }
}
