using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch
{

    public class Attributes
    {
        public string type { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
        public string BillingCountry { get; set; }
        public double TaxaR__c { get; set; }
        public double PIS__c { get; set; }
        public double COFINS__c { get; set; }
        public double ICMS__c { get; set; }
        public double IPI__c { get; set; }
        public double Valor_PIS__c { get; set; }
        public double Valor_Cofins__c { get; set; }
        public double Valor_ICMS__c { get; set; }
        public double Valor_Total_com_Impostos__c { get; set; }

        public string NumeroPedidoSAP__c { get; set; }

        public string DataPedidoSAP__c { get; set; }

        public string DataPedidoIntegrado__c { get; set; }

        public string CondicaoPagamento__c { get; set; }

        public string Transportadora__c { get; set; }

        [JsonIgnore]
        public string numeroDraft { get; set; }

        [JsonIgnore]
        public string pais { get; set; }

        public string Filial_Faturamento__c { get; set; }

       
    }

    public class OrderPatch
    {
        public List<Record> records { get; set; }
    }
}
