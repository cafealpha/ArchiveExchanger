using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archiveExchanger
{
    class archiveFormat : List<string>
    {
        public archiveFormat()
        {
            this.Add("ZIP");
            this.Add("7Z");
        }
    }
}
