using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2
{

    public class Attributes
    {
        public string type { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
        public string ProdutoCodSAP__c { get; set; }
    }

    public class OrderPatch2
    {
        public List<Record> records { get; set; }
    }
}
