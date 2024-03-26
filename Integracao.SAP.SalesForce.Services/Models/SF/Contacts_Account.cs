using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.SF
{
  
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

    public class Contacts
    {
        public int totalSize { get; set; }
        public bool done { get; set; }
        public List<Record> records { get; set; }
    }

    public class MailingAddress
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

    public class Record
    {
        public Attributes attributes { get; set; }
        public Contacts Contacts { get; set; }
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public string MasterRecordId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ParentId { get; set; }
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
        public string Phone { get; set; }
        public string AccountNumber { get; set; }
        public string Website { get; set; }
        public string PhotoUrl { get; set; }
        public string Industry { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Description { get; set; }
        public string Site { get; set; }
        public string OwnerId { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public string SystemModstamp { get; set; }
        public string LastActivityDate { get; set; }
        public string LastViewedDate { get; set; }
        public string LastReferencedDate { get; set; }
        public string Jigsaw { get; set; }
        public string JigsawCompanyId { get; set; }
        public string AccountSource { get; set; }
        public string SicDesc { get; set; }
        public string CNPJ__c { get; set; }
        public string InscricaoEstadual__c { get; set; }
        public string Status__c { get; set; }
        public string UnidadeDeNegocios__c { get; set; }
        public string SetorDeAplicacao__c { get; set; }
        public string LogradouroEntrega__c { get; set; }
        public string LogradouroCobranca__c { get; set; }
        public double NumeroEntrega__c { get; set; }
        public double NumeroCobranca__c { get; set; }
        public string ComplementoEntrega__c { get; set; }
        public string ComplementoCobranca__c { get; set; }
        public double NumeroContatosRelacionados__c { get; set; }
        public string PermiteTaxaPreDefinida__c { get; set; }
        public string NomeFantasia__c { get; set; }
        public string Taxa__c { get; set; }
        public string BairroCobranca__c { get; set; }
        public string BairroEntrega__c { get; set; }
        public double? TaxaR__c { get; set; }
        public string CondicaoPagamento__c { get; set; }
        public string RUT__c { get; set; }
        public string Tipo_Tributario__c { get; set; }

        public string Forma_de_Pagamento__c { get; set; }
        public string Regime_Tributario__c { get; set; }
        public string Inicio_do_Regime__c { get; set; }
        public string Fim_do_Regime__c { get; set; }
        public string AccountId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Salutation { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string MailingStreet { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingPostalCode { get; set; }
        public string MailingCountry { get; set; }
        public string MailingLatitude { get; set; }
        public string MailingLongitude { get; set; }
        public string MailingGeocodeAccuracy { get; set; }
        public MailingAddress MailingAddress { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string OtherPhone { get; set; }
        public string AssistantPhone { get; set; }
        public string ReportsToId { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Birthdate { get; set; }
        public string LastCURequestDate { get; set; }
        public string LastCUUpdateDate { get; set; }
        public string EmailBouncedReason { get; set; }
        public string EmailBouncedDate { get; set; }
        public bool IsEmailBounced { get; set; }
        public string JigsawContactId { get; set; }
        public string IndividualId { get; set; }
        public string NumeroDaConta__c { get; set; }
        public string Departamento__c { get; set; }
        public string RecebeNFe__c { get; set; }
        public string RecebeBoleto__c { get; set; }
        public string IdContaFormula__c { get; set; }
    }

    public class Contacts_Account
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
