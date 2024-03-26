using AutoMapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Services.Interfaces;
using Integracao.SAP.SalesForce.Services.Models.SF.Order_OrderItem;
using Integracao.SAP.SalesForce.Services.Models.SF;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.SQL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Record = Integracao.SAP.SalesForce.Services.Models.SF.Order_OrderItem.Record;
using Integracao.SAP.SalesForce.Services.Models.OrdersDTO;
using System.Reflection.Metadata.Ecma335;
using System.Collections;
using Integracao.SAP.SalesForce.Services.Models.OrdersDTORetorno;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class OrdersService : IOrdersService
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

        public OrdersService(ISqlAdapter sqlAdapter,
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

                var cots = await _getOrdersPendent();

                cots = cots.Where(n => n.Status == "Aberto").ToList();


                //agrupa por Pais - campo BillingCountry
                var listGrouped = cots.GroupBy(x => x.BillingCountry).Select(grp => grp.ToList()).ToList();
                foreach (var grouped in listGrouped)
                {
                    lista = new List<Record>();
                    lista = grouped;


                    if (lista != null)
                        await _processOrders(lista);

                }

                return true;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método ProcessAsync: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "_processPartners",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<Record>> _getOrdersPendent()
        {
            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            //var endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);
            var endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+Status='Aberto'+LIMIT+200", apiVersion);
            List<Record> allPartners = new List<Record>();
            Token token = new Token();
            string tokenDB = "";
            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;

            try
            {

                //pega filtro data
                var query = SQLSupport.GetConsultas("GetLastModificationDatePed");
                query = string.Format(query, nomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);
                data = String.IsNullOrEmpty(result) ? data : result;

                endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+Status='Aberto'+LIMIT+200", apiVersion); ;// String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);

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

                    var response = await _httpAdapter.Call<Order_OrderItem>(HttpMethod.Get, endpointPN, null, _sfHttp.Uri, token.access_token);

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
                    Message = $"Erro no método _getOrdersPendent: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "_getOrdersPendent",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processOrders(List<Record> cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots)
                {
                    if (cot.BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cot.BillingCountry.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cot.BillingCountry.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;

                    cont++;
                    string query = "";
                    List<string> result = null;


                    query = SQLSupport.GetConsultas("VerificaSeExistePedByIDSF");
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
                    Message = $"Erro no método _processOrders: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "_processOrders",
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
                    var query = SQLSupport.GetConsultas("SetLastModificationDatePed");
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
                    Owner = "Orderservice",
                    Method = "_InsereUnitQuotation",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<OrdersDTORetorno> _createCot(Record cot, string basePais)
        {

            var cotacaoVenda = await _populateCotSAP(cot, basePais);

            try
            {
                if (cotacaoVenda != null)
                {

                    //se ja existir draft apaga
                    var query = SQLSupport.GetConsultas("GetExistsDraft");
                    query = string.Format(query, basePais, cotacaoVenda.U_k33p_SFID);
                    string retDr = await _sqlAdapter.QueryReaderString(query);

                    if (retDr != "")
                    {
                        var responseDraft4 = await _serviceLayerAdapter.Call<QuotationsDTO>(
                              $"Drafts({retDr.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.BillingCountry);
                    }


                    //if (cotacaoVenda.DocumentLines[0].TaxCode == null)
                    //{
                    //    await _logger.Logger(new LogIntegration
                    //    {
                    //        LogTypeCode = 1,
                    //        Message = $"Erro - Linha sem imposto: Cotação ID :  {cotacaoVenda.U_k33p_SFID}",
                    //        Owner = "QuotationsService",
                    //        Method = "_createCot",
                    //        Key = cot.Id,
                    //        Key2 = cot.Id,
                    //        Company = basePais
                    //    });
                    //    return null;
                    //}

                    //geraDraft
                    var responseDraft = await _serviceLayerAdapter.Call<QuotationsDTO>(
                          "Drafts", HttpMethod.Post, cotacaoVenda, _serviceLayerHttp.Uri, null, cot.BillingCountry);


                    //valida se esboço criou impostos
                    var consulta = SQLSupport.GetConsultas("ValidaDRF4");
                    consulta = string.Format(consulta, basePais, responseDraft.DocEntry);

                    string strRet = await _sqlAdapter.QueryReaderString(consulta);

                    if (String.IsNullOrEmpty(strRet) || strRet == "0")
                    {
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Erro - Linha sem imposto no Rascunho: Verifique as parametrizações para a filial correspondente - Filial:  {cotacaoVenda.BPL_IDAssignedToInvoice} -  Cotação ID :  {cotacaoVenda.U_k33p_SFID} - Draft :{responseDraft.DocNum}",
                            Owner = "QuotationsService",
                            Method = "_createCot",
                            Key = cot.Id,
                            Key2 = cot.Id,
                            Company = basePais
                        });
                        return null;
                    }

                    //log  POST DRAFT


                    //calcula Impostos e pacth no Draft(pega DocEntry do Drat acima no objeto responseDraft) - 
                    // var ret = await CalculaImpostos(basePais, responseDraft.DocEntry.ToString(), true);

                    //double docTotal = 0;
                    //foreach (var linhaCot in cotacaoVenda.DocumentLines)
                    //{
                    //    docTotal += linhaCot.Quantity * linhaCot.UnitPrice;

                    //}
                    int i = 0;
                    foreach (var linhaCot in responseDraft.DocumentLines)
                    {
                        var ret = await GetMemoriaCalculoNEW(basePais, linhaCot.TaxCode);// responseDraft.DocEntry.ToString());


                        if (ret < 0)
                        {
                            await _logger.Logger(new LogIntegration
                            {
                                LogTypeCode = 1,
                                Message = $"Erro - Coeficiente nao preenchido - Filial:  {cotacaoVenda.BPL_IDAssignedToInvoice} -  Cotação ID :  {cotacaoVenda.U_k33p_SFID} - Draft :{responseDraft.DocNum}",
                                Owner = "QuotationsService",
                                Method = "_createCot",
                                Key = cot.Id,
                                Key2 = cot.Id,
                                Company = basePais
                            });
                            return null;
                        }


                        //ret = (docTotal / ret) / linhaCot.Quantity;
                        ret = ((linhaCot.Quantity * linhaCot.UnitPrice) / ret) / linhaCot.Quantity;
                        cotacaoVenda.DocumentLines[i].UnitPrice = ret;
                        i++;
                    }
                    //foreach (var linhaCot in cotacaoVenda.DocumentLines)
                    //{
                    //    var ret = await GetMemoriaCalculoNEW(basePais, responseDraft.DocEntry.ToString());
                    //    ret = (docTotal / ret) / linhaCot.Quantity;
                    //    cotacaoVenda.DocumentLines[i].UnitPrice = ret;
                    //    i++;
                    //}




                    //POST QUOTATION/////////////////////////////
                    var responseDraft2 = await _serviceLayerAdapter.Call<OrdersDTORetorno>(
                          $"Orders", HttpMethod.Post, cotacaoVenda, _serviceLayerHttp.Uri, null, cot.BillingCountry);

                    //log PATCH Quotation
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Inserção de Ped: {responseDraft2.DocEntry} ",
                        Owner = "OrdersService",
                        Method = "_createCot",
                        Key = responseDraft2.DocEntry.ToString(),
                        Key2 = responseDraft2.U_k33p_SFID,
                        Company = basePais,
                        RequestObject = JsonSerializer.Serialize<OrdersDTO>(cotacaoVenda),
                        ResponseObject = JsonSerializer.Serialize<OrdersDTORetorno>(responseDraft2)
                    });

                    var responseDraft45 = await _serviceLayerAdapter.Call<QuotationsDTO>(
                          $"Drafts({responseDraft.DocEntry.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.BillingCountry);

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
                    Owner = "OrdersService",
                    Method = "_createCot",
                    Key = "",
                    Key2 = "",
                    Company = ""
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

        private async Task<OrdersDTO> _populateCotSAP(Record cotacao, string basePais)
        {
            var cot = new OrdersDTO();// _mapper.Map<Record, BusinessPartnersDTO>(partner);
            int line = 0;

            try
            {

                var listaEmailFilial = await BuscaEmailFilialByQuotation(cotacao.QuoteId, basePais);

                if (listaEmailFilial[0].Contains("Erro:"))
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Erro no método _populateCotSAP: {listaEmailFilial[0].ToString()}",
                        Owner = "OrdersService",
                        Method = "_populateCotSAP",
                        Key = cotacao.Id,
                        Key2 = cotacao.Id,
                        Company = basePais
                    });
                    return null;
                }

                //CAB
                cot.DocObjectCode = "17";
                cot.DocDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.TaxDate = DateTime.Today.ToString("yyyy-MM-dd");
                cot.DocDueDate = await BuscaDataVencCot(basePais, cotacao.QuoteId); ;// cotacao.EffectiveDate == null ? DateTime.Today.ToString("yyyy-MM-dd") : Convert.ToDateTime(cotacao.EffectiveDate).ToString("yyyy-MM-dd"); //Convert.ToDateTime(cotacao.EffectiveDate).ToString("yyyy-MM-dd");
                cot.DocRate = cotacao.TaxaPreDefinida__c == "Não" ? await BuscaTaxaDia(basePais) : cotacao.TaxaR__c;
                cot.DocCurrency = "USD";
                cot.BPL_IDAssignedToInvoice = Convert.ToInt32(listaEmailFilial[0].ToString());
                //cot.Comments = cotacao.Name;
                cot.CardCode = await BuscaPN(basePais, cotacao.AccountId);
                //cot.Series = 14;
                cot.U_k33p_SFSMsg = "";
                cot.U_k33p_SFID = cotacao.Id;
                cot.U_k33p_SFSend = "N";
                cot.SalesPersonCode = await BuscaVendedor(basePais, listaEmailFilial[1]);// cotacao.Email);
                cot.PaymentGroupCode = Convert.ToInt32(cotacao.CondicaoPagamento__c == null ? -1 : cotacao.CondicaoPagamento__c);//GroupNum OCRD  *******ver a necessidade de colocar cond pagto no json - senao pegar da OCRD e por no JSON 
                cot.PaymentMethod = await BuscaPayMethod(basePais, cot.CardCode);  //"CB_ITAU"  ver regra de Produção (B) Boleto Itau / (T) Depósito ABC - no SF virá Depósito / Boleto
                cot.U_CFS_DataEntrega = DateTime.Today.ToString("yyyy-MM-dd");


                if (cot.DocRate == 0)
                {
                    await _logger.Logger(new LogIntegration
                    {
                        LogTypeCode = 1,
                        Message = $"Erro no método _populateCotSAP: {"Taxa do Dia não preenchida"}",
                        Owner = "OrdersService",
                        Method = "_populateCotSAP",
                        Key = cotacao.Id,
                        Key2 = cotacao.Id,
                        Company = basePais
                    });
                    return null;
                }

                //LINHAS
                var documentLines = new List<Integracao.SAP.SalesForce.Services.Models.OrdersDTO.DocumentLine>();

                line = 0;
                foreach (var item in cotacao.OrderItems.records)
                {
                    string cc1 = "2001";// await BuscaCC1_FromCot(basePais, cotacao.QuoteId);// cotacao.Email);
                    string cc2 = await BuscaCC2_FromCot(basePais, cotacao.QuoteId);
                    string cc3 = await BuscaCC3_FromCot(basePais, cotacao.QuoteId);

                    string codProd = await BuscaProdutoByQuotation(cotacao.QuoteId, basePais, line);

                    documentLines.Add(new Integracao.SAP.SalesForce.Services.Models.OrdersDTO.DocumentLine
                    {
                        ItemCode = codProd, //"1.01.001.03",//item.ProdutoCodSAP__c,
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
                        //DiscountPercent = item.Discount == null ? 0 : item.Discount,
                        Usage = Convert.ToInt32(cotacao.Type),
                        U_k33p_SFID = item.Id,
                        U_k33p_SFUPrice = item.UnitPrice,
                        U_LG_xPed = cotacao.PoNumber,
                        BaseType = 23,
                        BaseEntry = await BuscaDocEntryCot(basePais, cotacao.QuoteId),
                        BaseLine = await BuscaBaseLine(basePais, cotacao.QuoteId, item.QuoteLineItemId),
                        SupplierCatNum = await BuscaCatNum(basePais, cot.CardCode, codProd)

                    });
                    line++;
                }


                cot.DocumentLines = documentLines;

                Models.OrdersDTO.TaxExtension tax = new Models.OrdersDTO.TaxExtension();
                tax.Incoterms = cotacao.Frete__c;
                cot.TaxExtension = tax;


                return cot;

            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _populateCotSAP: {ex.Message}",
                    Owner = "Orderservice",
                    Method = "_populateCotSAP",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaProdutoByQuotation(string codCot, string basePais, int line)
        {

            try
            {
                string ret = "";
                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetProdutoByQuotation");
                query = string.Format(query, basePais, codCot, line.ToString());

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        return sql.GetValue(0).ToString();
                    }
                }
                else
                {
                    return null;
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
                    Message = $"Erro no método BuscaeEmailFilial: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "BuscaEmailFilial",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }



        private async Task<List<string>> BuscaEmailFilialByQuotation(string codCot, string basePais)
        {

            try
            {
                List<string> ret = new List<string>();
                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetEmailFilialByQuotation");
                query = string.Format(query, basePais, codCot);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {

                        ret.Add(sql.GetValue(0).ToString());
                        ret.Add(sql.GetValue(1).ToString());

                    }
                }
                else
                {
                    ret.Add("Erro:Cotação não encontrada pro ID: " + codCot);
                    return ret;
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
                    Message = $"Erro no método BuscaeEmailFilial: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "BuscaEmailFilial",
                    Key = "",
                    Key2 = "",
                    Company = ""
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
                    Owner = "OrdersService",
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
                    Owner = "OrdersService",
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
                    Owner = "OrdersService",
                    Method = "BuscaVendedor",
                    Key = "",
                    Key2 = "",
                    Company = basePais
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
                    Owner = "OrdersService",
                    Method = "BuscaVendedor",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

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
                    Owner = "OrdersService",
                    Method = "BuscaDep",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaDataVencCot(string basePais, string id)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaDataVencCot");
                query = string.Format(query, basePais, id);

                string ret = await _sqlAdapter.QueryReaderString(query);

                var data = Convert.ToDateTime(ret).ToString("yyyy-MM-dd");

                return data.ToString();


            }
            catch (Exception ex)
            {
                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método BuscaCC2: {ex.Message}",
                    Owner = "OrdersService",
                    Method = "BuscaCC1",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<string> BuscaCC1_FromCot(string basePais, string email)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC1_FromCot");
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
                    Owner = "OrdersService",
                    Method = "BuscaCC1",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<string> BuscaCC2_FromCot(string basePais, string cardCode)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC2_FromCot");
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
                    Owner = "OrdersService",
                    Method = "BuscaCC1",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }
        private async Task<string> BuscaCC3_FromCot(string basePais, string cardCode)
        {
            try
            {
                string data = DateTime.Today.ToString("yyyyMMdd");
                var query = SQLSupport.GetConsultas("GetCC3_FromCot");
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
                    Owner = "OrdersService",
                    Method = "BuscaCC3",
                    Key = "",
                    Key2 = "",
                    Company = basePais
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }


        private async Task<int> BuscaDocEntryCot(string basePais, string id)
        {
            try
            {
                
                var query = SQLSupport.GetConsultas("BuscaDocEntryCot");
                query = string.Format(query, basePais, id);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return Convert.ToInt32(ret);


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

        private async Task<int> BuscaBaseLine(string basePais, string id,string lineId)
        {
            try
            {

                var query = SQLSupport.GetConsultas("BuscaBaseLine");
                query = string.Format(query, basePais, id,lineId);

                string ret = await _sqlAdapter.QueryReaderString(query);


                return Convert.ToInt32(ret);


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
                    Owner = "OrdersService",
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
