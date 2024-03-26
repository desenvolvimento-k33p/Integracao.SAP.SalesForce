using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Domain.Logger
{
    public class LogIntegration
    {
        public int LogTypeCode { get; set; }
        public int Module { get; set; }
        public string Company { get; set; }
        public string Message { get; set; }
        public string FullMessage { get; set; }
        public string Key { get; set; }//chave sf
        public string Key2 { get; set; }//chave sap
        public string RequestObject { get; set; }
        public string ResponseObject { get; set; }
        public string Database { get; set; }
        public string Owner { get; set; }
        public string Method { get; set; }

    }
}
