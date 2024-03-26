using AutoMapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Services.Interfaces;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.Models.ARCH;
using Integracao.SAP.SalesForce.Services.Models.QuotationsPatch;
using Integracao.SAP.SalesForce.Services.Models.SF;
using Integracao.SAP.SalesForce.Services.Models.SF.Quote_QuoteItens;
using Integracao.SAP.SalesForce.Services.SQL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Record = Integracao.SAP.SalesForce.Services.Models.SF.Quote_QuoteItens.Record;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class QuotationsService : IQuotationsService
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

        public QuotationsService(ISqlAdapter sqlAdapter,
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

                var cots = await _getQuotationsPendent();

                cots = cots.Where(n => n.Status == "Aprovado").ToList();

                //atualiza data anterior               
                var query = SQLSupport.GetConsultas("SetDataAnteriorCot");
                query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);



                //agrupa por Pais - campo Pais_da_Venda__c
                var listGrouped = cots.GroupBy(x => x.Pais_da_Venda__c).Select(grp => grp.ToList()).ToList();
                foreach (var grouped in listGrouped)
                {
                    lista = new List<Record>();
                    lista = grouped;


                    if (lista != null)
                        await _processQuotations(lista);

                }

                return true;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método ProcessAsync: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "_processPartners",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<Record>> _getQuotationsPendent()
        {
            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+QuoteLineItems+LIMIT+200)+FROM+Quote+WHERE+IsSyncing=True+AND+(Status='Aprovado')+AND+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);
            List<Record> allPartners = new List<Record>();
            Token token = new Token();
            string tokenDB = "";
            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;

            try
            {

                //pega filtro data
                var query = SQLSupport.GetConsultas("GetLastModificationDateCot");
                query = string.Format(query, nomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);
                data = String.IsNullOrEmpty(result) ? data : result;

                endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+QuoteLineItems+LIMIT+200)+FROM+Quote+WHERE+IsSyncing=True+AND+(Status='Aprovado')+AND+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);

                //validaToken
                query = SQLSupport.GetConsultas("GetTokenBD2");
                query = string.Format(query, nomeBaseBR);
                result = await _sqlAdapter.QueryReaderString(query);



                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    var response = await _httpAdapter.Call<Quote_QuoteItens>(HttpMethod.Get, endpointPN, null, _sfHttp.Uri, token.access_token);

                    if (response == null)
                    {
                        token = await _httpAdapter.CallLogin<Token>(HttpMethod.Post, endpointToken, null, _sfHttp.Uri);
                        result = "";

                        //insert token in BD
                        query = SQLSupport.GetConsultas("InsertTokenBD2");
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
                    Message = $"Erro no método _getQuotationsPendent: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "_getQuotationsPendent",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processQuotations(List<Record> cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots)
                {
                    if (cot.Pais_da_Venda__c.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cot.Pais_da_Venda__c.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cot.Pais_da_Venda__c.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;

                    cont++;
                    string query = "";
                    List<string> result = null;


                    query = SQLSupport.GetConsultas("VerificaSeExisteCotByIDSF");
                    query = string.Format(query, basePais, cot.Id);
                    result = await _sqlAdapter.QueryListString(query);



                    if (result.Count == 0)
                        await _InsereUnitQuotation(cot, basePais);


                }

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _processQuotations: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "_processQuotations",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<dynamic> _InsereUnitQuotation(Record cot, string basePais)
        {
            try
            {

                var response = await _createCot(cot, basePais);

                if (response != null)
                {
                    //atualiza flag data
                    string data = DateTime.Now.AddHours(3).AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    var query = SQLSupport.GetConsultas("SetLastModificationDateCot");
                    query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, cot.Id);
                    var result = await _sqlAdapter.QueryReaderString(query);

                }

                return null;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _InsereUnitQuotation: {ex.Message}",
                    Owner = "QuotationService",
                    Method = "_InsereUnitQuotation",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<QuotationsDTORetorno> _createCot(Record cot, string basePais)
        {

            QuotationsDTORetorno responseDraft = null;
            QuotationsDTOARCH cotacaoVenda2 = null;
            QuotationsDTO cotacaoVenda = null;

            if (basePais == "SBOPRODAR" || basePais == "SBOPRODCH")
            {
                cotacaoVenda2 = await _populateCotSAPARCH(cot, basePais);
            }
            else
            {
                cotacaoVenda = await _populateCotSAP(cot, basePais);
            }

            //var cotacaoVenda = await _populateCotSAP(cot, basePais);

            try
            {
                if (cotacaoVenda2 != null || cotacaoVenda != null)
                {
                    if (cotacaoVenda != null)
                    {
                        string idcot = cotacaoVenda != null ? cotacaoVenda.U_k33p_SFID : cotacaoVenda2.U_k33p_SFID;
                        string filialCot = cotacaoVenda != null ? cotacaoVenda.BPL_IDAssignedToInvoice.ToString() : cotacaoVenda2.BPL_IDAssignedToInvoice.ToString();

                        //se ja existir draft nao cria outro
                        var query = SQLSupport.GetConsultas("GetExistsDraft");
                        query = string.Format(query, basePais, idcot);
                        string retDr = await _sqlAdapter.QueryReaderString(query);

                        if (retDr != "")
                        {
                            await _logger.Logger(new LogIntegration
                            {
                                LogTypeCode = 1,
                                Message = $"Erro - Já existe um Draft pra essa cotação na base - Filial:  {filialCot} -  Cotação IDSF :  {idcot} ",
                                Owner = "QuotationsService",
                                Method = "_createCot",
                                Key = cot.Id,
                                Key2 = cot.Id,
                                Company = basePais
                            });
                            return null;
                        }


                        //geraDraft
                        responseDraft = await _serviceLayerAdapter.Call<QuotationsDTORetorno>(
                             "Drafts", HttpMethod.Post, cotacaoVenda == null ? cotacaoVenda2 : cotacaoVenda, _serviceLayerHttp.Uri, null, cot.Pais_da_Venda__c);

                        #region OLD Calcula Impostos


                        //calcula Impostos e pacth no Draft(pega DocEntry do Drat acima no objeto responseDraft) - 
                        //var ret = await CalculaImpostos(basePais, responseDraft.DocEntry.ToString(), true);

                        //if (ret == null)
                        //{
                        //    await _logger.Logger(new LogIntegration
                        //    {
                        //        LogTypeCode = 1,
                        //        Message = $"Erro no método CalculaImpostos: {"Esboço não pode carregar os impostos"}",
                        //        Owner = "QuotationsService",
                        //        Method = "CalculaImpostos",
                        //        Key = cot.Id,
                        //        Key2 = cot.Id,
                        //        Company = basePais
                        //    });
                        //    return null;
                        //}

                        ////PATCh DRAFT//////////////////////////////
                        //double vlrSomar = 0;
                        //foreach (var soma in ret)
                        //{
                        //    vlrSomar += soma.TaxSumFrgn;// ret.
                        //}



                        //int i = 0;
                        //double vlrFinalSomar = 0;
                        //foreach (var linhaCot in cotacaoVenda.DocumentLines)
                        //{
                        //    vlrFinalSomar = 0;
                        //    vlrFinalSomar = vlrSomar / linhaCot.Quantity;
                        //    cotacaoVenda.DocumentLines[i].UnitPrice = cotacaoVenda.DocumentLines[i].U_k33p_SFUPrice + vlrFinalSomar;
                        //    cotacaoVenda.DocumentLines[i].Price = cotacaoVenda.DocumentLines[i].U_k33p_SFUPrice + vlrFinalSomar;
                        //    i++;
                        //}

                        #endregion


                        //valida se esboço criou impostos
                        var consulta = SQLSupport.GetConsultas("ValidaDRF4");
                        consulta = string.Format(consulta, basePais, responseDraft.DocEntry);

                        string strRet = await _sqlAdapter.QueryReaderString(consulta);

                        if (String.IsNullOrEmpty(strRet) || strRet == "0")
                        {
                            await _logger.Logger(new LogIntegration
                            {
                                LogTypeCode = 1,
                                Message = $"Erro - {responseDraft.error.message.value} -  Linha sem imposto no Rascunho: Verifique as parametrizações para a filial correspondente - Filial:  {filialCot} -  Cotação ID :  {idcot} - Draft :{responseDraft.DocNum}",
                                Owner = "QuotationsService",
                                Method = "_createCot",
                                Key = cot.Id,
                                Key2 = cot.Id,
                                Company = basePais,
                                RequestObject = cotacaoVenda != null ? JsonSerializer.Serialize<QuotationsDTO>(cotacaoVenda) : JsonSerializer.Serialize<QuotationsDTOARCH>(cotacaoVenda2)
                            });
                            return null;
                        }



                        //double docTotal = 0;
                        //foreach (var linhaCot in cotacaoVenda.DocumentLines)
                        //{
                        //    docTotal += linhaCot.Quantity * linhaCot.UnitPrice;

                        //}

                        if (basePais == "SBOPRODBR")
                        {
                            int i = 0;
                            foreach (var linhaCot in responseDraft.DocumentLines)
                            {
                                var ret = await GetMemoriaCalculoNEW(basePais, linhaCot.TaxCode);// responseDraft.DocEntry.ToString());

                                if (ret < 0)
                                {
                                    await _logger.Logger(new LogIntegration
                                    {
                                        LogTypeCode = 1,
                                        Message = $"Erro - Coeficiente nao preenchido - Filial:  {filialCot} -  Cotação ID :  {idcot} - Draft :{responseDraft.DocNum}",
                                        Owner = "QuotationsService",
                                        Method = "_createCot",
                                        Key = cot.Id,
                                        Key2 = cot.Id,
                                        Company = basePais,
                                        RequestObject = cotacaoVenda != null ? JsonSerializer.Serialize<QuotationsDTO>(cotacaoVenda) : JsonSerializer.Serialize<QuotationsDTOARCH>(cotacaoVenda2)
                                    });
                                    return null;
                                }


                                //ret = (docTotal / ret) / linhaCot.Quantity;
                                ret = ((linhaCot.Quantity * linhaCot.UnitPrice) / ret) / linhaCot.Quantity;
                                cotacaoVenda.DocumentLines[i].UnitPrice = ret;
                                i++;
                            }
                        }

                    }


                    //POST QUOTATION/////////////////////////////
                    var responseDraft2 = await _serviceLayerAdapter.Call<QuotationsDTORetorno>(
                          $"Quotations", HttpMethod.Post, cotacaoVenda == null ? cotacaoVenda2 : cotacaoVenda, _serviceLayerHttp.Uri, null, cot.Pais_da_Venda__c);

                    //log POST QUOTATION

                    #region old
                    ///*************************************NOVO********************************************************8
                    //  var ret2 = await CalculaImpostos(basePais, responseDraft2.DocEntry.ToString(), false);

                    //  if (ret2 == null)
                    //  {
                    //      await _logger.Logger(new LogIntegration
                    //      {
                    //          LogTypeCode = 1,
                    //          Message = $"Erro no método CalculaImpostos: {"Cotação não pode carregar os impostos"}",
                    //          Owner = "QuotationsService",
                    //          Method = "CalculaImpostos",
                    //          Key = cot.Id,
                    //          Key2 = cot.Id,
                    //          Company = basePais
                    //      });
                    //      return null;
                    //  }

                    //  //PATCh DRAFT//////////////////////////////
                    //  vlrSomar = 0;
                    //  foreach (var soma in ret)
                    //  {
                    //      vlrSomar += soma.TaxSumFrgn;// ret.
                    //  }

                    //  QuotationsPatch quotPatch = new QuotationsPatch();
                    //  Integracao.SAP.SalesForce.Services.Models.QuotationsPatch.DocumentLine linePatch = null;
                    //  List<Integracao.SAP.SalesForce.Services.Models.QuotationsPatch.DocumentLine> linhasPatch = new List<Models.QuotationsPatch.DocumentLine>();


                    //  i = 0;
                    //  vlrFinalSomar = 0;
                    //  foreach (var linhaCot in cotacaoVenda.DocumentLines)
                    //  {

                    //      linePatch = new Integracao.SAP.SalesForce.Services.Models.QuotationsPatch.DocumentLine();

                    //      vlrFinalSomar = 0;
                    //      vlrFinalSomar = vlrSomar / linhaCot.Quantity;
                    //      linePatch.UnitPrice = cotacaoVenda.DocumentLines[i].U_k33p_SFUPrice + vlrFinalSomar;
                    //      linePatch.Price = cotacaoVenda.DocumentLines[i].U_k33p_SFUPrice + vlrFinalSomar;

                    //      linhasPatch.Add(linePatch);

                    //      i++;
                    //  }

                    //  quotPatch.DocumentLines = linhasPatch;

                    //  //tax
                    //  LineTaxJurisdiction tax = null;
                    //  List<LineTaxJurisdiction> listTax = new List<LineTaxJurisdiction>();
                    //  //listTax = await CalculaLineTax(basePais, responseDraft2.DocEntry.ToString(), false);

                    //  foreach (var linhaTax in ret2)
                    //  {
                    //      tax = new LineTaxJurisdiction();
                    //      tax.JurisdictionType = linhaTax.Imposto == "COFINS-S/ICMS" ? 27 : linhaTax.Imposto == "ICMS" ? 10 : linhaTax.Imposto == "IPI" ? 16 : linhaTax.Imposto == "PIS-S/ICMS" ? 26 : 0;
                    //      tax.TaxAmountFC = linhaTax.TaxSumFrgn;
                    //      tax.TaxRate = linhaTax.TaxRate; 

                    //      listTax.Add(tax);
                    //  }

                    //  quotPatch.LineTaxJurisdictions = listTax;
                    //  //xax

                    //  var responseDraft3 = await _serviceLayerAdapter.Call<QuotationsPatch>(
                    //$"Quotations({responseDraft2.DocEntry})", HttpMethod.Patch, quotPatch, _serviceLayerHttp.Uri, null, cot.Pais_da_Venda__c);

                    #endregion

                    //log PATCH Quotation
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Inserção de Cot: {responseDraft2.DocEntry} ",
                        Owner = "QuotationsService",
                        Method = "_createCot",
                        Key = responseDraft2.DocEntry.ToString(),
                        Key2 = responseDraft2.U_k33p_SFID,
                        Company = basePais,
                        RequestObject = cotacaoVenda != null ? JsonSerializer.Serialize<QuotationsDTO>(cotacaoVenda) : JsonSerializer.Serialize<QuotationsDTOARCH>(cotacaoVenda2),
                        ResponseObject = JsonSerializer.Serialize<QuotationsDTORetorno>(responseDraft2)
                    });

                    ///*************************************NOVO********************************************************8

                    //DELETE DRAFT
                    if (cotacaoVenda != null)
                         await _serviceLayerAdapter.Call<QuotationsDTO>(
                              $"Drafts({responseDraft.DocEntry.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.Pais_da_Venda__c);




                    return responseDraft2;

                }


                return null;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _createCot: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "_createCot",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<QuotationsDTO> _populateCotSAP(Record cotacao, string basePais)
        {
            var cot = new QuotationsDTO();// _mapper.Map<Record, BusinessPartnersDTO>(partner);
            int line = 0;

            try
            {
                //if (cotacao.ExpirationDate == null)
                //{
                //    await _logger.Logger(new LogIntegration
                //    {
                //        LogTypeCode = 1,
                //        Message = $"Erro no método _populateCotSAP: {"Data de Vencimento: (ExpirationDate) está nula"}",
                //        Owner = "QuotationsService",
                //        Method = "_populateCotSAP",
                //        Key = cotacao.Id,
                //        Key2 = cotacao.Id,
                //        Company = basePais
                //    });
                //    return null;
                //}
                ////CAB
                cot.DocObjectCode = "23";
                cot.DocDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.TaxDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.DocDueDate = cotacao.ExpirationDate == null ? DateTime.Today.AddDays(30).ToString("yyyy-MM-dd") : Convert.ToDateTime(cotacao.ExpirationDate).ToString("yyyy-MM-dd");
                cot.DocRate = cotacao.TaxaPreDefinida__c == "Não" ? await BuscaTaxaDia(basePais) : cotacao.TaxaR__c;
                cot.DocCurrency = "USD";
                cot.BPL_IDAssignedToInvoice = Convert.ToInt32(cotacao.Filial_Faturamento__c);
                cot.Comments = cotacao.Description;
                cot.NumAtCard = cotacao.NumeroPedidoCompra__c;
                cot.CardCode = await BuscaPN(basePais, cotacao.AccountId);
                cot.Series = 14;
                cot.U_k33p_SFSMsg = "";
                cot.U_k33p_SFID = cotacao.Id;
                cot.U_k33p_SFSend = "N";
                cot.SalesPersonCode = await GetVendedorByIDSF(basePais, cotacao.OwnerId);
                cot.PaymentGroupCode = Convert.ToInt32(cotacao.CondicaoPagamento__c == null ? -1 : cotacao.CondicaoPagamento__c);//GroupNum OCRD  *******ver a necessidade de colocar cond pagto no json - senao pegar da OCRD e por no JSON 
                cot.PaymentMethod = await BuscaPayMethod(basePais, cot.CardCode);  //"CB_ITAU"  ver regra de Produção (B) Boleto Itau / (T) Depósito ABC - no SF virá Depósito / Boleto
                cot.DocumentsOwner = await GetVendedorByIDSF2(basePais, cot.SalesPersonCode);



                if (cot.DocRate == 0)
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Erro no método _populateCotSAP: {"Taxa do Dia não preenchida"}",
                        Owner = "QuotationsService",
                        Method = "_populateCotSAP",
                        Key = cotacao.Id,
                        Key2 = cotacao.Id,
                        Company = basePais
                    });
                    return null;
                }

                //LINHAS
                var documentLines = new List<Integracao.SAP.SalesForce.Services.Models.DocumentLine>();
                foreach (var item in cotacao.QuoteLineItems.records)
                {
                    string cc1 = "2001"; //await BuscaCC1(basePais, cotacao.AccountId);
                    string cc2 = await BuscaCC2(basePais, cotacao.AccountId);
                    string cc3 = await BuscaCC3(basePais, cotacao.OwnerId);

                    string codProd = await BuscaProd(basePais, item.NomeDoProduto__c, item.TipoEmbalagem__c);

                    documentLines.Add(new Integracao.SAP.SalesForce.Services.Models.DocumentLine
                    {
                        ItemCode = codProd,
                        Currency = "USD",
                        Rate = cot.DocRate,
                        CostingCode = cc1,
                        CostingCode2 = cc2,
                        //CostingCode3= "",
                        //CostingCode4 = "",
                        CostingCode5 = cc3,
                        COGSCostingCode = cc1,
                        COGSCostingCode2 = cc2,
                        //COGSCostingCode3 = "",
                        //COGSCostingCode4 = "",
                        COGSCostingCode5 = cc3,
                        WarehouseCode = await BuscaDep(basePais, cot.BPL_IDAssignedToInvoice), //DflWhs FROM OBPL WHERE "BPLId" = Convert.ToInt32(cotacao.Filial_Faturamento__c) "INDA-01",//
                        Quantity = item.Quantity,
                        Price = item.UnitPrice,
                        UnitPrice = item.UnitPrice,
                        DiscountPercent = item.Discount == null ? 0 : item.Discount,
                        Usage = Convert.ToInt32(cotacao.TipoCotacao__c),
                        U_k33p_SFID = item.Id,
                        U_k33p_SFUPrice = item.UnitPrice,
                        U_LG_xPed = cotacao.NumeroPedidoCompra__c,
                        U_LG_nItemPed = item.Numero_Item_Pedido__c,
                        U_CFS_SETOR_APLIC = cotacao.SetorAplicacao__c,
                        ShipDate = item.DataEntrega__c,
                        SupplierCatNum = await BuscaCatNum(basePais, cot.CardCode, codProd)


                    });
                    line++;
                }


                cot.DocumentLines = documentLines;

                TaxExtension taxExtension = new TaxExtension();
                taxExtension.Incoterms = cotacao.Frete__c;

                cot.TaxExtension = taxExtension;

                return cot;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateCotSAP: {ex.Message}",
                    Owner = "QuotationService",
                    Method = "_populateCotSAP",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<QuotationsDTOARCH> _populateCotSAPARCH(Record cotacao, string basePais)
        {
            var cot = new QuotationsDTOARCH();// _mapper.Map<Record, BusinessPartnersDTO>(partner);
            int line = 0;

            try
            {
                //if (cotacao.ExpirationDate == null)
                //{
                //    await _logger.Logger(new LogIntegration
                //    {
                //        LogTypeCode = 1,
                //        Message = $"Erro no método _populateCotSAP: {"Data de Vencimento: (ExpirationDate) está nula"}",
                //        Owner = "QuotationsService",
                //        Method = "_populateCotSAP",
                //        Key = cotacao.Id,
                //        Key2 = cotacao.Id,
                //        Company = basePais
                //    });
                //    return null;
                //}
                ////CAB
                cot.DocObjectCode = "23";
                cot.DocDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.TaxDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.DocDueDate = cotacao.ExpirationDate == null ? DateTime.Today.AddDays(30).ToString("yyyy-MM-dd") : Convert.ToDateTime(cotacao.ExpirationDate).ToString("yyyy-MM-dd");
                cot.DocRate = cotacao.TaxaPreDefinida__c == "Não" ? await BuscaTaxaDia(basePais) : cotacao.TaxaR__c;
                cot.DocCurrency = "USD";
                cot.BPL_IDAssignedToInvoice = Convert.ToInt32(cotacao.Filial_Faturamento__c);
                cot.Comments = cotacao.Description;
                cot.NumAtCard = cotacao.NumeroPedidoCompra__c;
                cot.CardCode = await BuscaPN(basePais, cotacao.AccountId);
                //cot.Series = 14;
                cot.U_k33p_SFSMsg = "";
                cot.U_k33p_SFID = cotacao.Id;
                cot.U_k33p_SFSend = "N";
                cot.SalesPersonCode = await GetVendedorByIDSF(basePais, cotacao.OwnerId);
                cot.PaymentGroupCode = Convert.ToInt32(cotacao.CondicaoPagamento__c == null ? -1 : cotacao.CondicaoPagamento__c);//GroupNum OCRD  *******ver a necessidade de colocar cond pagto no json - senao pegar da OCRD e por no JSON 
                cot.PaymentMethod = await BuscaPayMethod(basePais, cot.CardCode);  //"CB_ITAU"  ver regra de Produção (B) Boleto Itau / (T) Depósito ABC - no SF virá Depósito / Boleto
                //cot.DocumentsOwner = await GetVendedorByIDSF2(basePais, cot.SalesPersonCode);



                if (cot.DocRate == 0)
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Erro no método _populateCotSAP: {"Taxa do Dia não preenchida"}",
                        Owner = "QuotationsService",
                        Method = "_populateCotSAP",
                        Key = cotacao.Id,
                        Key2 = cotacao.Id,
                        Company = basePais
                    });
                    return null;
                }

                //LINHAS
                var documentLines = new List<Integracao.SAP.SalesForce.Services.Models.DocumentLineARCH>();
                foreach (var item in cotacao.QuoteLineItems.records)
                {
                    string cc1 = "2001"; //await BuscaCC1(basePais, cotacao.AccountId);
                    string cc2 = await BuscaCC2(basePais, cotacao.AccountId);
                    string cc3 = await BuscaCC3(basePais, cotacao.OwnerId);

                    string codProd = await BuscaProd(basePais, item.NomeDoProduto__c, item.TipoEmbalagem__c);

                    documentLines.Add(new Integracao.SAP.SalesForce.Services.Models.DocumentLineARCH
                    {
                        ItemCode = codProd,
                        Currency = "USD",
                        Rate = cot.DocRate,
                        CostingCode = cc1,
                        CostingCode2 = cc2,
                        //CostingCode3= "",
                        //CostingCode4 = "",
                        CostingCode5 = cc3,
                        COGSCostingCode = cc1,
                        COGSCostingCode2 = cc2,
                        //COGSCostingCode3 = "",
                        //COGSCostingCode4 = "",
                        COGSCostingCode5 = cc3,
                        //WarehouseCode = await BuscaDep(basePais, cot.BPL_IDAssignedToInvoice), //DflWhs FROM OBPL WHERE "BPLId" = Convert.ToInt32(cotacao.Filial_Faturamento__c) "INDA-01",//
                        Quantity = item.Quantity,
                        Price = item.UnitPrice,
                        UnitPrice = item.UnitPrice,
                        DiscountPercent = item.Discount == null ? 0 : item.Discount,
                        //Usage = Convert.ToInt32(cotacao.TipoCotacao__c),
                         U_k33p_SFID = item.Id,
                        // U_k33p_SFUPrice = item.UnitPrice,
                        //U_LG_xPed = cotacao.NumeroPedidoCompra__c,
                        //U_LG_nItemPed = item.Numero_Item_Pedido__c,
                        //U_CFS_SETOR_APLIC = cotacao.SetorAplicacao__c,
                        ShipDate = item.DataEntrega__c,
                        SupplierCatNum = await BuscaCatNum(basePais, cot.CardCode, codProd)


                    });
                    line++;
                }


                cot.DocumentLines = documentLines;

                TaxExtensionARCH taxExtension = new TaxExtensionARCH();
                taxExtension.Incoterms = cotacao.Frete__c;

                cot.TaxExtension = taxExtension;

                return cot;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateCotSAP: {ex.Message}",
                    Owner = "QuotationService",
                    Method = "_populateCotSAP",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }


        private async Task<string> BuscaCatNum(string basePais, string pn, string item)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaCatNum");
                query = string.Format(query, basePais, pn, item);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<double> BuscaTaxaDia(string basePais)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyy-MM-dd");
                var query = SQLSupport.GetConsultas("GetTaxaDia");
                query = string.Format(query, basePais, data);

                string ret = await _sqlAdapter.QueryReaderString(query);

                if (String.IsNullOrEmpty(ret))
                {
                    return 0;
                }


                return Convert.ToDouble(ret.Replace(",", "."), CultureInfo.InvariantCulture);


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaTaxaDia: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaTaxaDia",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<string> BuscaPN(string basePais, string accountId)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetPNCot");
                query = string.Format(query, basePais, accountId);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaPN: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaPN",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<int> BuscaVendedor(string basePais, string email)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetVendedorCot");
                query = string.Format(query, basePais, email);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return Convert.ToInt32(String.IsNullOrEmpty(ret) ? 0 : ret);


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaVendedor: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaVendedor",
                    Key = "",
                    Key2 = "",
                    Company = basePais
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

        private async Task<int> GetVendedorByIDSF2(string basePais, int id)
        {
            try
            {

                var query = SQLSupport.GetConsultas("GetVendedorByIdSF2");
                query = string.Format(query, basePais, id);

                string ret = await _sqlAdapter.QueryReaderString(query);

                if (!String.IsNullOrEmpty(ret))
                    return Convert.ToInt32(ret);
                else
                    return 0;


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
        private async Task<string> BuscaPayMethod(string basePais, string cardCode)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetPayMethodCot");
                query = string.Format(query, basePais, cardCode);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaVendedor: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaVendedor",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        /// <param name="basePais"></param>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> BuscaDep(string basePais, int filial)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetDep");
                query = string.Format(query, basePais, filial);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaDep: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaDep",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaProd(string basePais, string prod, string emb)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaProd");
                query = string.Format(query, basePais, prod, emb);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaDep: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaDep",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaCC1(string basePais, string email)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC1");
                query = string.Format(query, basePais, email);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC1: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC1",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaUsage(string basePais, string id)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaUsage");
                query = string.Format(query, basePais, id);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaUsage: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaUsage",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<string> BuscaCC2(string basePais, string cardCode)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC2");
                query = string.Format(query, basePais, cardCode);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC2: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC2",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<string> BuscaCC3(string basePais, string cardCode)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC3");
                query = string.Format(query, basePais, cardCode);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }


        private async Task<List<CalculaImposto>> CalculaImpostos(string basePais, string numeroDraft, bool isDraft)
        {
            try
            {
                string query = "";
                double totRate = 0;
                double totTaxSum = 0;
                List<CalculaImposto> listaImp = new List<CalculaImposto>();

                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                if (isDraft)
                    query = SQLSupport.GetConsultas("GetDadosImpostos");
                else
                    query = SQLSupport.GetConsultas("GetDadosImpostos2");

                query = string.Format(query, basePais, numeroDraft);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        CalculaImposto ci = new CalculaImposto();

                        //for (int x = 0; x < sql.FieldCount; x++)
                        //{
                        ci.Imposto = sql.GetValue(0).ToString();
                        ci.TaxRate = Convert.ToDouble(sql.GetValue(1).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.TaxSum = Convert.ToDouble(sql.GetValue(2).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.TaxSumFrgn = Convert.ToDouble(sql.GetValue(3).ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                        totRate += ci.TaxRate;
                        totTaxSum += ci.TaxSum;
                        //}

                        listaImp.Add(ci);

                        //i++;
                    }
                }
                else
                {
                    return null;
                }

                sql.Close();
                conexao.Close();

                return listaImp;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<LineTaxJurisdiction>> CalculaLineTax(string basePais, string numeroDraft, bool isDraft)
        {
            try
            {
                string query = "";
                double totRate = 0;
                double totTaxSum = 0;
                List<LineTaxJurisdiction> listaImp = new List<LineTaxJurisdiction>();

                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";


                query = SQLSupport.GetConsultas("GetLineTax");

                query = string.Format(query, basePais, numeroDraft);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        LineTaxJurisdiction ci = new LineTaxJurisdiction();

                        //for (int x = 0; x < sql.FieldCount; x++)
                        //{
                        ci.JurisdictionType = Convert.ToInt32(sql.GetValue(0).ToString());
                        ci.TaxRate = Convert.ToDouble(sql.GetValue(1).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.TaxAmountFC = Convert.ToDouble(sql.GetValue(2).ToString().Replace(",", "."), CultureInfo.InvariantCulture);



                        listaImp.Add(ci);

                        //i++;
                    }
                }
                else
                {
                    return null;
                }

                sql.Close();
                conexao.Close();

                return listaImp;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<double> GetMemoriaCalculo(string basePais, string numeroDraft, int linha)
        {
            try
            {
                double ret = 0;
                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";


                var query = SQLSupport.GetConsultas("GetMemoriaCalculo");
                query = string.Format(query, basePais, numeroDraft, linha);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        ret = Convert.ToDouble(sql.GetValue(0).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return 0;
                }

                sql.Close();
                conexao.Close();

                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<double> GetMemoriaCalculoNEW(string basePais, string numeroDraft)
        {
            try
            {
                double ret = 0;
                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";


                var query = SQLSupport.GetConsultas("GetMemoriaCalculoNEW");
                query = string.Format(query, basePais, numeroDraft);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        ret = Convert.ToDouble(sql.GetValue(0).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    return 0;
                }

                sql.Close();
                conexao.Close();

                return ret;


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC3: {ex.Message}",
                    Owner = "QuotationsService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

    }
}
