using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.QuotationsPatch
{
   
    public class DocumentLine
    {
        public double Price { get; set; }
        public double UnitPrice { get; set; }
    }

    public class QuotationsPatch
    {
        public List<DocumentLine> DocumentLines { get; set; }

        public List<LineTaxJurisdiction> LineTaxJurisdictions { get; set; }
    }

    public class LineTaxJurisdiction
    {
        public int JurisdictionType { get; set; }
        public double TaxAmountFC { get; set; }
        public double TaxRate { get; set; }
    }



}
