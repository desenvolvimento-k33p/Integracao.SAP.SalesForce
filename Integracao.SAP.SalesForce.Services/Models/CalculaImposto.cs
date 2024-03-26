using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models
{
    public class CalculaImposto
    {
        public string Imposto { get; set; }
        public double TaxRate { get; set; }
        public double TaxSum { get; set; }

        public double TaxSumFrgn { get; set; }


    }
}
