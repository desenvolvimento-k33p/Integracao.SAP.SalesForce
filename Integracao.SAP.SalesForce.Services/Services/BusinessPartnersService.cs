using AutoMapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Services.Interfaces;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.Models.ARCH;
using Integracao.SAP.SalesForce.Services.Models.Full;
using Integracao.SAP.SalesForce.Services.Models.SF;
using Integracao.SAP.SalesForce.Services.SQL;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using BPAddress = Integracao.SAP.SalesForce.Services.Models.BPAddress;
using BPFiscalTaxIDCollection = Integracao.SAP.SalesForce.Services.Models.BPFiscalTaxIDCollection;
using BPPaymentMethod = Integracao.SAP.SalesForce.Services.Models.BPPaymentMethod;
using ContactEmployee = Integracao.SAP.SalesForce.Services.Models.ContactEmployee;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class BusinessPartnersService : IBusinessPartnersService
    {
        private readonly ISqlAdapter _sqlAdapter;
        private readonly IHttpAdapter _httpAdapter;
        private readonly ILoggerRepository _logger;
        private readonly IServiceLayerAdapter _serviceLayerAdapter;
        private readonly IMapper _mapper;
        private readonly SalesForceHttp _sfHttp;
        private readonly SalesForceBusiness _sfBusiness;
        private readonly ServiceLayer _serviceLayerHttp;
        private readonly IOptions<Configuration> _configuration;


        public BusinessPartnersService(ISqlAdapter sqlAdapter,
                            IOptions<Configuration> configurations,
                            IHttpAdapter httpAdapter,
                            IServiceLayerAdapter serviceLayerAdapter,
                            IMapper mapper,
                            ILoggerRepository logger,
                            IOptions<Configuration> configuration)
        {
            _sqlAdapter = sqlAdapter;
            _httpAdapter = httpAdapter;
            _logger = logger;
            _mapper = mapper;
            _serviceLayerAdapter = serviceLayerAdapter;
            _configuration = configuration;
            _sfHttp = configurations.Value.SalesForceHttp;
            _sfBusiness = configurations.Value.SalesForceBusiness;
            _serviceLayerHttp = configurations.Value.ServiceLayer;

        }

        public async Task<bool> ProcessAsync()
        {
            try
            {
                List<Record> lista = null;

                var bps = await _getBpsPendent();

                //atualiza data anterior               
                var query = SQLSupport.GetConsultas("SetDataAnteriorPN");
                query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);

                //******************************!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!RETIRAR DEPOIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!***************************************************
                //bps[0].BillingCountry = "Argentina";
                //******************************!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!RETIRAR DEPOIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!***************************************************


                //agrupa por Pais - campo BillingCountry
                var listGrouped = bps.GroupBy(x => x.BillingCountry).Select(grp => grp.ToList()).ToList();
                foreach (var grouped in listGrouped)
                {
                    lista = new List<Record>();
                    lista = grouped;


                    if (lista != null)
                        await _processPartners(lista);//bps

                }

                return true;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método ProcessAsync: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_processPartners",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<Record>> _getBpsPendent()
        {
            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Contas_Contratos;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+Contacts+LIMIT+200)+FROM+Account+WHERE+(Status__c='Integração'+OR+Status__c='Liberado')+AND+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);
            List<Record> allPartners = new List<Record>();
            Token token = new Token();
            string tokenDB = "";
            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;

            try
            {

                //pega filtro data
                var query = SQLSupport.GetConsultas("GetLastModificationDatePN");
                query = string.Format(query, nomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);
                data = String.IsNullOrEmpty(result) ? data : result;

                endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+Contacts+LIMIT+200)+FROM+Account+WHERE+(Status__c='Integração'+OR+Status__c='Liberado')+AND+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);

                //validaToken
                query = SQLSupport.GetConsultas("GetTokenBD");
                query = string.Format(query, nomeBaseBR);
                result = await _sqlAdapter.QueryReaderString(query);

                token.access_token = result;

                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    var response = await _httpAdapter.Call<Contacts_Account>(HttpMethod.Get, endpointPN, null, _sfHttp.Uri, token.access_token);

                    if (response == null)
                    {
                        token = await _httpAdapter.CallLogin<Token>(HttpMethod.Post, endpointToken, null, _sfHttp.Uri);
                        result = "";

                        //insert token in BD
                        query = SQLSupport.GetConsultas("InsertTokenBD");
                        query = string.Format(query, token.access_token, DateTime.Now.ToString(), nomeBaseBR);
                        int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);
                    }
                    else
                    {
                        response.records.ForEach(e => allPartners.Add(e));
                        token.access_token = "OK";
                    }
                }

                return allPartners;

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _getBpsPendent: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_getBpsPendent",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processPartners(List<Record> bps)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var bp in bps)
                {
                    if (bp.BillingCountry.ToString().ToUpper().Equals("BRASIL") || (!bp.BillingCountry.Equals("Argentina") && !bp.BillingCountry.Equals("Chile")))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    else if (bp.BillingCountry.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    else if (bp.BillingCountry.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;
                    else
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;

                    cont++;
                    string query = "";
                    List<string> result = null;

                    if (bp.BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                    {
                        query = SQLSupport.GetConsultas("GetBpartnersById_CNPJ");
                        query = string.Format(query, bp.CNPJ__c, basePais);
                        result = await _sqlAdapter.QueryListString(query);
                    }
                    else if (!bp.BillingCountry.ToString().ToUpper().Equals("BRASIL") && (bp.BillingCountry.ToString().Equals("Argentina") || bp.BillingCountry.ToString().Equals("Chile")))
                    {
                        query = SQLSupport.GetConsultas("GetBpartnersById_FederalTaxIdARCH");
                        query = string.Format(query, bp.RUT__c, basePais);
                        result = await _sqlAdapter.QueryListString(query);
                    }
                    else
                    {
                        query = SQLSupport.GetConsultas("GetBpartnersById_FederalTaxId");
                        query = string.Format(query, bp.RUT__c, basePais);
                        result = await _sqlAdapter.QueryListString(query);
                    }

                    query = SQLSupport.GetConsultas("GetBpartnersBySFID");
                    query = string.Format(query, bp.Id, basePais);
                    var result2 = await _sqlAdapter.QueryReaderString(query);

                    //if (bp.Id == "8345431201")
                    //   continue;

                    if (bp.RUT__c == null && !bp.BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                    {
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Campo RUT pro PN Estrangeiro esta vazio ou nulo PN: {bp.Id}",
                            Owner = "BusinessPartnersService",
                            Method = "_processPartners",
                            Key = bp.Id,
                            Key2 = bp.Id,
                            Company = basePais
                        });
                        continue;
                    }

                    //if (result.Count > 0)
                    //    if (result[0] != result2 && bp.BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                    //    {
                    //        await _logger.Logger(new LogIntegration
                    //        {
                    //            LogTypeCode = 1,
                    //            Message = $"Divergência de informações de ID e CNPJ. PN: {bp.AccountNumber}",
                    //            Owner = "BusinessPartnersService",
                    //            Method = "_processPartners",
                    //            Key = bp.Id,
                    //            Key2 = bp.Id,
                    //            Company = basePais
                    //        });
                    //        continue;
                    //    }

                    if (String.IsNullOrEmpty(result2))
                        if (bp.BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                        {
                            query = SQLSupport.GetConsultas("GetCNPJByIdSF");
                            query = string.Format(query, basePais, bp.Id);
                            string cnpj = await _sqlAdapter.QueryReaderString(query);

                            if (bp.CNPJ__c != cnpj && !String.IsNullOrEmpty(cnpj))
                            {

                                await _logger.Logger(new LogIntegration
                                {
                                    LogTypeCode = 1,
                                    Message = $"Divergência de informações de ID e CNPJ. PN: {bp.AccountNumber}",
                                    Owner = "BusinessPartnersService",
                                    Method = "_processPartners",
                                    Key = bp.Id,
                                    Key2 = bp.Id,
                                    Company = basePais
                                });
                                continue;
                            }
                        }

                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Processando PN: {bp.Id}",
                        Owner = "BusinessPartnersService",
                        Method = "_processPartners",
                        Key = bp.Id,
                        Key2 = bp.Id,
                        Company = basePais
                    });


                    if (result.Count == 0)
                        await _InsereUnitBP(bp, basePais);
                    else
                        await _AtualizaUnitBP(bp, basePais, result[0]);

                }

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _processPartners: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_processPartners",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                //throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<dynamic> _InsereUnitBP(Record bp, string basePais)
        {
            try
            {
                List<ReturnAccountPatch> ret = null;
                var response = await _createBP(bp, basePais);

                if (response == null)
                    return null;

                //envia pro SF (CardCode e Status : "Integrado")
                if (!String.IsNullOrEmpty(response.CardCode))
                    ret = await _EnviaPNSF(response, "Add", basePais, bp.BillingCountry);

                string msg = null;

                if (ret != null)
                {
                    msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id;
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Envio de retorno pro SalesForce: {response.CardCode} - {msg}",
                        Owner = "BusinessPartnersService",
                        Method = "_InsereUnitBP",
                        Key = response.CardCode,
                        Key2 = bp.Id,
                        Company = basePais,
                        RequestObject = JsonSerializer.Serialize<List<ReturnAccountPatch>>(ret)
                    });
                }

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Inserção de PN: {response.CardCode} ",
                    Owner = "BusinessPartnersService",
                    Method = "_InsereUnitBP",
                    Key = response.CardCode,
                    Key2 = bp.Id,
                    Company = basePais,
                    RequestObject = JsonSerializer.Serialize<Record>(bp),
                    ResponseObject = JsonSerializer.Serialize<BusinessPartnersDTORetorno>(response)
                });

                if (response.CardCode != null)
                {

                    //atualiza flag data
                    string data = DateTime.Now.AddHours(3).AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    var query = SQLSupport.GetConsultas("SetLastModificationDatePN");
                    query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, response.CardCode, ret[0].id);
                    var result = await _sqlAdapter.QueryReaderString(query);
                }

                return null;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _InsereUnitBP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_InsereUnitBP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<dynamic> _AtualizaUnitBP(Record bp, string basePais, string cardCode)
        {
            try
            {
                List<ReturnAccountPatch> ret = null;
                var response = await _PatchBP(bp, basePais, cardCode);


                //envia pro SF(Status: "Liberado")--passa SF ID CardCode
                if (response != null)
                    ret = await _EnviaPNSFPatch(bp, "Atu", basePais, cardCode, bp.BillingCountry);

                string msg = "";
                if (ret != null)
                {
                    msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id + " PN:" + cardCode;
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Envio de retorno pro SalesForce- {msg}",
                        Owner = "BusinessPartnersService",
                        Method = "_AtualizaUnitBP",
                        Key = cardCode,
                        Key2 = bp.Id,
                        Company = basePais,
                        RequestObject = JsonSerializer.Serialize<List<ReturnAccountPatch>>(ret)
                    });
                }
                //else
                //{
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Atualização de PN: {cardCode}",
                    Owner = "BusinessPartnersService",
                    Method = "_AtualizaUnitBP",
                    Key = cardCode,
                    Key2 = bp.Id,
                    Company = basePais,
                    RequestObject = JsonSerializer.Serialize<FullBusinessPartnersSAP>(response),
                    ResponseObject = JsonSerializer.Serialize<FullBusinessPartnersSAP>(response)
                });
                //}

                //atualiza flag data
                string data = DateTime.Now.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ssZ");
                var query = SQLSupport.GetConsultas("SetLastModificationDatePN");
                query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, cardCode, ret == null ? "" : ret[0].id);
                var result = await _sqlAdapter.QueryReaderString(query);

                return null;
            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _AtualizaUnitBP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_AtualizaUnitBP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _EnviaPNSF(BusinessPartnersDTORetorno obj, string tipo, string basePais, string pais)
        {
            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;
            string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Contas_Contratos;
            var endpoint = String.Format("/services/data/{0}/composite/sobjects/Account/Id", apiVersion);
            var endpointToken = $"/services/oauth2/token";

            Token token = new Token();
            Account_Patch objPatchPN = new Account_Patch();
            List<RecordPatch> pnPatch = new List<RecordPatch>();
            RecordPatch pn = new RecordPatch();
            AttributesPatch atrib = new AttributesPatch();
            List<ReturnAccountPatch> response = null;

            try
            {
                atrib.type = "Account";

                pn.attributes = atrib;
                pn.Id = obj.U_k33p_SFID;
                pn.AccountNumber = obj.CardCode;
                pn.CNPJ__c = pais.ToUpper() == "BRASIL" ? obj.BPFiscalTaxIDCollection[0].TaxId0.ToString() : ((pais == "Argentina" || pais == "Chile") ? obj.FederalTaxID : obj.U_Rut);// ? obj.BPFiscalTaxIDCollection[0].TaxId0.ToString() : obj.U_Rut;
                pn.Status__c = "Liberado";

                pnPatch.Add(pn);

                objPatchPN.records = pnPatch;

                //validaToken
                var query = SQLSupport.GetConsultas("GetTokenBD");
                query = string.Format(query, _configuration.Value.ServiceLayer.CompanyDB_BR);
                var result = await _sqlAdapter.QueryReaderString(query);

                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    response = await _httpAdapter.Call<List<ReturnAccountPatch>>(HttpMethod.Patch, endpoint, objPatchPN, _sfHttp.Uri, token.access_token);

                    if (response == null)
                    {
                        token = await _httpAdapter.CallLogin<Token>(HttpMethod.Post, endpointToken, null, _sfHttp.Uri);
                        result = "";
                        //insert token in BD
                        query = SQLSupport.GetConsultas("InsertTokenBD");
                        query = string.Format(query, token.access_token, DateTime.Now.ToString(), _configuration.Value.ServiceLayer.CompanyDB_BR);
                        int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);
                    }
                    else
                        token.access_token = "OK";

                }

                return response;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _EnviaPNSF: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_EnviaPNSF",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _EnviaPNSFPatch(Record obj, string tipo, string basePais, string cardCode, string pais)
        {
            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;
            string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Contas_Contratos;
            var endpoint = String.Format("/services/data/{0}/composite/sobjects/Account/Id", apiVersion);
            var endpointToken = $"/services/oauth2/token";

            Token token = new Token();
            Account_Patch objPatchPN = new Account_Patch();
            List<RecordPatch> pnPatch = new List<RecordPatch>();
            RecordPatch pn = new RecordPatch();
            AttributesPatch atrib = new AttributesPatch();
            List<ReturnAccountPatch> response = null;

            try
            {
                atrib.type = "Account";

                pn.attributes = atrib;
                pn.Id = obj.Id;
                pn.AccountNumber = cardCode;//obj.AccountNumber;
                pn.CNPJ__c = pais.ToUpper() == "BRASIL" ? obj.CNPJ__c : obj.RUT__c;//  ? obj.CNPJ__c : obj.RUT__c;
                pn.Status__c = "Liberado";

                pnPatch.Add(pn);

                objPatchPN.records = pnPatch;

                //validaToken
                var query = SQLSupport.GetConsultas("GetTokenBD");
                query = string.Format(query, _configuration.Value.ServiceLayer.CompanyDB_BR);
                var result = await _sqlAdapter.QueryReaderString(query);

                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    response = await _httpAdapter.Call<List<ReturnAccountPatch>>(HttpMethod.Patch, endpoint, objPatchPN, _sfHttp.Uri, token.access_token);

                    if (response == null)
                    {
                        token = await _httpAdapter.CallLogin<Token>(HttpMethod.Post, endpointToken, null, _sfHttp.Uri);
                        result = "";

                        //insert token in BD
                        query = SQLSupport.GetConsultas("InsertTokenBD");
                        query = string.Format(query, token.access_token, DateTime.Now.ToString(), _configuration.Value.ServiceLayer.CompanyDB_BR);
                        int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);
                    }
                    else
                        token.access_token = "OK";

                }

                return response;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _EnviaPNSF: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_EnviaPNSF",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<BusinessPartnersDTORetorno> _createBP(Record bpSf, string basePais)
        {

            BusinessPartnersDTOARCH businessPartner2 = null;
            BusinessPartnersDTO businessPartner = null;

            if (basePais == "SBOPRODAR" || basePais == "SBOPRODCH")
            {
                businessPartner2 = await _populateBusinessPartnerSAPARCH(bpSf, basePais);
            }
            else
            {
                businessPartner = await _populateBusinessPartnerSAP(bpSf, basePais);
            }

            try
            {


                if (businessPartner == null && businessPartner2 == null)
                    return null;


                var responseBp = await _serviceLayerAdapter.Call<BusinessPartnersDTORetorno>(
                      "BusinessPartners", HttpMethod.Post, businessPartner == null ? businessPartner2 : businessPartner, _serviceLayerHttp.Uri, null, bpSf.BillingCountry);


                if (!String.IsNullOrEmpty(bpSf.Inicio_do_Regime__c) && !String.IsNullOrEmpty(bpSf.Fim_do_Regime__c))
                    if (bpSf.BillingCountry.ToUpper() == "BRASIL")
                    {
                        //Informções tribitarias
                        var query = SQLSupport.GetConsultas("GetOBNI");
                        query = string.Format(query, basePais, bpSf.Tipo_Tributario__c);
                        string tipo = await _sqlAdapter.QueryReaderString(query);


                        query = SQLSupport.GetConsultas("GetOBNI2");
                        query = string.Format(query, basePais, bpSf.Regime_Tributario__c);
                        string regime = await _sqlAdapter.QueryReaderString(query);

                        if (bpSf.Tipo_Tributario__c == "181")//Simples
                        {
                            query = SQLSupport.GetConsultas("INSERTCRD11_1");
                            query = string.Format(query, basePais, responseBp.CardCode, tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c, regime, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c);
                        }
                        else
                        {
                            query = SQLSupport.GetConsultas("INSERTCRD11_2");
                            query = string.Format(query, basePais, responseBp.CardCode, tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c, regime, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c);
                        }


                        int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);
                    }

                return responseBp;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _createBP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_createBP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }


        private async Task<FullBusinessPartnersSAP> _PatchBP(Record bpSf, string basePais, string cardCode)
        {
            BPFiscalTaxSAP bpPatch = new BPFiscalTaxSAP();
            List<BPFiscalTaxIDCollection2> lstPatch = new List<BPFiscalTaxIDCollection2>();

            try
            {

                var businessPartner = await _populateBusinessPartnerSAPPatch(bpSf, basePais, cardCode);

                var responseBp = await _serviceLayerAdapter.Call<FullBusinessPartnersSAP>(
                      $"BusinessPartners('{cardCode}')", HttpMethod.Put, businessPartner, _serviceLayerHttp.Uri, null, bpSf.BillingCountry);

                //**************************preencher objeto cria metodo*************************************************************            
                foreach (var linhas in businessPartner.BPFiscalTaxIDCollection)
                {
                    BPFiscalTaxIDCollection2 fiscalPatch = new BPFiscalTaxIDCollection2();
                    fiscalPatch.Address = linhas.Address;
                    fiscalPatch.AToRetrNFe = linhas.AToRetrNFe;
                    fiscalPatch.CNAECode = linhas.CNAECode;
                    fiscalPatch.AddrType = linhas.AddrType;
                    fiscalPatch.BPCode = linhas.BPCode;
                    fiscalPatch.TaxId0 = linhas.TaxId0;
                    fiscalPatch.TaxId1 = linhas.TaxId1;
                    fiscalPatch.TaxId2 = linhas.TaxId2;
                    fiscalPatch.TaxId3 = linhas.TaxId3;
                    fiscalPatch.TaxId4 = linhas.TaxId4;
                    fiscalPatch.TaxId5 = linhas.TaxId5;
                    fiscalPatch.TaxId6 = linhas.TaxId6;
                    fiscalPatch.TaxId7 = linhas.TaxId7;
                    fiscalPatch.TaxId8 = linhas.TaxId8;
                    fiscalPatch.TaxId9 = linhas.TaxId9;
                    fiscalPatch.TaxId10 = linhas.TaxId10;
                    fiscalPatch.TaxId11 = linhas.TaxId11;
                    fiscalPatch.TaxId12 = linhas.TaxId12;
                    fiscalPatch.TaxId13 = linhas.TaxId13;

                    lstPatch.Add(fiscalPatch);


                }

                bpPatch.BPFiscalTaxIDCollection = lstPatch;

                //**************************preencher objeto cria metodo*************************************************************

                var resp = await _serviceLayerAdapter.Call<BPFiscalTaxSAP>(
                      $"BusinessPartners('{cardCode}')", HttpMethod.Patch, bpPatch, _serviceLayerHttp.Uri, null, bpSf.BillingCountry);


                #region old UPDATE
                ////Informções tribitarias
                //var query = SQLSupport.GetConsultas("GetOBNI");
                //query = string.Format(query, basePais, bpSf.Tipo_Tributario__c);
                //string tipo = await _sqlAdapter.QueryReaderString(query);


                //query = SQLSupport.GetConsultas("GetOBNI2");
                //query = string.Format(query, basePais, bpSf.Regime_Tributario__c);
                //string regime = await _sqlAdapter.QueryReaderString(query);

                //if (bpSf.Tipo_Tributario__c == "181")//Simples
                //{
                //    query = SQLSupport.GetConsultas("updateCRD11_1");
                //    query = string.Format(query, basePais,tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c,regime, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c,cardCode);
                //}
                //else
                //{
                //    query = SQLSupport.GetConsultas("updateCRD11_2");
                //    query = string.Format(query, basePais, tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c, cardCode);
                //}

                //int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);
                #endregion


                if (!String.IsNullOrEmpty(bpSf.Inicio_do_Regime__c) && !String.IsNullOrEmpty(bpSf.Fim_do_Regime__c) && !String.IsNullOrEmpty(bpSf.Tipo_Tributario__c))
                    if (bpSf.BillingCountry.ToUpper() == "BRASIL")
                    {

                        //Informções tribitarias
                        var query = SQLSupport.GetConsultas("GetOBNI");
                        query = string.Format(query, basePais, bpSf.Tipo_Tributario__c);
                        string tipo = await _sqlAdapter.QueryReaderString(query);


                        query = SQLSupport.GetConsultas("GetOBNI2");
                        query = string.Format(query, basePais, bpSf.Regime_Tributario__c);
                        string regime = await _sqlAdapter.QueryReaderString(query);

                        if (bpSf.Tipo_Tributario__c == "181")//Simples
                        {
                            query = SQLSupport.GetConsultas("INSERTCRD11_1");
                            query = string.Format(query, basePais, cardCode, tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c, regime, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c);
                        }
                        else
                        {
                            query = SQLSupport.GetConsultas("INSERTCRD11_2");
                            query = string.Format(query, basePais, cardCode, tipo, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c, regime, bpSf.Inicio_do_Regime__c, bpSf.Fim_do_Regime__c);
                        }


                        int ret = await _sqlAdapter.QueryInsertUpdate<int>(query);

                    }

                return businessPartner;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _PatchBP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_PatchBP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<BusinessPartnersDTO> _populateBusinessPartnerSAP(Record partner, string basePais)
        {
            var bp = new BusinessPartnersDTO();// _mapper.Map<Record, BusinessPartnersDTO>(partner);

            try
            {
                if (basePais != _configuration.Value.SalesForceBusiness.NomeBaseBR)
                {
                    bp.CardCode = await BuscaUltimoCardCode(basePais);//sequencia manual devido a serie da base estrang
                    bp.FederalTaxID = partner.RUT__c;
                }

                bp.CardName = partner.Name;
                bp.CardForeignName = partner.NomeFantasia__c;

                if (bp.CardName.Length > 99)
                {
                    bp.CardName = bp.CardName.Substring(0, 99);
                }

                bp.SalesPersonCode = await GetVendedorByIDSF(basePais, partner.OwnerId);

                //**************cabeçaho
                bp.U_SBZ_GerBol = partner.Forma_de_Pagamento__c == "Boleto" ? "Y" : "N";
                bp.U_SBZ_CodCart = partner.Forma_de_Pagamento__c == "Boleto" ? "ITAU-FL02" : "";

                bp.PeymentMethodCode = partner.Forma_de_Pagamento__c == "Boleto" ? await BuscaBoletoAtivoSAP(basePais) : await BuscaDepositoAtivoSAP(basePais);
                bp.PayTermsGrpCode = Convert.ToInt32(partner.CondicaoPagamento__c);
                bp.Series = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.SeriesCardCodeNumerationBR : ((partner.BillingCountry == "Argentina" || partner.BillingCountry == "Chile") ? 1 : _sfBusiness.SeriesCardCodeNumerationEST);// _sfBusiness.SeriesCardCodeNumerationEST;// (partner.BillingCountry != "Argentina" && partner.BillingCountry != "Chile") ? _sfBusiness.SeriesCardCodeNumerationBR : _sfBusiness.SeriesCardCodeNumerationEST;
                bp.CardType = "cCustomer";
                bp.GroupCode = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.GroupCodeBR_BaseBR : ((partner.BillingCountry == "Argentina" || partner.BillingCountry == "Chile") ? 100 : _sfBusiness.SeriesCardCodeNumerationEST); //_sfBusiness.GroupCode_BaseEST;
                bp.EmailAddress = partner.Contacts.records[0].Email;
                bp.Phone1 = partner.Phone;
                //bp.Phone2 = partner.Phone;
                bp.ContactPerson = partner.Name + " " + partner.LastName;
                bp.U_Setor = partner.SetorDeAplicacao__c;
                bp.Industry = Convert.ToInt32(partner.UnidadeDeNegocios__c);
                bp.U_k33p_SFID = partner.Id;
                bp.U_k33p_SFSend = "S";
                bp.U_Rut = partner.RUT__c;
                bp.U_CFS_OCRCODE1 = "2001";
                bp.U_CFS_OCRCODE2 = await BuscaCFS2(basePais, partner.UnidadeDeNegocios__c);

                string pais = await BuscaPais(basePais, partner.BillingCountry);

                if (String.IsNullOrEmpty(pais))
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"inserção de PN: {partner.Id}",
                        Owner = "BusinessPartnersService",
                        Method = "_populqteBusinessPartnerSAP",
                        Key = "",
                        Key2 = partner.Id,
                        Company = basePais,
                        RequestObject = null,
                        ResponseObject = $"Pais nao encontrado: {partner.BillingCountry}"
                    });
                    return null;
                }

                var municipioCob = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.BillingAddress.city, basePais) : await BuscaMunicipioEst(partner.BillingAddress.city, basePais, pais);
                municipioCob = String.IsNullOrEmpty(municipioCob) ? "1" : municipioCob;
                var municipioEnt = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.ShippingAddress.city, basePais) : await BuscaMunicipioEst(partner.ShippingAddress.city, basePais, pais);
                municipioEnt = String.IsNullOrEmpty(municipioEnt) ? "1" : municipioEnt;
                //***************endereços
                bp.BPAddresses = new List<BPAddress>();

                bp.BPAddresses.Add(new BPAddress
                {
                    AddressName = "COBRANÇA",
                    AddressType = "bo_BillTo",
                    Street = partner.BillingAddress.street,
                    StreetNo = partner.NumeroCobranca__c.ToString(),
                    City = partner.BillingAddress.city,
                    Block = partner.BairroCobranca__c,
                    Country = partner.BillingCountry.ToUpper() == "BRASIL" ? "BR" : pais,// (partner.BillingCountry == "Argentina" ? "AR" : "CH"),
                    State = partner.BillingCountry.ToUpper() == "BRASIL" ? partner.BillingAddress.state : "EX",//: await BuscaEstadoAR(basePais, partner.BillingCity),
                    ZipCode = partner.BillingPostalCode.Replace("-", ""),
                    TypeOfAddress = "Rua",
                    County = municipioCob.ToString(),
                    //BuildingFloorRoom = "",

                });

                bp.BPAddresses.Add(new BPAddress
                {
                    AddressName = "ENTREGA",
                    AddressType = "bo_ShipTo",
                    Street = partner.ShippingAddress.street,
                    StreetNo = partner.NumeroEntrega__c.ToString(),
                    City = partner.ShippingAddress.city,
                    Block = partner.BairroEntrega__c,
                    Country = partner.BillingCountry.ToUpper() == "BRASIL" ? "BR" : pais,//await BuscaPais(basePais, partner.BillingCountry),
                    State = partner.BillingCountry.ToUpper() == "BRASIL" ? partner.ShippingAddress.state : "EX",//await BuscaEstadoAR(basePais, partner.ShippingCity),
                    ZipCode = partner.ShippingPostalCode.Replace("-", ""),
                    County = municipioEnt.ToString(),
                    //BuildingFloorRoom = "",
                });




                //******************fiscais
                if (partner.BillingCountry.ToUpper() == "BRASIL" || (partner.BillingCountry != "Argentina" && partner.BillingCountry != "Chile"))
                {
                    bp.BPFiscalTaxIDCollection =
                        new List<BPFiscalTaxIDCollection>();

                    bp.BPFiscalTaxIDCollection.Add(
                            new BPFiscalTaxIDCollection
                            {
                                //Address = "ENTREGA",
                                TaxId0 = partner.CNPJ__c,
                                //TaxId4 = "",
                                TaxId1 = partner.InscricaoEstadual__c
                            });
                }

                //**contatos
                bp.ContactEmployees = new List<ContactEmployee>();

                foreach (var contato in partner.Contacts.records)
                {

                    bp.ContactEmployees.Add(new ContactEmployee
                    {
                        Name = contato.Name,
                        Phone1 = contato.Phone,
                        //Phone2 = "",
                        MobilePhone = contato.MobilePhone,
                        E_Mail = contato.Email,
                        FirstName = contato.FirstName,
                        //MiddleName = "",
                        LastName = contato.LastName,
                        U_k33p_SFID = contato.Id,
                        U_SBZ_EnvEml = contato.RecebeBoleto__c == "Sim" ? "Y" : "N",
                        U_LG_RecEmailXML = contato.RecebeNFe__c == "Sim" ? "S" : "N",


                    });
                }

                List<BPPaymentMethod> listaPaym = new List<BPPaymentMethod>();
                BPPaymentMethod bp2 = new BPPaymentMethod();
                bp2.PaymentMethodCode = bp.PeymentMethodCode;


                bp.BPPaymentMethods = new List<BPPaymentMethod>();
                bp.BPPaymentMethods.Add(bp2);






                return bp;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateBusinessPartnerSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_populateBusinessPartnerSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<BusinessPartnersDTOARCH> _populateBusinessPartnerSAPARCH(Record partner, string basePais)
        {
            var bp = new BusinessPartnersDTOARCH();// _mapper.Map<Record, BusinessPartnersDTO>(partner);

            try
            {
                if (basePais != _configuration.Value.SalesForceBusiness.NomeBaseBR)
                {
                    bp.CardCode = await BuscaUltimoCardCode(basePais);//sequencia manual devido a serie da base estrang
                    bp.FederalTaxID = partner.RUT__c;
                }

                bp.CardName = partner.Name;
                bp.CardForeignName = partner.NomeFantasia__c;

                if (bp.CardName.Length > 99)
                {
                    bp.CardName = bp.CardName.Substring(0, 99);
                }

                bp.SalesPersonCode = await GetVendedorByIDSF(basePais, partner.OwnerId);

                //**************cabeçaho
                // bp.U_SBZ_GerBol = partner.Forma_de_Pagamento__c == "Boleto" ? "Y" : "N";
                // bp.U_SBZ_CodCart = partner.Forma_de_Pagamento__c == "Boleto" ? "ITAU-FL02" : "";

                bp.PeymentMethodCode = partner.Forma_de_Pagamento__c == "Boleto" ? await BuscaBoletoAtivoSAP(basePais) : await BuscaDepositoAtivoSAP(basePais);
                bp.PayTermsGrpCode = await BuscaCondPgto(basePais, partner.CondicaoPagamento__c);
                bp.Series = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.SeriesCardCodeNumerationBR : ((partner.BillingCountry == "Argentina" || partner.BillingCountry == "Chile") ? 1 : _sfBusiness.SeriesCardCodeNumerationEST);// _sfBusiness.SeriesCardCodeNumerationEST;// (partner.BillingCountry != "Argentina" && partner.BillingCountry != "Chile") ? _sfBusiness.SeriesCardCodeNumerationBR : _sfBusiness.SeriesCardCodeNumerationEST;
                bp.CardType = "cCustomer";
                bp.GroupCode = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.GroupCodeBR_BaseBR : ((partner.BillingCountry == "Argentina" || partner.BillingCountry == "Chile") ? 100 : _sfBusiness.SeriesCardCodeNumerationEST); //_sfBusiness.GroupCode_BaseEST;
                bp.EmailAddress = partner.Contacts.records[0].Email;
                bp.Phone1 = partner.Phone;
                //bp.Phone2 = partner.Phone;
                bp.ContactPerson = partner.Name + " " + partner.LastName;
                bp.U_Setor = partner.SetorDeAplicacao__c;
                bp.Industry = await BuscaSetor(basePais, partner.UnidadeDeNegocios__c);
                bp.U_k33p_SFID = partner.Id;
                bp.U_k33p_SFSend = "S";
                bp.FederalTaxID = partner.RUT__c;
                bp.U_CFS_OCRCODE1 = "2001";
                bp.U_CFS_OCRCODE2 = await BuscaCFS2(basePais, partner.UnidadeDeNegocios__c);

                string pais = await BuscaPais(basePais, partner.BillingCountry);

                if (String.IsNullOrEmpty(pais))
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"inserção de PN: {partner.Id}",
                        Owner = "BusinessPartnersService",
                        Method = "_populqteBusinessPartnerSAP",
                        Key = "",
                        Key2 = partner.Id,
                        Company = basePais,
                        RequestObject = null,
                        ResponseObject = $"Pais nao encontrado: {partner.BillingCountry}"
                    });
                    return null;
                }

                var municipioCob = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.BillingAddress.city, basePais) : await BuscaMunicipioEst(partner.BillingAddress.city, basePais, pais);
                municipioCob = String.IsNullOrEmpty(municipioCob) ? "1" : municipioCob;
                var municipioEnt = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.ShippingAddress.city, basePais) : await BuscaMunicipioEst(partner.ShippingAddress.city, basePais, pais);
                municipioEnt = String.IsNullOrEmpty(municipioEnt) ? "1" : municipioEnt;
                //***************endereços
                bp.BPAddresses = new List<Integracao.SAP.SalesForce.Services.Models.ARCH.BPAddress>();

                bp.BPAddresses.Add(new Integracao.SAP.SalesForce.Services.Models.ARCH.BPAddress
                {
                    AddressName = "COBRANÇA",
                    AddressType = "bo_BillTo",
                    Street = partner.BillingAddress.street,
                    StreetNo = partner.NumeroCobranca__c.ToString(),
                    City = partner.BillingAddress.city,
                    Block = partner.BairroCobranca__c,
                    Country = partner.BillingCountry.ToUpper() == "BRASIL" ? "BR" : pais,// (partner.BillingCountry == "Argentina" ? "AR" : "CH"),
                    State = await BuscaEstadoAR(basePais, partner.BillingCity),
                    ZipCode = partner.BillingPostalCode.Replace("-", ""),
                    TypeOfAddress = "Rua",
                    County = municipioCob.ToString(),
                    //BuildingFloorRoom = "",

                });

                bp.BPAddresses.Add(new Integracao.SAP.SalesForce.Services.Models.ARCH.BPAddress
                {
                    AddressName = "ENTREGA",
                    AddressType = "bo_ShipTo",
                    Street = partner.ShippingAddress.street,
                    StreetNo = partner.NumeroEntrega__c.ToString(),
                    City = partner.ShippingAddress.city,
                    Block = partner.BairroEntrega__c,
                    Country = partner.BillingCountry.ToUpper() == "BRASIL" ? "BR" : pais,//await BuscaPais(basePais, partner.BillingCountry),
                    State = await BuscaEstadoAR(basePais, partner.BillingCity),//partner.BillingCountry.ToUpper() == "BRASIL" ? partner.ShippingAddress.state : "EX",//await BuscaEstadoAR(basePais, partner.ShippingCity),
                    ZipCode = partner.ShippingPostalCode.Replace("-", ""),
                    County = municipioEnt.ToString(),
                    //BuildingFloorRoom = "",
                });




                //******************fiscais
                if (partner.BillingCountry.ToUpper() == "BRASIL" || (partner.BillingCountry != "Argentina" && partner.BillingCountry != "Chile"))
                {
                    bp.BPFiscalTaxIDCollection =
                        new List<Integracao.SAP.SalesForce.Services.Models.ARCH.BPFiscalTaxIDCollection>();

                    bp.BPFiscalTaxIDCollection.Add(
                            new Integracao.SAP.SalesForce.Services.Models.ARCH.BPFiscalTaxIDCollection
                            {
                                //Address = "ENTREGA",
                                TaxId0 = partner.CNPJ__c,
                                //TaxId4 = "",
                                TaxId1 = partner.InscricaoEstadual__c
                            });
                }

                //**contatos
                bp.ContactEmployees = new List<Integracao.SAP.SalesForce.Services.Models.ARCH.ContactEmployee>();

                foreach (var contato in partner.Contacts.records)
                {

                    bp.ContactEmployees.Add(new Integracao.SAP.SalesForce.Services.Models.ARCH.ContactEmployee
                    {
                        Name = contato.Name,
                        Phone1 = contato.Phone,
                        //Phone2 = "",
                        MobilePhone = contato.MobilePhone,
                        E_Mail = contato.Email,
                        FirstName = contato.FirstName,
                        //MiddleName = "",
                        LastName = contato.LastName,
                        U_k33p_SFID = contato.Id,
                        //U_SBZ_EnvEml = contato.RecebeBoleto__c == "Sim" ? "Y" : "N",
                        //U_LG_RecEmailXML = contato.RecebeNFe__c == "Sim" ? "S" : "N",


                    });
                }

                List<Integracao.SAP.SalesForce.Services.Models.ARCH.BPPaymentMethod> listaPaym = new List<Integracao.SAP.SalesForce.Services.Models.ARCH.BPPaymentMethod>();
                Integracao.SAP.SalesForce.Services.Models.ARCH.BPPaymentMethod bp2 = new Integracao.SAP.SalesForce.Services.Models.ARCH.BPPaymentMethod();
                bp2.PaymentMethodCode = bp.PeymentMethodCode;


                bp.BPPaymentMethods = new List<Integracao.SAP.SalesForce.Services.Models.ARCH.BPPaymentMethod>();
                bp.BPPaymentMethods.Add(bp2);






                return bp;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateBusinessPartnerSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_populateBusinessPartnerSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<FullBusinessPartnersSAP> _populateBusinessPartnerSAPPatch(Record partner, string basePais, string cardCode)
        {
            try
            {

                var responseBP = await _serviceLayerAdapter.Call<FullBusinessPartnersSAP>(
                     $"BusinessPartners('{cardCode}')", HttpMethod.Get, null, _serviceLayerHttp.Uri, null, partner.BillingCountry);

                FullBusinessPartnersSAP fullBP = responseBP;
                BPAddressPatch bpAdress = null;
                //var municipioCob = await BuscaMunicipio(partner.BillingAddress.city, basePais);
                //municipioCob = String.IsNullOrEmpty(municipioCob) ? "1" : municipioCob;
                //var municipioEnt = await BuscaMunicipio(partner.ShippingAddress.city, basePais);
                //municipioEnt = String.IsNullOrEmpty(municipioEnt) ? "1" : municipioEnt;
                string pais = await BuscaPais(basePais, partner.BillingCountry);

                var municipioCob = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.BillingAddress.city, basePais) : await BuscaMunicipioEst(partner.BillingAddress.city, basePais, pais);
                municipioCob = String.IsNullOrEmpty(municipioCob) ? "1" : municipioCob;
                var municipioEnt = partner.BillingCountry.ToUpper() == "BRASIL" ? await BuscaMunicipio(partner.ShippingAddress.city, basePais) : await BuscaMunicipioEst(partner.ShippingAddress.city, basePais, pais);
                municipioEnt = String.IsNullOrEmpty(municipioEnt) ? "1" : municipioEnt;

                fullBP.U_Rut = partner.RUT__c;

                for (int i = 0; i < fullBP.BPAddresses.Count(); i++)
                {


                    bpAdress = new BPAddressPatch();
                    bpAdress.AddressType = fullBP.BPAddresses[i].AddressType;
                    bpAdress.AddressName = fullBP.BPAddresses[i].AddressName;
                    bpAdress.Street = partner.BillingAddress.street;
                    bpAdress.StreetNo = partner.NumeroCobranca__c.ToString();
                    bpAdress.City = partner.BillingAddress.city;
                    bpAdress.Block = partner.BairroCobranca__c;
                    bpAdress.Country = partner.BillingCountry.ToUpper() == "BRASIL" ? "BR" : pais;//(partner.BillingCountry == "Argentina" ? "AR" : "CH");
                    bpAdress.State = partner.BillingCountry.ToUpper() == "BRASIL" ? partner.BillingAddress.state : "EX";
                    bpAdress.ZipCode = partner.BillingPostalCode.Replace("-", "");
                    bpAdress.TypeOfAddress = "Rua";
                    bpAdress.County = fullBP.BPAddresses[i].AddressType == "bo_ShipTo" ? municipioEnt.ToString() : municipioCob.ToString();


                    fullBP.BPAddresses[i] = bpAdress;



                }

                fullBP.CardName = partner.Name;
                fullBP.CardForeignName = partner.NomeFantasia__c;

                if (fullBP.CardName.Length > 99)
                {
                    fullBP.CardName = fullBP.CardName.Substring(0, 99);
                }

                fullBP.SalesPersonCode = await GetVendedorByIDSF(basePais, partner.OwnerId);

                //**************cabeçaho
                fullBP.PeymentMethodCode = partner.Forma_de_Pagamento__c == "Boleto" ? await BuscaBoletoAtivoSAP(basePais) : await BuscaDepositoAtivoSAP(basePais);
                fullBP.PayTermsGrpCode = Convert.ToInt32(partner.CondicaoPagamento__c);
                fullBP.Series = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.SeriesCardCodeNumerationBR : _sfBusiness.SeriesCardCodeNumerationEST;//fullBP.Series = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.SeriesCardCodeNumerationBR : (partner.BillingCountry != "Argentina" && partner.BillingCountry != "Chile") ? _sfBusiness.SeriesCardCodeNumerationBR : _sfBusiness.SeriesCardCodeNumerationEST;

                fullBP.CardType = "cCustomer";
                fullBP.GroupCode = partner.BillingCountry.ToUpper() == "BRASIL" ? _sfBusiness.GroupCodeBR_BaseBR : _sfBusiness.GroupCode_BaseEST;
                fullBP.EmailAddress = partner.Contacts.records[0].Email;
                fullBP.Phone1 = partner.Phone;
                //fullBP.Phone2 = partner.Phone;
                fullBP.ContactPerson = partner.Name + " " + partner.LastName;
                fullBP.U_Setor = partner.SetorDeAplicacao__c;
                fullBP.Industry = Convert.ToInt32(partner.UnidadeDeNegocios__c);
                fullBP.U_k33p_SFID = partner.Id;
                fullBP.U_k33p_SFSend = "S";


                fullBP.U_SBZ_GerBol = partner.Forma_de_Pagamento__c == "Boleto" ? "Y" : "N";
                fullBP.U_SBZ_CodCart = partner.Forma_de_Pagamento__c == "Boleto" ? "ITAU-FL02" : "";
                //fullBP.PeymentMethodCode = partner.Forma_de_Pagamento__c == "Boleto" ? await BuscaBoletoAtivoSAP(basePais) : await BuscaDepositoAtivoSAP(basePais);
                //fullBP.PayTermsGrpCode = Convert.ToInt32(partner.CondicaoPagamento__c);
                fullBP.U_CFS_OCRCODE1 = "2001";
                fullBP.U_CFS_OCRCODE2 = await BuscaCFS2(basePais, partner.UnidadeDeNegocios__c);
                //******************fiscais
                //if (partner.BillingCountry.ToUpper() == "BRASIL")
                //{
                //    BPFiscalTaxIDCollectionPatch bpTax = null;
                //    for (int i = 0; i < fullBP.BPAddresses.Count(); i++)
                //    {
                //        bpTax = new BPFiscalTaxIDCollectionPatch();
                //        bpTax.Address = fullBP.BPFiscalTaxIDCollection[i].Address;
                //        bpTax.BPCode = fullBP.BPFiscalTaxIDCollection[i].BPCode;
                //        bpTax.AddrType = fullBP.BPFiscalTaxIDCollection[i].AddrType;
                //        bpTax.TaxId12 = fullBP.BPFiscalTaxIDCollection[i].TaxId12;
                //        bpTax.TaxId0 = partner.CNPJ__c;
                //        bpTax.TaxId1 = partner.InscricaoEstadual__c;

                //        fullBP.BPFiscalTaxIDCollection[i] = bpTax;
                //    }
                //}

                fullBP.BPFiscalTaxIDCollection = fullBP.BPFiscalTaxIDCollection;



                //ContactEmployeePatch bpEmployee = null;
                string ret = "";
                fullBP.ContactEmployees = fullBP.ContactEmployees;// new List<ContactEmployeePatch>();
                int y = 0;
                for (int x = 0; x < partner.Contacts.records.Count(); x++)
                {

                    for (int i = 0; i < fullBP.ContactEmployees.Count(); i++)
                    {

                        if (fullBP.ContactEmployees[i].U_k33p_SFID == null)
                        {
                            y++;
                            continue;
                        }

                        ret = fullBP.ContactEmployees[i].U_k33p_SFID.ToString();
                        if (ret == partner.Contacts.records[x].Id)
                        {
                            fullBP.ContactEmployees[i].Name = partner.Contacts.records[x].Name;
                            fullBP.ContactEmployees[i].LastName = partner.Contacts.records[x].LastName;
                            fullBP.ContactEmployees[i].MiddleName = partner.Contacts.records[x].MiddleName;
                            fullBP.ContactEmployees[i].MobilePhone = partner.Contacts.records[x].MobilePhone;
                            fullBP.ContactEmployees[i].Phone1 = partner.Contacts.records[x].Phone;
                            fullBP.ContactEmployees[i].E_Mail = partner.Contacts.records[x].Email;
                            fullBP.ContactEmployees[i].U_SBZ_EnvEml = partner.Contacts.records[x].RecebeBoleto__c == "Sim" ? "Y" : "N";
                            fullBP.ContactEmployees[i].U_LG_RecEmailXML = partner.Contacts.records[x].RecebeNFe__c == "Sim" ? "S" : "N";
                            partner.Contacts.records.RemoveAt(x);
                        }
                    }
                }

                foreach (var contato in partner.Contacts.records)
                {
                    //var result = fullBP.ContactEmployees.Find(x => x.U_k33p_SFID.Equals(contato.Id));

                    //if (result == null)
                    fullBP.ContactEmployees.Add(new ContactEmployeePatch
                    {
                        Name = contato.Name,
                        Phone1 = contato.Phone,
                        //Phone2 = "",
                        MobilePhone = contato.MobilePhone,
                        E_Mail = contato.Email,
                        FirstName = contato.FirstName,
                        //MiddleName = "",
                        LastName = contato.LastName,
                        U_k33p_SFID = contato.Id,
                        U_SBZ_EnvEml = contato.RecebeBoleto__c == "Sim" ? "Y" : "N",
                        U_LG_RecEmailXML = contato.RecebeNFe__c == "Sim" ? "S" : "N",
                        //U_EhBeneficPag = contato.RecebeBoleto__c == "Sim" ? "S" : "N"


                    });
                }




                //for (int i = 0; i < fullBP.ContactEmployees.Count(); i++)
                //{


                //    //bpEmployee = new ContactEmployeePatch();
                //    fullBP.ContactEmployees[i].Name = partner.Contacts.records[0].Name;
                //    fullBP.ContactEmployees[i].Phone1 = partner.Contacts.records[0].Phone;
                //    //Phone2 = "",
                //    fullBP.ContactEmployees[i].MobilePhone = partner.Contacts.records[0].MobilePhone;
                //    fullBP.ContactEmployees[i].E_Mail = partner.Contacts.records[0].Email;
                //    fullBP.ContactEmployees[i].FirstName = partner.Contacts.records[0].FirstName;
                //    //MiddleName = "",
                //    fullBP.ContactEmployees[i].LastName = partner.Contacts.records[0].LastName;
                //    fullBP.ContactEmployees[i].U_SBZ_EnvEml = fullBP.ContactEmployees[i].U_SBZ_EnvEml;

                //    //fullBP.ContactEmployees[i] = bpEmployee;

                //}

                fullBP.Notes = partner.Description;



                //List<BPPaymentMethod> listaPaym = new List<BPPaymentMethod>();
                //BPPaymentMethod bp2 = new BPPaymentMethod();
                //bp2.PaymentMethodCode = fullBP.PeymentMethodCode;

                for (int i = 0; i < fullBP.BPPaymentMethods.Count(); i++)
                {
                    fullBP.BPPaymentMethods[i].PaymentMethodCode = fullBP.PeymentMethodCode;
                    fullBP.BPPaymentMethods[i].RowNumber = fullBP.BPPaymentMethods[i].RowNumber;
                    fullBP.BPPaymentMethods[i].BPCode = fullBP.BPPaymentMethods[i].BPCode;
                }

                //fullBP.BPPaymentMethods = new List<BPPaymentMethod>();
                //fullBP.BPPaymentMethods.Add(bp2);

                return fullBP;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateBusinessPartnerSAPPatch: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "_populateBusinessPartnerSAPPatch",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaMunicipio(string cidade, string basePais)
        {
            try
            {
                string input = @"âãäåæçèéêëìíîïðñòóôõøùúûüýþÿı";
                string pattern = @"\w+";
                var municipio = cidade;


                var query = SQLSupport.GetConsultas("GetMunicipioByDesc");
                query = string.Format(query, municipio, basePais);

                string ibgeCode = await _sqlAdapter.QueryReaderString(query);


                return ibgeCode.ToString();


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaMunicipioEst(string cidade, string basePais, string pais)
        {
            try
            {
                string input = @"âãäåæçèéêëìíîïðñòóôõøùúûüýþÿı";
                string pattern = @"\w+";
                var municipio = cidade;


                var query = SQLSupport.GetConsultas("GetMunicipioEstByDesc");
                query = string.Format(query, basePais, pais);

                string ibgeCode = await _sqlAdapter.QueryReaderString(query);


                return ibgeCode.ToString();


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaPais(string basePais, string pais)
        {
            try
            {


                var query = SQLSupport.GetConsultas("GetPais");
                query = string.Format(query, basePais, pais);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret.ToString();


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaCFS2(string basePais, string cfs)
        {
            try
            {


                var query = SQLSupport.GetConsultas("GetCFS2");
                query = string.Format(query, basePais, cfs);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret.ToString();


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaUltimoCardCode(string basePais)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetMaxPN_AR");
                query = string.Format(query, basePais);

                string ret = await _sqlAdapter.QueryReaderString(query);

                ret = Regex.Match(ret, @"\d+").Value;

                int max = Convert.ToInt32(ret) + 1;
                var count = ret.TakeWhile(c => c == '0').Count();

                ret = "CL" + max.ToString().PadLeft(count + 2, '0');
                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaEstadoAR(string basePais, string nome)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetEstadoAR");
                query = string.Format(query, basePais, nome);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaMunicipio: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaMunicipio",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<int> GetVendedorByIDSF(string basePais, string id)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetVendedorByIdSF");
                query = string.Format(query, basePais, id);

                string ret = await _sqlAdapter.QueryReaderString(query);

                if (!String.IsNullOrEmpty(ret))
                    return Convert.ToInt32(ret);
                else
                    return -1;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaBoletoAtivoSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaBoletoAtivoSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaBoletoAtivoSAP(string basePais)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetBoletoAtivoSAP");
                query = string.Format(query, basePais);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaBoletoAtivoSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaBoletoAtivoSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<int> BuscaCondPgto(string basePais, string condpgto)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaCondPgto");
                query = string.Format(query, basePais, condpgto);

                string ret = await _sqlAdapter.QueryReaderString(query);



                return String.IsNullOrEmpty(ret) ? 0 : Convert.ToInt32(ret);


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaBoletoAtivoSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaBoletoAtivoSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<int> BuscaSetor(string basePais, string condpgto)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaSetor");
                query = string.Format(query, basePais, condpgto);

                string ret = await _sqlAdapter.QueryReaderString(query);



                return String.IsNullOrEmpty(ret) ? 0 : Convert.ToInt32(ret);


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaBoletoAtivoSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaBoletoAtivoSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaDepositoAtivoSAP(string basePais)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetDepositoAtivoSAP");
                query = string.Format(query, basePais);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaBoletoAtivoSAP: {ex.Message}",
                    Owner = "BusinessPartnersService",
                    Method = "BuscaBoletoAtivoSAP",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

    }
}
