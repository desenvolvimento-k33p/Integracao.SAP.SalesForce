using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.Quote_QuoteItens
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class BillingAddress
    {
        public string city { get; set; }
        public string country { get; set; }
        public object geocodeAccuracy { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string street { get; set; }
    }

    public class QuoteLineItems
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<RecordsLine> records { get; set; }
    }

    public class RecordsLine
    {
        public Attributes attributes { get; set; }
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string LineNumber { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public string SystemModstamp { get; set; }
        public object LastViewedDate { get; set; }
        public object LastReferencedDate { get; set; }
        public string QuoteId { get; set; }
        public string PricebookEntryId { get; set; }
        public string OpportunityLineItemId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double? Discount { get; set; }
        public object Description { get; set; }
        public object ServiceDate { get; set; }
        public string Product2Id { get; set; }
        public object SortOrder { get; set; }
        public double ListPrice { get; set; }
        public double Subtotal { get; set; }
        public double TotalPrice { get; set; }
        public object ProdutoCodSAP__c { get; set; }
        public string NomeProduto__c { get; set; }
        public string DataEntrega__c { get; set; }
        public double DescontoValor__c { get; set; }
        public object UnidadeMedidaQuantidade__c { get; set; }
        public double Desconto__c { get; set; }
        public object ValorIPI__c { get; set; }
        public object AliquotaIPI__c { get; set; }
        public object VIPI__c { get; set; }
        public object VPIS__c { get; set; }
        public string TipoEmbalagem__c { get; set; }
        public string CodigoDoProduto__c { get; set; }
        public object VCOFINS__c { get; set; }
        public string NomeDoProduto__c { get; set; }
        public object Valor_ICMS__c { get; set; }

        public string Numero_Item_Pedido__c { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public QuoteLineItems QuoteLineItems { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public string SystemModstamp { get; set; }
        public object LastViewedDate { get; set; }
        public object LastReferencedDate { get; set; }
        public string OpportunityId { get; set; }
        public string Pricebook2Id { get; set; }
        public string ContactId { get; set; }
        public string QuoteNumber { get; set; }
        public bool IsSyncing { get; set; }
        public object ShippingHandling { get; set; }
        public object Tax { get; set; }
        public string Status { get; set; }
        public string ExpirationDate { get; set; }
        public string Description { get; set; }
        public double Subtotal { get; set; }
        public double TotalPrice { get; set; }
        public int LineItemCount { get; set; }
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public object BillingLatitude { get; set; }
        public object BillingLongitude { get; set; }
        public object BillingGeocodeAccuracy { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public object ShippingLatitude { get; set; }
        public object ShippingLongitude { get; set; }
        public object ShippingGeocodeAccuracy { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public object QuoteToStreet { get; set; }
        public object QuoteToCity { get; set; }
        public object QuoteToState { get; set; }
        public object QuoteToPostalCode { get; set; }
        public object QuoteToCountry { get; set; }
        public object QuoteToLatitude { get; set; }
        public object QuoteToLongitude { get; set; }
        public object QuoteToGeocodeAccuracy { get; set; }
        public object QuoteToAddress { get; set; }
        public object AdditionalStreet { get; set; }
        public object AdditionalCity { get; set; }
        public object AdditionalState { get; set; }
        public object AdditionalPostalCode { get; set; }
        public object AdditionalCountry { get; set; }
        public object AdditionalLatitude { get; set; }
        public object AdditionalLongitude { get; set; }
        public object AdditionalGeocodeAccuracy { get; set; }
        public object AdditionalAddress { get; set; }
        public string BillingName { get; set; }
        public string ShippingName { get; set; }
        public object QuoteToName { get; set; }
        public object AdditionalName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public object Fax { get; set; }
        public object ContractId { get; set; }
        public string AccountId { get; set; }
        public double? Discount { get; set; }
        public double GrandTotal { get; set; }
        public bool CanCreateQuoteLineItems { get; set; }
        public object NumeroCotacaoSalesforce__c { get; set; }
        public string NumeroPedidoCompra__c { get; set; }
        public string NumeroCotacaoSAP__c { get; set; }
        public string TaxaPreDefinida__c { get; set; }
        public string CondicaoPagamento__c { get; set; }
        public object EnvioParaContato__c { get; set; }
        public string DataPedidoCompra__c { get; set; }
        public string DataCotacaoSAP__c { get; set; }
        public string TipoCotacao__c { get; set; }
        public double? PIS__c { get; set; }
        public string Frete__c { get; set; }
        public object ValorCotacao__c { get; set; }
        public object MotivoPerda__c { get; set; }
        public object DescricaoMotivoPerda__c { get; set; }
        public string LogradouroFaturamento__c { get; set; }
        public double NumeroFaturamento__c { get; set; }
        public object ComplementoFaturamento__c { get; set; }
        public string LogradouroRemessa__c { get; set; }
        public double NumeroRemessa__c { get; set; }
        public object ComplementoRemessa__c { get; set; }
        public string BairroFaturamento__c { get; set; }
        public string IdConta__c { get; set; }
        public bool PedidoGerado__c { get; set; }
        public string SetorAplicacao__c { get; set; }
        public string UnidadeNegocios__c { get; set; }
        public string NomeDoProduto__c { get; set; }
        public string TipoEmbalagem__c { get; set; }
        public bool PrecoVendaMenor__c { get; set; }
        public double? COFINS__c { get; set; }
        public double? ICMS__c { get; set; }
        public string Transportadora__c { get; set; }
        public string Celular__c { get; set; }
        public string BairroRemessa__c { get; set; }
        public double? TaxaR__c { get; set; }
        public double? Valor_Total_com_Impostos__c { get; set; }
        public string VendaDireta__c { get; set; }
        public string LocalidadeVenda__c { get; set; }
        public double? IPI__c { get; set; }
        public double? Valor_IPI__c { get; set; }
        public double? Valor_PIS__c { get; set; }
        public double? Valor_Cofins__c { get; set; }
        public double? Valor_ICMS__c { get; set; }

       public string Pais_da_Venda__c { get;  set; } 
        //public string LineNumber { get; set; }
        //public string QuoteId { get; set; }
        //public string PricebookEntryId { get; set; }
        //public string OpportunityLineItemId { get; set; }
        //public double Quantity { get; set; }
        //public double UnitPrice { get; set; }
        //public object ServiceDate { get; set; }
        //public string Product2Id { get; set; }
        //public object SortOrder { get; set; }
        //public double ListPrice { get; set; }
        //public object ProdutoCodSAP__c { get; set; }
        //public string NomeProduto__c { get; set; }
        //public string DataEntrega__c { get; set; }
        //public double DescontoValor__c { get; set; }
        //public object UnidadeMedidaQuantidade__c { get; set; }
        //public double Desconto__c { get; set; }
        //public double? ValorIPI__c { get; set; }
        //public object AliquotaIPI__c { get; set; }
        //public object VIPI__c { get; set; }
        //public double? VPIS__c { get; set; }
        //public string CodigoDoProduto__c { get; set; }
        //public double? VCOFINS__c { get; set; }

        public string Filial_Faturamento__c { get; set; }
    }

    public class Quote_QuoteItens
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class ShippingAddress
    {
        public string city { get; set; }
        public string country { get; set; }
        public object geocodeAccuracy { get; set; }
        public object latitude { get; set; }
        public object longitude { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string street { get; set; }
    }


}
