using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BPAddressRetorno
    {
        public string AddressName { get; set; }
        public string Street { get; set; }
        public string Block { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string BuildingFloorRoom { get; set; }
        public string AddressType { get; set; }
        public string TypeOfAddress { get; set; }
        public string StreetNo { get; set; }


    }

    public class BPFiscalTaxIDCollectionRetorno
    {
        public string Address { get; set; }
        public object TaxId0 { get; set; }
        public object TaxId1 { get; set; }
        public object TaxId4 { get; set; }
    }

    public class ContactEmployeeRetorno
    {
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public object Phone2 { get; set; }
        public object MobilePhone { get; set; }
        public string E_Mail { get; set; }
        public string FirstName { get; set; }
        public object MiddleName { get; set; }
        public object LastName { get; set; }

        public string U_k33p_SFID { get; set; }

        public string U_SBZ_EnvEml { get; set; }

        //public string U_EhBeneficPag { get; set; }

        public string U_LG_RecEmailXML { get; set; }
    }

    public class BusinessPartnersDTORetorno
    {
        // [JsonIgnore]
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int SalesPersonCode { get; set; }
        public int PayTermsGrpCode { get; set; }
        public string PeymentMethodCode { get; set; }
        public string CardForeignName { get; set; }
        public string CardType { get; set; }
        public int GroupCode { get; set; }
        public int Series { get; set; }
        public string EmailAddress { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string ContactPerson { get; set; }

        public int Industry { get; set; }

        public string U_Setor { get; set; }

        public string U_k33p_SFID { get; set; }

        public string U_k33p_SFSend { get; set; }

        public string FederalTaxID { get; set; }

        public string U_Rut { get; set; }

        public string U_SBZ_CodCart { get; set; }
        public string U_SBZ_GerBol { get; set; }


        public List<BPFiscalTaxIDCollectionRetorno> BPFiscalTaxIDCollection { get; set; }
        public List<BPAddressRetorno> BPAddresses { get; set; }
        public List<ContactEmployeeRetorno> ContactEmployees { get; set; }

        public List<BPPaymentMethodRetorno> BPPaymentMethods { get; set; }

        //[JsonIgnore]
        public Error error { get; set; }
    }

    public class BPPaymentMethodRetorno
    {
        public string PaymentMethodCode { get; set; }
        //public int RowNumber { get; set; }
        //public string BPCode { get; set; }
    }

    public class ErrorRetorno
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class MessageRetorno
    {
        public string lang { get; set; }
        public string value { get; set; }
    }


}
