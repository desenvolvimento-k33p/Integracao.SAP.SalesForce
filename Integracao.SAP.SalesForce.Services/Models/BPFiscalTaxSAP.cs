using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BPFiscalTaxIDCollection2
    {
        public string Address { get; set; }
        public object CNAECode { get; set; }
        public string TaxId0 { get; set; }
        public string TaxId1 { get; set; }
        public object TaxId2 { get; set; }
        public object TaxId3 { get; set; }
        public object TaxId4 { get; set; }
        public object TaxId5 { get; set; }
        public object TaxId6 { get; set; }
        public object TaxId7 { get; set; }
        public object TaxId8 { get; set; }
        public object TaxId9 { get; set; }
        public object TaxId10 { get; set; }
        public object TaxId11 { get; set; }
        public string BPCode { get; set; }
        public string AddrType { get; set; }
        public string TaxId12 { get; set; }
        public object TaxId13 { get; set; }
        public string AToRetrNFe { get; set; }
    }

    public class BPFiscalTaxSAP
    {
        public List<BPFiscalTaxIDCollection2> BPFiscalTaxIDCollection { get; set; }
    }


}
