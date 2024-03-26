using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attributes
    {
        public string type { get; set; } = "Quote";
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ExpirationDate { get; set; }
        //public string NumeroPedidoCompra__c { get; set; }
        //public string DataPedidoCompra__c { get; set; }
        public string NumeroCotacaoSAP__c { get; set; }
        public string DataCotacaoSAP__c { get; set; }
        //public string TipoCotacao__c { get; set; }
       // public string TaxaPreDefinida__c { get; set; }
        public double Tax { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        //public string Fax { get; set; }
        //public string NumeroCotacaoSalesforce__c { get; set; }
        //public string CondicaoPagamento__c { get; set; }
        //public string EnvioParaContato__c { get; set; }
        //public string Frete__c { get; set; }
        //public double ShippingHandling { get; set; }
        //public double ValorCotacao__c { get; set; }
        public double Valor_Total_com_Impostos__c { get; set; }
        
        //public string MotivoPerda__c { get; set; }
        //public string DescricaoMotivoPerda__c { get; set; }
        //public string Description { get; set; }
        //public string BillingName { get; set; }
        //public string BillingStreet { get; set; }
        //public string BillingCity { get; set; }
        //public string BillingState { get; set; }
        //public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }

        public string Pais_da_Venda__c { get; set; }
        //public string ComplementoFaturamento__c { get; set; }
        //public int NumeroFaturamento__c { get; set; }
        //public string LogradouroFaturamento__c { get; set; }
        //public string ShippingName { get; set; }
        //public string ShippingStreet { get; set; }
        //public string ShippingCity { get; set; }
        //public string ShippingState { get; set; }
        //public string ShippingPostalCode { get; set; }
        //public string ShippingCountry { get; set; }
        //public string ComplementoRemessa__c { get; set; }
        //public int NumeroRemessa__c { get; set; }
        //public string LogradouroRemessa__c { get; set; }
        public double TaxaR__c { get; set; }

        public double PIS__c { get; set; }
        public double COFINS__c { get; set; }
        public double ICMS__c { get; set; }
        public double IPI__c { get; set; }
        public double Valor_PIS__c { get; set; }
        public double Valor_Cofins__c { get; set; }
        public double Valor_ICMS__c { get; set; }

        // public double ValorIPI__c { get; set; }

        [JsonIgnore]
        public string numeroDraft { get; set; }

        [JsonIgnore]
        public string pais { get; set; }

        public string Transportadora__c { get; set; }

        public double IVA__c { get; set; }

        public double IVAV__c { get; set; }

    }

    public class QuotePatch
    {
        public List<Record> records { get; set; }
    }


    
}
