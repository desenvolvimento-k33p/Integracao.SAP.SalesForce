using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class QuotePatch2
    {
        public List<Record> records { get; set; }
    }



}
