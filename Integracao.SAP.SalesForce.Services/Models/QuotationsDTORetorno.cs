using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class DocumentLineRetorno
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double PriceAfterVAT { get; set; }
        public string Currency { get; set; }
        public double? Rate { get; set; }
        public double? DiscountPercent { get; set; }
        public string WarehouseCode { get; set; }
        public string AccountCode { get; set; }
        public string CostingCode { get; set; }
        public string TaxCode { get; set; }
        public string CFOPCode { get; set; }
        public int Usage { get; set; }
        public double UnitPrice { get; set; }
        public string COGSCostingCode { get; set; }
        public string COGSAccountCode { get; set; }
        public string CostingCode2 { get; set; }
        public string CostingCode3 { get; set; }
        public string CostingCode4 { get; set; }
        public string CostingCode5 { get; set; }
        public string COGSCostingCode2 { get; set; }
        public string COGSCostingCode3 { get; set; }
        public string COGSCostingCode4 { get; set; }
        public string COGSCostingCode5 { get; set; }
        public string U_k33p_SFID { get; set; }
        public double U_k33p_SFUPrice { get; set; }

        public string U_LG_xPed { get; set; }
        public string U_LG_nItemPed { get; set; }
        public string U_CFS_SETOR_APLIC { get; set; }
        public string ShipDate { get; set; }
    }

    public class QuotationsDTORetorno
    {
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public string DocObjectCode { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public string DocCurrency { get; set; }
        public double? DocRate { get; set; }
        public string Comments { get; set; }

        public string NumAtCard { get; set; }
        public int PaymentGroupCode { get; set; }
        public int SalesPersonCode { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; }
        public string PaymentMethod { get; set; }
        public int BPL_IDAssignedToInvoice { get; set; }
        public string U_k33p_SFID { get; set; }
        public string U_k33p_SFSend { get; set; }
        public string U_k33p_SFSMsg { get; set; }

        //public int DocumentsOwner { get; set; }
        public List<DocumentLineRetorno> DocumentLines { get; set; }

        public TaxExtensionRetorno TaxExtension { get; set; }

        public ErrorRetorno2 error { get; set; }
    }

    public class TaxExtensionRetorno
    {
        //public string TaxId0 { get; set; }
        //public string TaxId1 { get; set; }

        public string Incoterms { get; set; }

    }

    public class ErrorRetorno2
    {
        public int code { get; set; }
        public MessageRetorno2 message { get; set; }
    }

    public class MessageRetorno2
    {
        public string lang { get; set; }
        public string value { get; set; }
    }




}
