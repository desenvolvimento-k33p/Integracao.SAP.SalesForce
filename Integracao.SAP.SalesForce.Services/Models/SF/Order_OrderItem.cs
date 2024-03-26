using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF.Order_OrderItem
{

    // Root myDeserializedClass = JsonConvert.Deserializestring<Root>(myJsonResponse);
    public class Attributes
    {
        public string type { get; set; }
        public string url { get; set; }
    }

    public class BillingAddress
    {
        public string city { get; set; }
        public string country { get; set; }
        public string geocodeAccuracy { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string street { get; set; }
    }

    public class OrderItems
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class Record
    {
        public Attributes attributes { get; set; }
        public OrderItems OrderItems { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string ContractId { get; set; }
        public string AccountId { get; set; }
        public string Pricebook2Id { get; set; }
        public string OriginalOrderId { get; set; }
        public string OpportunityId { get; set; }
        public string QuoteId { get; set; }
        public string EffectiveDate { get; set; }
        public string EndDate { get; set; }
        public bool IsReductionOrder { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string CustomerAuthorizedById { get; set; }
        public string CompanyAuthorizedById { get; set; }
        public string Type { get; set; }
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public string BillingLatitude { get; set; }
        public string BillingLongitude { get; set; }
        public string BillingGeocodeAccuracy { get; set; }
        public BillingAddress BillingAddress { get; set; }
        public string ShippingStreet { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingLatitude { get; set; }
        public string ShippingLongitude { get; set; }
        public string ShippingGeocodeAccuracy { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public string PoDate { get; set; }
        public string PoNumber { get; set; }
        public string ShipToContactId { get; set; }
        public string ActivatedDate { get; set; }
        public string ActivatedById { get; set; }
        public string StatusCode { get; set; }
        public string OrderNumber { get; set; }
        public double TotalAmount { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public bool IsDeleted { get; set; }
        public string SystemModstamp { get; set; }
        public string LastViewedDate { get; set; }
        public string LastReferencedDate { get; set; }
        public string NomeCotacao__c { get; set; }
        public string NumeroPedidoSAP__c { get; set; }
        public string TaxaPreDefinida__c { get; set; }
        public string CondicaoPagamento__c { get; set; }
        public string DataPedidoCompra__c { get; set; }
        public string DataPedidoSAP__c { get; set; }
        public string EnvioContato__c { get; set; }
        public double? Taxa__c { get; set; }
        public string Frete__c { get; set; }
        public string ProdutoCodSAP__c { get; set; }
        public string BairroCobranca__c { get; set; }
        public string DataPedidoIntegrado__c { get; set; }
        public string LogradouroCobranca__c { get; set; }
        public string LogradouroEnvio__c { get; set; }
        public double NumeroCobranca__c { get; set; }
        public double NumeroEnvio__c { get; set; }
        public string ComplementoCobranca__c { get; set; }
        public string ComplementoEnvio__c { get; set; }
        public string DataValidade__c { get; set; }
        public string BairroEnvio__c { get; set; }
        public double? TaxaR__c { get; set; }
        public string Transportadora__c { get; set; }
        public double? PIS__c { get; set; }
        public double? COFINS__c { get; set; }
        public double? ICMS__c { get; set; }
        public double? Valor_Total_com_Impostos__c { get; set; }
        public string VendaDireta__c { get; set; }
        public string LocalidadeVenda__c { get; set; }
        public double? IPI__c { get; set; }
        public double? Valor_Cofins__c { get; set; }
        public double? Valor_PIS__c { get; set; }
        public double? Valor_ICMS__c { get; set; }
        public double? Valor_IPI__c { get; set; }
        public string Product2Id { get; set; }
        public string OrderId { get; set; }
        public string PricebookEntryId { get; set; }
        public string OriginalOrderItemId { get; set; }
        public string QuoteLineItemId { get; set; }
        public double AvailableQuantity { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double ListPrice { get; set; }
        public double TotalPrice { get; set; }
        public string ServiceDate { get; set; }
        public string OrderItemNumber { get; set; }
        public double? ValorIPI__c { get; set; }
        public string AliquotaIPI__c { get; set; }
        public string PrecoTotal__c { get; set; }
        public string TipoEmbalagem__c { get; set; }
        public string CodigoProduto__c { get; set; }
        public string Cotacao__c { get; set; }
        public string DataEntrega__c { get; set; }
        public string DescontoValor__c { get; set; }
        public string Desconto__c { get; set; }
        public string Descricao__c { get; set; }
    }

    public class Order_OrderItem
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class ShippingAddress
    {
        public string city { get; set; }
        public string country { get; set; }
        public string geocodeAccuracy { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string postalCode { get; set; }
        public string state { get; set; }
        public string street { get; set; }
    }




}
