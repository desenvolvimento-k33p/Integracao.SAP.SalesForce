using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF
{

    public class AttributesPatch
    {
        public string type { get; set; }
    }

    public class RecordPatch
    {
        public AttributesPatch attributes { get; set; }
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string Status__c { get; set; }
        public string CNPJ__c { get; set; }
    }

    public class Account_Patch
    {
        public List<RecordPatch> records { get; set; }
    }




}
