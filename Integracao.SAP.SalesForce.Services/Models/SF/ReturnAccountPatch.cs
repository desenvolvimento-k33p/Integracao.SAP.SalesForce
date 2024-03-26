using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF
{
  
    public class ReturnAccountPatch
    {
        public string id { get; set; }
        public bool success { get; set; }
        public List<object> errors { get; set; }
        public bool created { get; set; }
    }
}
