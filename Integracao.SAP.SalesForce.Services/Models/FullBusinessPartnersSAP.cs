using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models.Full
{


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BPAccountReceivablePaybleCollection
    {
        public string AccountType { get; set; }
        public string AccountCode { get; set; }
        public string BPCode { get; set; }
    }

    public class BPAddressPatch
    {
        public string AddressName { get; set; }
        public string Street { get; set; }
        public string Block { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public object FederalTaxID { get; set; }
        public object TaxCode { get; set; }
        public string BuildingFloorRoom { get; set; }
        public string AddressType { get; set; }
        public object AddressName2 { get; set; }
        public object AddressName3 { get; set; }
        public string TypeOfAddress { get; set; }
        public string StreetNo { get; set; }
        public string BPCode { get; set; }
        public int? RowNum { get; set; }
        public object GlobalLocationNumber { get; set; }
        public object Nationality { get; set; }
        public object TaxOffice { get; set; }
        public object GSTIN { get; set; }
        public object GstType { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public object MYFType { get; set; }
        public string TaasEnabled { get; set; }
    }

    public class BPBranchAssignment
    {
        public string BPCode { get; set; }
        public int? BPLID { get; set; }
        public string DisabledForBP { get; set; }
    }

    public class BPFiscalTaxIDCollectionPatch
    {
        public string Address { get; set; }
        public object CNAECode { get; set; }
        public string TaxId0 { get; set; }
        public string TaxId1 { get; set; }
        public object TaxId2 { get; set; }
        public object TaxId3 { get; set; }
        public object TaxId4 { get; set; }
        public object TaxId5 { get; set; }
        public object TaxId6 { get; set; }
        public object TaxId7 { get; set; }
        public object TaxId8 { get; set; }
        public object TaxId9 { get; set; }
        public object TaxId10 { get; set; }
        public object TaxId11 { get; set; }
        public string BPCode { get; set; }
        public string AddrType { get; set; }
        public string TaxId12 { get; set; }
        public object TaxId13 { get; set; }
        public string AToRetrNFe { get; set; }
    }

    public class BPIntrastatExtension
    {
    }



    public class BPPaymentMethod
    {
        public string PaymentMethodCode { get; set; }
        public int? RowNumber { get; set; }
        public string BPCode { get; set; }
    }

    public class ContactEmployeePatch
    {
        public string CardCode { get; set; }
        public string Name { get; set; }
        public object Position { get; set; }
        public object Address { get; set; }
        public string Phone1 { get; set; }
        public object Phone2 { get; set; }
        public string MobilePhone { get; set; }
        public object Fax { get; set; }
        public string E_Mail { get; set; }
        public object Pager { get; set; }
        public object Remarks1 { get; set; }
        public object Remarks2 { get; set; }
        public object Password { get; set; }
        public int InternalCode { get; set; }
        public object PlaceOfBirth { get; set; }
        public object DateOfBirth { get; set; }
        public string Gender { get; set; }
        public object Profession { get; set; }
        public object Title { get; set; }
        public object CityOfBirth { get; set; }
        public string Active { get; set; }
        public string FirstName { get; set; }
        public object MiddleName { get; set; }
        public string LastName { get; set; }
        public object EmailGroupCode { get; set; }
        public string BlockSendingMarketingContent { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public object ConnectedAddressName { get; set; }
        public object ConnectedAddressType { get; set; }
        public object ForeignCountry { get; set; }
        public string U_SBZ_EnvEml { get; set; }

        //public string U_EhBeneficPag { get; set; }

        public string U_LG_RecEmailXML { get; set; }
        public object U_k33p_SFID { get; set; }
        public List<object> ContactEmployeeBlockSendingMarketingContents { get; set; }
    }

    public class FullBusinessPartnersSAP
    {
        //[JsonProperty("odata.metadata")]
        //public string odatametadata { get; set; }

        //[JsonProperty("odata.etag")]
        //public string odataetag { get; set; }
        public string U_Rut { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public int? GroupCode { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string MailAddress { get; set; }
        public string MailZipCode { get; set; }
        public string Phone1 { get; set; }
        public object Phone2 { get; set; }
        public object Fax { get; set; }
        public string ContactPerson { get; set; }
        public object Notes { get; set; }
        public int PayTermsGrpCode { get; set; }
        public double CreditLimit { get; set; }
        public double MaxCommitment { get; set; }
        public double DiscountPercent { get; set; }
        public string VatLiable { get; set; }
        public string FederalTaxID { get; set; }
        public string DeductibleAtSource { get; set; }
        public double DeductionPercent { get; set; }
        public object DeductionValidUntil { get; set; }
        public int? PriceListNum { get; set; }
        public double IntrestRatePercent { get; set; }
        public double CommissionPercent { get; set; }
        public int CommissionGroupCode { get; set; }
        public object FreeText { get; set; }
        public int? SalesPersonCode { get; set; }
        public string Currency { get; set; }
        public object RateDiffAccount { get; set; }
        public object Cellular { get; set; }
        public object AvarageLate { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string MailCity { get; set; }
        public string MailCounty { get; set; }
        public string MailCountry { get; set; }
        public string EmailAddress { get; set; }
        public object Picture { get; set; }
        public object DefaultAccount { get; set; }
        public object DefaultBranch { get; set; }
        public string DefaultBankCode { get; set; }
        public object AdditionalID { get; set; }
        public object Pager { get; set; }
        public object FatherCard { get; set; }
        public string CardForeignName { get; set; }
        public string FatherType { get; set; }
        public object DeductionOffice { get; set; }
        public object ExportCode { get; set; }
        public double MinIntrest { get; set; }
        public double CurrentAccountBalance { get; set; }
        public double OpenDeliveryNotesBalance { get; set; }
        public double OpenOrdersBalance { get; set; }
        public double OpenChecksBalance { get; set; }
        public object VatGroup { get; set; }
        public object ShippingType { get; set; }
        public object Password { get; set; }
        public object Indicator { get; set; }
        public object IBAN { get; set; }
        public int CreditCardCode { get; set; }
        public string CreditCardNum { get; set; }
        public object CreditCardExpiration { get; set; }
        public string DebitorAccount { get; set; }
        public int? OpenOpportunities { get; set; }
        public string Valid { get; set; }
        public object ValidFrom { get; set; }
        public object ValidTo { get; set; }
        public object ValidRemarks { get; set; }
        public string Frozen { get; set; }
        public object FrozenFrom { get; set; }
        public object FrozenTo { get; set; }
        public object FrozenRemarks { get; set; }
        public string Block { get; set; }
        public string BillToState { get; set; }
        public string ShipToState { get; set; }
        public object ExemptNum { get; set; }
        public int? Priority { get; set; }
        public object FormCode1099 { get; set; }
        public object Box1099 { get; set; }
        public string PeymentMethodCode { get; set; }
        public string BackOrder { get; set; }
        public string PartialDelivery { get; set; }
        public string BlockDunning { get; set; }
        public object BankCountry { get; set; }
        public string HouseBank { get; set; }
        public string HouseBankCountry { get; set; }
        public string HouseBankAccount { get; set; }
        public string ShipToDefault { get; set; }
        public object DunningLevel { get; set; }
        public object DunningDate { get; set; }
        public string CollectionAuthorization { get; set; }
        public object DME { get; set; }
        public object InstructionKey { get; set; }
        public string SinglePayment { get; set; }
        public object ISRBillerID { get; set; }
        public string PaymentBlock { get; set; }
        public object ReferenceDetails { get; set; }
        public string HouseBankBranch { get; set; }
        public object OwnerIDNumber { get; set; }
        public int PaymentBlockDescription { get; set; }
        public object TaxExemptionLetterNum { get; set; }
        public double MaxAmountOfExemption { get; set; }
        public object ExemptionValidityDateFrom { get; set; }
        public object ExemptionValidityDateTo { get; set; }
        public object LinkedBusinessPartner { get; set; }
        public object LastMultiReconciliationNum { get; set; }
        public string DeferredTax { get; set; }
        public string Equalization { get; set; }
        public string SubjectToWithholdingTax { get; set; }
        public object CertificateNumber { get; set; }
        public object ExpirationDate { get; set; }
        public object NationalInsuranceNum { get; set; }
        public string AccrualCriteria { get; set; }
        public object WTCode { get; set; }
        public string BillToBuildingFloorRoom { get; set; }
        public string DownPaymentClearAct { get; set; }
        public object ChannelBP { get; set; }
        public object DefaultTechnician { get; set; }
        public string BilltoDefault { get; set; }
        public object CustomerBillofExchangDisc { get; set; }
        public object Territory { get; set; }
        public string ShipToBuildingFloorRoom { get; set; }
        public object CustomerBillofExchangPres { get; set; }
        public object ProjectCode { get; set; }
        public object VatGroupLatinAmerica { get; set; }
        public object DunningTerm { get; set; }
        public object Website { get; set; }
        public object OtherReceivablePayable { get; set; }
        public object BillofExchangeonCollection { get; set; }
        public string CompanyPrivate { get; set; }
        public int? LanguageCode { get; set; }
        public object UnpaidBillofExchange { get; set; }
        public int WithholdingTaxDeductionGroup { get; set; }
        public object ClosingDateProcedureNumber { get; set; }
        public object Profession { get; set; }
        public object BankChargesAllocationCode { get; set; }
        public string TaxRoundingRule { get; set; }
        public string Properties1 { get; set; }
        public string Properties2 { get; set; }
        public string Properties3 { get; set; }
        public string Properties4 { get; set; }
        public string Properties5 { get; set; }
        public string Properties6 { get; set; }
        public string Properties7 { get; set; }
        public string Properties8 { get; set; }
        public string Properties9 { get; set; }
        public string Properties10 { get; set; }
        public string Properties11 { get; set; }
        public string Properties12 { get; set; }
        public string Properties13 { get; set; }
        public string Properties14 { get; set; }
        public string Properties15 { get; set; }
        public string Properties16 { get; set; }
        public string Properties17 { get; set; }
        public string Properties18 { get; set; }
        public string Properties19 { get; set; }
        public string Properties20 { get; set; }
        public string Properties21 { get; set; }
        public string Properties22 { get; set; }
        public string Properties23 { get; set; }
        public string Properties24 { get; set; }
        public string Properties25 { get; set; }
        public string Properties26 { get; set; }
        public string Properties27 { get; set; }
        public string Properties28 { get; set; }
        public string Properties29 { get; set; }
        public string Properties30 { get; set; }
        public string Properties31 { get; set; }
        public string Properties32 { get; set; }
        public string Properties33 { get; set; }
        public string Properties34 { get; set; }
        public string Properties35 { get; set; }
        public string Properties36 { get; set; }
        public string Properties37 { get; set; }
        public string Properties38 { get; set; }
        public string Properties39 { get; set; }
        public string Properties40 { get; set; }
        public string Properties41 { get; set; }
        public string Properties42 { get; set; }
        public string Properties43 { get; set; }
        public string Properties44 { get; set; }
        public string Properties45 { get; set; }
        public string Properties46 { get; set; }
        public string Properties47 { get; set; }
        public string Properties48 { get; set; }
        public string Properties49 { get; set; }
        public string Properties50 { get; set; }
        public string Properties51 { get; set; }
        public string Properties52 { get; set; }
        public string Properties53 { get; set; }
        public string Properties54 { get; set; }
        public string Properties55 { get; set; }
        public string Properties56 { get; set; }
        public string Properties57 { get; set; }
        public string Properties58 { get; set; }
        public string Properties59 { get; set; }
        public string Properties60 { get; set; }
        public string Properties61 { get; set; }
        public string Properties62 { get; set; }
        public string Properties63 { get; set; }
        public string Properties64 { get; set; }
        public object CompanyRegistrationNumber { get; set; }
        public object VerificationNumber { get; set; }
        public string DiscountBaseObject { get; set; }
        public string DiscountRelations { get; set; }
        public string TypeReport { get; set; }
        public string ThresholdOverlook { get; set; }
        public string SurchargeOverlook { get; set; }
        public object DownPaymentInterimAccount { get; set; }
        public string OperationCode347 { get; set; }
        public string InsuranceOperation347 { get; set; }
        public string HierarchicalDeduction { get; set; }
        public string ShaamGroup { get; set; }
        public string WithholdingTaxCertified { get; set; }
        public string BookkeepingCertified { get; set; }
        public object PlanningGroup { get; set; }
        public string Affiliate { get; set; }
        public int? Industry { get; set; }
        public object VatIDNum { get; set; }
        public object DatevAccount { get; set; }
        public string DatevFirstDataEntry { get; set; }
        public string UseShippedGoodsAccount { get; set; }
        public object GTSRegNo { get; set; }
        public object GTSBankAccountNo { get; set; }
        public object GTSBillingAddrTel { get; set; }
        public object ETaxWebSite { get; set; }
        public object HouseBankIBAN { get; set; }
        public object VATRegistrationNumber { get; set; }
        public object RepresentativeName { get; set; }
        public object IndustryType { get; set; }
        public object BusinessType { get; set; }
        public int Series { get; set; }
        public string AutomaticPosting { get; set; }
        public object InterestAccount { get; set; }
        public object FeeAccount { get; set; }
        public object CampaignNumber { get; set; }
        public object AliasName { get; set; }
        public object DefaultBlanketAgreementNumber { get; set; }
        public string EffectiveDiscount { get; set; }
        public string NoDiscounts { get; set; }
        public string EffectivePrice { get; set; }
        public string EffectivePriceConsidersPriceBeforeDiscount { get; set; }
        public object GlobalLocationNumber { get; set; }
        public object EDISenderID { get; set; }
        public object EDIRecipientID { get; set; }
        public string ResidenNumber { get; set; }
        public object RelationshipCode { get; set; }
        public object RelationshipDateFrom { get; set; }
        public object RelationshipDateTill { get; set; }
        public object UnifiedFederalTaxID { get; set; }
        public int? AttachmentEntry { get; set; }
        public object TypeOfOperation { get; set; }
        public string EndorsableChecksFromBP { get; set; }
        public string AcceptsEndorsedChecks { get; set; }
        public object OwnerCode { get; set; }
        public string BlockSendingMarketingContent { get; set; }
        public object AgentCode { get; set; }
        public object PriceMode { get; set; }
        public object EDocGenerationType { get; set; }
        public object EDocStreet { get; set; }
        public object EDocStreetNumber { get; set; }
        public object EDocBuildingNumber { get; set; }
        public object EDocZipCode { get; set; }
        public object EDocCity { get; set; }
        public object EDocCountry { get; set; }
        public object EDocDistrict { get; set; }
        public object EDocRepresentativeFirstName { get; set; }
        public object EDocRepresentativeSurname { get; set; }
        public object EDocRepresentativeCompany { get; set; }
        public object EDocRepresentativeFiscalCode { get; set; }
        public object EDocRepresentativeAdditionalId { get; set; }
        public object EDocPECAddress { get; set; }
        public object IPACodeForPA { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string ExemptionMaxAmountValidationType { get; set; }
        public object ECommerceMerchantID { get; set; }
        public string UseBillToAddrToDetermineTax { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public object DefaultTransporterEntry { get; set; }
        public object DefaultTransporterLineNumber { get; set; }
        public string FCERelevant { get; set; }
        public string FCEValidateBaseDelivery { get; set; }
        public int? MainUsage { get; set; }
        public object EBooksVATExemptionCause { get; set; }
        public object LegalText { get; set; }
        public int? DataVersion { get; set; }
        public string ExchangeRateForIncomingPayment { get; set; }
        public string ExchangeRateForOutgoingPayment { get; set; }
        public object CertificateDetails { get; set; }
        public string DefaultCurrency { get; set; }
        public object EORINumber { get; set; }
        public string FCEAsPaymentMeans { get; set; }
        public object U_LG_CardCodeC { get; set; }
        public object U_LG_CardNameC { get; set; }
        public object U_LG_Cert { get; set; }
        public object U_LG_VencCert { get; set; }
        public object U_LG_Avaliado { get; set; }
        public double U_LG_Ponto1 { get; set; }
        public double U_LG_Ponto2 { get; set; }
        public double U_LG_Ponto3 { get; set; }
        public double U_LG_Ponto4 { get; set; }
        public double U_LG_Ponto5 { get; set; }
        public double U_LG_Ponto6 { get; set; }
        public double U_LG_Total { get; set; }
        public object U_LG_Obs { get; set; }
        public object U_LG_EndEntrega { get; set; }
        public string U_LG_FichaRec { get; set; }
        public string U_LG_Avaliar { get; set; }
        public object U_LG_StatusQF { get; set; }
        public object U_LGO_CNO { get; set; }
        public object U_LGO_AgrPick1 { get; set; }
        public object U_LGO_AgrPick2 { get; set; }
        public object U_LGO_AgrPick3 { get; set; }
        public object U_LGO_AgrPick4 { get; set; }
        public string U_LG_EmailCC { get; set; }
        public object U_LGO_MotDesICMS { get; set; }
        public object U_LGO_IndFinal { get; set; }
        public object U_LGO_UsageImp { get; set; }
        public string U_LGO_Sintegra { get; set; }
        public object U_RNTRC { get; set; }
        public object U_LGO_NFeindIEDest { get; set; }
        public string U_Setor { get; set; }
        public string U_SBZ_CodCart { get; set; }
        public string U_SBZ_GerBol { get; set; }
        public object U_SBZ_CCCar { get; set; }
        public object U_SBZ_CCNome { get; set; }
        public object U_SBZ_CCNro { get; set; }
        public object U_SBZ_CCVal { get; set; }
        public object U_SBZ_CCSeg { get; set; }
        public string U_SBZ_PerAgr { get; set; }
        public string U_SBZ_EnvEml { get; set; }
        public string U_SBZ_RecAlrt { get; set; }
        public object U_PrDesc { get; set; }
        public object U_SBZ_DACliEmp { get; set; }
        public object U_SBZ_DACliBco { get; set; }
        public object U_LG_Ocomp { get; set; }
        public object U_LGO_OcrCode { get; set; }
        public object U_LGO_OcrCode2 { get; set; }
        public object U_LGO_OcrCode3 { get; set; }
        public object U_LGO_OcrCode4 { get; set; }
        public object U_LGO_OcrCode5 { get; set; }
        public object U_LGO_CodCordPed { get; set; }
        public object U_LGO_CodAprovad { get; set; }
        public object U_LGO_UsuarioWEB { get; set; }
        public object U_LGO_UsuarioSAP { get; set; }
        public string U_CFS_DEPART { get; set; }
        public string U_CFS_OCRCODE1 { get; set; }
        public string U_CFS_OCRCODE2 { get; set; }
        public object U_CFS_OCRCODE3 { get; set; }
        public object U_CFS_CONSOLID { get; set; }
        public object U_LGO_DescPIS { get; set; }
        public object U_LGO_DescCOFINS { get; set; }
        public object U_LGO_DescICMS { get; set; }
        public object U_LGO_InfoProprietario { get; set; }
        public object U_LGO_IND_NAT_RET { get; set; }
        public object U_LGO_IndIntermed { get; set; }
        public object U_LGO_CNPJIntermed { get; set; }
        public object U_LGO_IdCadIntTran { get; set; }
        public object U_LGO_GrpFiscal { get; set; }
        public object U_LGO_DataQF { get; set; }
        public object U_LGO_QFEntry { get; set; }
        public object U_LGO_NFeIsSimplesN { get; set; }
        public object U_LGO_NFeDTIsSimplesN { get; set; }
        public object U_LGO_NFeDTConsultaPN { get; set; }
        public object U_LGO_MDFeTpTransp { get; set; }
        public object U_LGO_ReinfIndAqui { get; set; }
        public object U_LGO_ReinfIndOpcCP { get; set; }
        public string U_CFS_SemNota { get; set; }
        public string U_Obs_Cliente { get; set; }
        public object U_LGO_TiProdRural { get; set; }
        public object U_Cond_Dia_Fixo { get; set; }
        public object U_LGO_ValidMatriz { get; set; }
        public object U_CFS_Categoria { get; set; }
        public object U_SBZ_ModNFe { get; set; }
        public object U_SBZ_ModNFSe { get; set; }
        public object U_SBZ_ModVencer { get; set; }
        public object U_SBZ_ModVencidos { get; set; }
        public object U_LGO_DepConsig { get; set; }
        public object U_LGO_DataConsultaSintegra { get; set; }
        public object U_k33p_SFID { get; set; }
        public object U_k33p_SFSend { get; set; }
        public object U_k33p_SFSMsg { get; set; }
        public object U_LGO_UserUpd { get; set; }
        public string U_IB_BoletoGeradoPor { get; set; }
        public object U_ImprimirBoleto { get; set; }
        public object U_LGO_ReinfNatRend { get; set; }
        public object U_LGO_ReinfIndNIF { get; set; }
        public object U_LGO_ReinfNumNIF { get; set; }
        public object U_LGO_ReinfFrmTribut { get; set; }
        public object U_LGO_ReinfRelFontPg { get; set; }
      
        public List<object> ElectronicProtocols { get; set; }
        public List<BPAddressPatch> BPAddresses { get; set; }
        public List<ContactEmployeePatch> ContactEmployees { get; set; }
        public List<BPAccountReceivablePaybleCollection> BPAccountReceivablePaybleCollection { get; set; }
        public List<BPPaymentMethod> BPPaymentMethods { get; set; }
        public List<object> BPWithholdingTaxCollection { get; set; }
        public List<object> BPPaymentDates { get; set; }
        public List<BPBranchAssignment> BPBranchAssignment { get; set; }
        public List<object> BPBankAccounts { get; set; }
        public List<BPFiscalTaxIDCollectionPatch> BPFiscalTaxIDCollection { get; set; }
        public List<object> DiscountGroups { get; set; }
        public BPIntrastatExtension BPIntrastatExtension { get; set; }
        public List<object> BPBlockSendingMarketingContents { get; set; }
        public List<object> BPCurrenciesCollection { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    //public class BPAccountReceivablePaybleCollection
    //{
    //    public string AccountType { get; set; }
    //    public string AccountCode { get; set; }
    //    public string BPCode { get; set; }
    //}

    //public class BPAddressPatch
    //{
    //    public string AddressName { get; set; }
    //    public string Street { get; set; }
    //    public string Block { get; set; }
    //    public string ZipCode { get; set; }
    //    public string City { get; set; }
    //    public string County { get; set; }
    //    public string Country { get; set; }
    //    public string State { get; set; }
    //    public string FederalTaxID { get; set; }
    //    public object TaxCode { get; set; }
    //    public string BuildingFloorRoom { get; set; }
    //    public string AddressType { get; set; }
    //    public object AddressName2 { get; set; }
    //    public object AddressName3 { get; set; }
    //    public string TypeOfAddress { get; set; }
    //    public string StreetNo { get; set; }
    //    public string BPCode { get; set; }
    //    public int RowNum { get; set; }
    //    public object GlobalLocationNumber { get; set; }
    //    public object Nationality { get; set; }
    //    public object TaxOffice { get; set; }
    //    public object GSTIN { get; set; }
    //    public object GstType { get; set; }
    //    public string CreateDate { get; set; }
    //    public string CreateTime { get; set; }
    //    public object MYFType { get; set; }
    //    public string TaasEnabled { get; set; }
    //}

    //public class BPBranchAssignment
    //{
    //    public string BPCode { get; set; }
    //    public int BPLID { get; set; }
    //    public string DisabledForBP { get; set; }
    //}

    //public class BPCurrenciesCollection
    //{
    //    public string CurrencyCode { get; set; }
    //    public string Include { get; set; }
    //}

    //public class BPFiscalTaxIDCollectionPatch
    //{
    //    public string Address { get; set; }
    //    public int? CNAECode { get; set; }
    //    public string TaxId0 { get; set; }
    //    public string TaxId1 { get; set; }
    //    public string TaxId2 { get; set; }
    //    public string TaxId3 { get; set; }
    //    public string TaxId4 { get; set; }
    //    public string TaxId5 { get; set; }
    //    public string TaxId6 { get; set; }
    //    public string TaxId7 { get; set; }
    //    public string TaxId8 { get; set; }
    //    public object TaxId9 { get; set; }
    //    public object TaxId10 { get; set; }
    //    public object TaxId11 { get; set; }
    //    public string BPCode { get; set; }
    //    public string AddrType { get; set; }
    //    public string TaxId12 { get; set; }
    //    public object TaxId13 { get; set; }
    //    public string AToRetrNFe { get; set; }
    //}

    //public class BPIntrastatExtension
    //{
    //}

    //public class ContactEmployeePatch
    //{
    //    public string CardCode { get; set; }
    //    public string Name { get; set; }
    //    public object Position { get; set; }
    //    public object Address { get; set; }
    //    public string Phone1 { get; set; }
    //    public object Phone2 { get; set; }
    //    public object MobilePhone { get; set; }
    //    public object Fax { get; set; }
    //    public string E_Mail { get; set; }
    //    public object Pager { get; set; }
    //    public object Remarks1 { get; set; }
    //    public object Remarks2 { get; set; }
    //    public object Password { get; set; }
    //    public int InternalCode { get; set; }
    //    public string PlaceOfBirth { get; set; }
    //    public object DateOfBirth { get; set; }
    //    public string Gender { get; set; }
    //    public object Profession { get; set; }
    //    public object Title { get; set; }
    //    public object CityOfBirth { get; set; }
    //    public string Active { get; set; }
    //    public string FirstName { get; set; }
    //    public object MiddleName { get; set; }
    //    public object LastName { get; set; }
    //    public object EmailGroupCode { get; set; }
    //    public string BlockSendingMarketingContent { get; set; }
    //    public string CreateDate { get; set; }
    //    public string CreateTime { get; set; }
    //    public string UpdateDate { get; set; }
    //    public string UpdateTime { get; set; }
    //    public object ConnectedAddressName { get; set; }
    //    public object ConnectedAddressType { get; set; }
    //    public object ForeignCountry { get; set; }
    //    public string U_SBZ_EnvEml { get; set; }
    //    public object U_k33p_SFID { get; set; }
    //    public List<object> ContactEmployeeBlockSendingMarketingContents { get; set; }
    //}

    //public class FullBusinessPartnersSAP
    //{
    //    [JsonProperty("odata.metadata")]
    //    public string odatametadata { get; set; }

    //    [JsonProperty("odata.etag")]
    //    public string odataetag { get; set; }
    //    public string CardCode { get; set; }
    //    public string CardName { get; set; }
    //    public string CardType { get; set; }
    //    public int GroupCode { get; set; }
    //    public string Address { get; set; }
    //    public string ZipCode { get; set; }
    //    public string MailAddress { get; set; }
    //    public string MailZipCode { get; set; }
    //    public string Phone1 { get; set; }
    //    public string Phone2 { get; set; }
    //    public object Fax { get; set; }
    //    public string ContactPerson { get; set; }
    //    public object Notes { get; set; }
    //    public int PayTermsGrpCode { get; set; }
    //    public double CreditLimit { get; set; }
    //    public double MaxCommitment { get; set; }
    //    public double DiscountPercent { get; set; }
    //    public string VatLiable { get; set; }
    //    public string FederalTaxID { get; set; }
    //    public string DeductibleAtSource { get; set; }
    //    public double DeductionPercent { get; set; }
    //    public object DeductionValidUntil { get; set; }
    //    public int PriceListNum { get; set; }
    //    public double IntrestRatePercent { get; set; }
    //    public double CommissionPercent { get; set; }
    //    public int CommissionGroupCode { get; set; }
    //    public object FreeText { get; set; }
    //    public int SalesPersonCode { get; set; }
    //    public string Currency { get; set; }
    //    public string RateDiffAccount { get; set; }
    //    public object Cellular { get; set; }
    //    public object AvarageLate { get; set; }
    //    public string City { get; set; }
    //    public string County { get; set; }
    //    public string Country { get; set; }
    //    public string MailCity { get; set; }
    //    public string MailCounty { get; set; }
    //    public string MailCountry { get; set; }
    //    public string EmailAddress { get; set; }
    //    public object Picture { get; set; }
    //    public object DefaultAccount { get; set; }
    //    public object DefaultBranch { get; set; }
    //    public string DefaultBankCode { get; set; }
    //    public object AdditionalID { get; set; }
    //    public object Pager { get; set; }
    //    public object FatherCard { get; set; }
    //    public string CardForeignName { get; set; }
    //    public string FatherType { get; set; }
    //    public object DeductionOffice { get; set; }
    //    public object ExportCode { get; set; }
    //    public double MinIntrest { get; set; }
    //    public double CurrentAccountBalance { get; set; }
    //    public double OpenDeliveryNotesBalance { get; set; }
    //    public double OpenOrdersBalance { get; set; }
    //    public double OpenChecksBalance { get; set; }
    //    public object VatGroup { get; set; }
    //    public object ShippingType { get; set; }
    //    public object Password { get; set; }
    //    public object Indicator { get; set; }
    //    public object IBAN { get; set; }
    //    public int CreditCardCode { get; set; }
    //    public string CreditCardNum { get; set; }
    //    public object CreditCardExpiration { get; set; }
    //    public string DebitorAccount { get; set; }
    //    //public int OpenOpportunities { get; set; }
    //    public string Valid { get; set; }
    //    public object ValidFrom { get; set; }
    //    public object ValidTo { get; set; }
    //    public object ValidRemarks { get; set; }
    //    public string Frozen { get; set; }
    //    public object FrozenFrom { get; set; }
    //    public object FrozenTo { get; set; }
    //    public object FrozenRemarks { get; set; }
    //    public string Block { get; set; }
    //    public string BillToState { get; set; }
    //    public string ShipToState { get; set; }
    //    public object ExemptNum { get; set; }
    //    public int Priority { get; set; }
    //    public object FormCode1099 { get; set; }
    //    public object Box1099 { get; set; }
    //    public object PeymentMethodCode { get; set; }
    //    public string BackOrder { get; set; }
    //    public string PartialDelivery { get; set; }
    //    public string BlockDunning { get; set; }
    //    public object BankCountry { get; set; }
    //    public string HouseBank { get; set; }
    //    public string HouseBankCountry { get; set; }
    //    public string HouseBankAccount { get; set; }
    //    public string ShipToDefault { get; set; }
    //    public object DunningLevel { get; set; }
    //    public object DunningDate { get; set; }
    //    public string CollectionAuthorization { get; set; }
    //    public object DME { get; set; }
    //    public object InstructionKey { get; set; }
    //    public string SinglePayment { get; set; }
    //    public object ISRBillerID { get; set; }
    //    public string PaymentBlock { get; set; }
    //    public object ReferenceDetails { get; set; }
    //    public string HouseBankBranch { get; set; }
    //    public object OwnerIDNumber { get; set; }
    //    public int PaymentBlockDescription { get; set; }
    //    public object TaxExemptionLetterNum { get; set; }
    //    public double MaxAmountOfExemption { get; set; }
    //    public object ExemptionValidityDateFrom { get; set; }
    //    public object ExemptionValidityDateTo { get; set; }
    //    public object LinkedBusinessPartner { get; set; }
    //    public object LastMultiReconciliationNum { get; set; }
    //    public string DeferredTax { get; set; }
    //    public string Equalization { get; set; }
    //    public string SubjectToWithholdingTax { get; set; }
    //    public object CertificateNumber { get; set; }
    //    public object ExpirationDate { get; set; }
    //    public object NationalInsuranceNum { get; set; }
    //    public string AccrualCriteria { get; set; }
    //    public object WTCode { get; set; }
    //    public string BillToBuildingFloorRoom { get; set; }
    //    public string DownPaymentClearAct { get; set; }
    //    public object ChannelBP { get; set; }
    //    public object DefaultTechnician { get; set; }
    //    public string BilltoDefault { get; set; }
    //    public string CustomerBillofExchangDisc { get; set; }
    //    public object Territory { get; set; }
    //    public string ShipToBuildingFloorRoom { get; set; }
    //    public object CustomerBillofExchangPres { get; set; }
    //    public object ProjectCode { get; set; }
    //    public object VatGroupLatinAmerica { get; set; }
    //    public object DunningTerm { get; set; }
    //    public object Website { get; set; }
    //    public object OtherReceivablePayable { get; set; }
    //    public object BillofExchangeonCollection { get; set; }
    //    public string CompanyPrivate { get; set; }
    //    public int LanguageCode { get; set; }
    //    public object UnpaidBillofExchange { get; set; }
    //    public int WithholdingTaxDeductionGroup { get; set; }
    //    public object ClosingDateProcedureNumber { get; set; }
    //    public object Profession { get; set; }
    //    public object BankChargesAllocationCode { get; set; }
    //    public string TaxRoundingRule { get; set; }
    //    public string Properties1 { get; set; }
    //    public string Properties2 { get; set; }
    //    public string Properties3 { get; set; }
    //    public string Properties4 { get; set; }
    //    public string Properties5 { get; set; }
    //    public string Properties6 { get; set; }
    //    public string Properties7 { get; set; }
    //    public string Properties8 { get; set; }
    //    public string Properties9 { get; set; }
    //    public string Properties10 { get; set; }
    //    public string Properties11 { get; set; }
    //    public string Properties12 { get; set; }
    //    public string Properties13 { get; set; }
    //    public string Properties14 { get; set; }
    //    public string Properties15 { get; set; }
    //    public string Properties16 { get; set; }
    //    public string Properties17 { get; set; }
    //    public string Properties18 { get; set; }
    //    public string Properties19 { get; set; }
    //    public string Properties20 { get; set; }
    //    public string Properties21 { get; set; }
    //    public string Properties22 { get; set; }
    //    public string Properties23 { get; set; }
    //    public string Properties24 { get; set; }
    //    public string Properties25 { get; set; }
    //    public string Properties26 { get; set; }
    //    public string Properties27 { get; set; }
    //    public string Properties28 { get; set; }
    //    public string Properties29 { get; set; }
    //    public string Properties30 { get; set; }
    //    public string Properties31 { get; set; }
    //    public string Properties32 { get; set; }
    //    public string Properties33 { get; set; }
    //    public string Properties34 { get; set; }
    //    public string Properties35 { get; set; }
    //    public string Properties36 { get; set; }
    //    public string Properties37 { get; set; }
    //    public string Properties38 { get; set; }
    //    public string Properties39 { get; set; }
    //    public string Properties40 { get; set; }
    //    public string Properties41 { get; set; }
    //    public string Properties42 { get; set; }
    //    public string Properties43 { get; set; }
    //    public string Properties44 { get; set; }
    //    public string Properties45 { get; set; }
    //    public string Properties46 { get; set; }
    //    public string Properties47 { get; set; }
    //    public string Properties48 { get; set; }
    //    public string Properties49 { get; set; }
    //    public string Properties50 { get; set; }
    //    public string Properties51 { get; set; }
    //    public string Properties52 { get; set; }
    //    public string Properties53 { get; set; }
    //    public string Properties54 { get; set; }
    //    public string Properties55 { get; set; }
    //    public string Properties56 { get; set; }
    //    public string Properties57 { get; set; }
    //    public string Properties58 { get; set; }
    //    public string Properties59 { get; set; }
    //    public string Properties60 { get; set; }
    //    public string Properties61 { get; set; }
    //    public string Properties62 { get; set; }
    //    public string Properties63 { get; set; }
    //    public string Properties64 { get; set; }
    //    public object CompanyRegistrationNumber { get; set; }
    //    public object VerificationNumber { get; set; }
    //    public string DiscountBaseObject { get; set; }
    //    public string DiscountRelations { get; set; }
    //    public string TypeReport { get; set; }
    //    public string ThresholdOverlook { get; set; }
    //    public string SurchargeOverlook { get; set; }
    //    public string DownPaymentInterimAccount { get; set; }
    //    public string OperationCode347 { get; set; }
    //    public string InsuranceOperation347 { get; set; }
    //    public string HierarchicalDeduction { get; set; }
    //    public string ShaamGroup { get; set; }
    //    public string WithholdingTaxCertified { get; set; }
    //    public string BookkeepingCertified { get; set; }
    //    public object PlanningGroup { get; set; }
    //    public string Affiliate { get; set; }
    //    public int Industry { get; set; }
    //    public object VatIDNum { get; set; }
    //    public object DatevAccount { get; set; }
    //    public string DatevFirstDataEntry { get; set; }
    //    public string UseShippedGoodsAccount { get; set; }
    //    public object GTSRegNo { get; set; }
    //    public object GTSBankAccountNo { get; set; }
    //    public object GTSBillingAddrTel { get; set; }
    //    public object ETaxWebSite { get; set; }
    //    public object HouseBankIBAN { get; set; }
    //    public object VATRegistrationNumber { get; set; }
    //    public object RepresentativeName { get; set; }
    //    public object IndustryType { get; set; }
    //    public object BusinessType { get; set; }
    //    public int Series { get; set; }
    //    public object AutomaticPosting { get; set; }
    //    public object InterestAccount { get; set; }
    //    public object FeeAccount { get; set; }
    //    public object CampaignNumber { get; set; }
    //    public object AliasName { get; set; }
    //    public object DefaultBlanketAgreementNumber { get; set; }
    //    public string EffectiveDiscount { get; set; }
    //    public string NoDiscounts { get; set; }
    //    public string EffectivePrice { get; set; }
    //    public string EffectivePriceConsidersPriceBeforeDiscount { get; set; }
    //    public object GlobalLocationNumber { get; set; }
    //    public object EDISenderID { get; set; }
    //    public object EDIRecipientID { get; set; }
    //    public string ResidenNumber { get; set; }
    //    public object RelationshipCode { get; set; }
    //    public object RelationshipDateFrom { get; set; }
    //    public object RelationshipDateTill { get; set; }
    //    public object UnifiedFederalTaxID { get; set; }
    //    public object AttachmentEntry { get; set; }
    //    public object TypeOfOperation { get; set; }
    //    public string EndorsableChecksFromBP { get; set; }
    //    public string AcceptsEndorsedChecks { get; set; }
    //    public object OwnerCode { get; set; }
    //    public string BlockSendingMarketingContent { get; set; }
    //    public object AgentCode { get; set; }
    //    public object PriceMode { get; set; }
    //    public object EDocGenerationType { get; set; }
    //    public object EDocStreet { get; set; }
    //    public object EDocStreetNumber { get; set; }
    //    public object EDocBuildingNumber { get; set; }
    //    public object EDocZipCode { get; set; }
    //    public object EDocCity { get; set; }
    //    public object EDocCountry { get; set; }
    //    public object EDocDistrict { get; set; }
    //    public object EDocRepresentativeFirstName { get; set; }
    //    public object EDocRepresentativeSurname { get; set; }
    //    public object EDocRepresentativeCompany { get; set; }
    //    public object EDocRepresentativeFiscalCode { get; set; }
    //    public object EDocRepresentativeAdditionalId { get; set; }
    //    public object EDocPECAddress { get; set; }
    //    public object IPACodeForPA { get; set; }
    //    public string UpdateDate { get; set; }
    //    public string UpdateTime { get; set; }
    //    public string ExemptionMaxAmountValidationType { get; set; }
    //    public object ECommerceMerchantID { get; set; }
    //    public string UseBillToAddrToDetermineTax { get; set; }
    //    public string CreateDate { get; set; }
    //    public string CreateTime { get; set; }
    //    public object DefaultTransporterEntry { get; set; }
    //    public object DefaultTransporterLineNumber { get; set; }
    //    public string FCERelevant { get; set; }
    //    public string FCEValidateBaseDelivery { get; set; }
    //    public object MainUsage { get; set; }
    //    public object EBooksVATExemptionCause { get; set; }
    //    public object LegalText { get; set; }
    //    public int DataVersion { get; set; }
    //    public string ExchangeRateForIncomingPayment { get; set; }
    //    public string ExchangeRateForOutgoingPayment { get; set; }
    //    public object CertificateDetails { get; set; }
    //    public string DefaultCurrency { get; set; }
    //    public object EORINumber { get; set; }
    //    public string FCEAsPaymentMeans { get; set; }
    //    public object U_LG_CardCodeC { get; set; }
    //    public object U_LG_CardNameC { get; set; }
    //    public object U_LG_Cert { get; set; }
    //    public object U_LG_VencCert { get; set; }
    //    public object U_LG_Avaliado { get; set; }
    //    public double U_LG_Ponto1 { get; set; }
    //    public double U_LG_Ponto2 { get; set; }
    //    public double U_LG_Ponto3 { get; set; }
    //    public double U_LG_Ponto4 { get; set; }
    //    public double U_LG_Ponto5 { get; set; }
    //    public double U_LG_Ponto6 { get; set; }
    //    public double U_LG_Total { get; set; }
    //    public object U_LG_Obs { get; set; }
    //    public object U_LG_EndEntrega { get; set; }
    //    public string U_LG_FichaRec { get; set; }
    //    public string U_LG_Avaliar { get; set; }
    //    public object U_LG_StatusQF { get; set; }
    //    public object U_LGO_CNO { get; set; }
    //    public object U_LGO_AgrPick1 { get; set; }
    //    public object U_LGO_AgrPick2 { get; set; }
    //    public object U_LGO_AgrPick3 { get; set; }
    //    public object U_LGO_AgrPick4 { get; set; }
    //    public string U_LG_EmailCC { get; set; }
    //    public object U_LGO_MotDesICMS { get; set; }
    //    public object U_LGO_IndFinal { get; set; }
    //    public object U_LGO_UsageImp { get; set; }
    //    public string U_LGO_Sintegra { get; set; }
    //    public object U_RNTRC { get; set; }
    //    public object U_LGO_NFeindIEDest { get; set; }
    //    public string U_Setor { get; set; }
    //    public object U_SBZ_CodCart { get; set; }
    //    public string U_SBZ_GerBol { get; set; }
    //    public object U_SBZ_CCCar { get; set; }
    //    public object U_SBZ_CCNome { get; set; }
    //    public object U_SBZ_CCNro { get; set; }
    //    public object U_SBZ_CCVal { get; set; }
    //    public object U_SBZ_CCSeg { get; set; }
    //    public string U_SBZ_PerAgr { get; set; }
    //    public string U_SBZ_EnvEml { get; set; }
    //    public string U_SBZ_RecAlrt { get; set; }
    //    public double U_PrDesc { get; set; }
    //    public object U_SBZ_DACliEmp { get; set; }
    //    public object U_SBZ_DACliBco { get; set; }
    //    public string U_LG_Ocomp { get; set; }
    //    public object U_LGO_OcrCode { get; set; }
    //    public object U_LGO_OcrCode2 { get; set; }
    //    public object U_LGO_OcrCode3 { get; set; }
    //    public object U_LGO_OcrCode4 { get; set; }
    //    public object U_LGO_OcrCode5 { get; set; }
    //    public object U_LGO_CodCordPed { get; set; }
    //    public object U_LGO_CodAprovad { get; set; }
    //    public object U_LGO_UsuarioWEB { get; set; }
    //    public object U_LGO_UsuarioSAP { get; set; }
    //    public string U_CFS_DEPART { get; set; }
    //    public string U_CFS_OCRCODE1 { get; set; }
    //    public string U_CFS_OCRCODE2 { get; set; }
    //    public object U_CFS_OCRCODE3 { get; set; }
    //    public object U_CFS_CONSOLID { get; set; }
    //    public object U_LGO_DescPIS { get; set; }
    //    public object U_LGO_DescCOFINS { get; set; }
    //    public object U_LGO_DescICMS { get; set; }
    //    public object U_LGO_InfoProprietario { get; set; }
    //    public object U_LGO_IND_NAT_RET { get; set; }
    //    public object U_LGO_IndIntermed { get; set; }
    //    public object U_LGO_CNPJIntermed { get; set; }
    //    public object U_LGO_IdCadIntTran { get; set; }
    //    public object U_LGO_GrpFiscal { get; set; }
    //    public object U_LGO_DataQF { get; set; }
    //    public object U_LGO_QFEntry { get; set; }
    //    public object U_LGO_NFeIsSimplesN { get; set; }
    //    public object U_LGO_NFeDTIsSimplesN { get; set; }
    //    public object U_LGO_NFeDTConsultaPN { get; set; }
    //    public object U_LGO_MDFeTpTransp { get; set; }
    //    public object U_LGO_ReinfIndAqui { get; set; }
    //    public object U_LGO_ReinfIndOpcCP { get; set; }
    //    public string U_CFS_SemNota { get; set; }
    //    public object U_Obs_Cliente { get; set; }
    //    public object U_LGO_TiProdRural { get; set; }
    //    public object U_Cond_Dia_Fixo { get; set; }
    //    public object U_LGO_ValidMatriz { get; set; }
    //    public object U_CFS_Categoria { get; set; }
    //    public object U_SBZ_ModNFe { get; set; }
    //    public object U_SBZ_ModNFSe { get; set; }
    //    public object U_SBZ_ModVencer { get; set; }
    //    public object U_SBZ_ModVencidos { get; set; }
    //    public object U_LGO_DepConsig { get; set; }
    //    public object U_LGO_DataConsultaSintegra { get; set; }
    //    public object U_k33p_SFID { get; set; }
    //    public object U_k33p_SFSend { get; set; }
    //    public object U_k33p_SFSMsg { get; set; }
    //    public List<object> ElectronicProtocols { get; set; }
    //    public List<BPAddressPatch> BPAddresses { get; set; }
    //    public List<ContactEmployeePatch> ContactEmployees { get; set; }
    //    public List<BPAccountReceivablePaybleCollection> BPAccountReceivablePaybleCollection { get; set; }
    //    public List<object> BPPaymentMethods { get; set; }
    //    public List<object> BPWithholdingTaxCollection { get; set; }
    //    public List<object> BPPaymentDates { get; set; }
    //    public List<BPBranchAssignment> BPBranchAssignment { get; set; }
    //    public List<object> BPBankAccounts { get; set; }
    //    public List<BPFiscalTaxIDCollectionPatch> BPFiscalTaxIDCollection { get; set; }
    //    public List<object> DiscountGroups { get; set; }
    //    public BPIntrastatExtension BPIntrastatExtension { get; set; }
    //    public List<object> BPBlockSendingMarketingContents { get; set; }
    //    public List<BPCurrenciesCollection> BPCurrenciesCollection { get; set; }
    //}


}
