using AutoMapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Services.Interfaces;
using Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch;
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
using Record = Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch.Record;
using Attributes = Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch.Attributes;
using System.Text.Json;
using Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2;
using Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch;
using Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2;
using Integracao.SAP.SalesForce.Services.Models.SF.Order_OrderItem;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class OrdersRetornoSF : IOrdersRetornoSF
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

        public OrdersRetornoSF(ISqlAdapter sqlAdapter,
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

                var cots = await _getOrdersPendent();//select join 3 bases              


                //////agrupa por Pais - campo BillingCountry
                //var listGrouped = cots.GroupBy(x => x.BillingCountry).Select(grp => grp.ToList()).ToList();
                //foreach (var grouped in listGrouped)
                //{
                //    lista = new List<Record>();
                //    lista = grouped;
                var cotsLin = await _getOrdersPendent2();//select join 3 bases      

                if (cots != null)
                    await _processOrders(cots);

                if (cots != null)
                    await _processOrders2(cotsLin, cots[0]);

                //}

                return true;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método ProcessAsync: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_processOrders",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _PatchUnitOrder2(OrderPatch2 rec)
        {

            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            //string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/composite/sobjects/OrderItem/Id", apiVersion);
            List<Record> allPartners = new List<Record>();
            Token token = new Token();
            string tokenDB = "";
            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            List<ReturnAccountPatch> response = null;
            try
            {

                //pega filtro data
                //var query = SQLSupport.GetConsultas("GetLastModificationDateCot");
                //query = string.Format(query, nomeBaseBR);
                //var result = await _sqlAdapter.QueryReaderString(query);
                //data = String.IsNullOrEmpty(result) ? data : result;

                endpointPN = String.Format("/services/data/{0}/composite/sobjects/OrderItem/Id", apiVersion);

                //validaToken
                var query = SQLSupport.GetConsultas("GetTokenBD2");
                query = string.Format(query, nomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);



                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    response = await _httpAdapter.Call<List<ReturnAccountPatch>>(HttpMethod.Patch, endpointPN, rec, _sfHttp.Uri, token.access_token);

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

                        token.access_token = "OK";
                    }
                }

                //if (response != null)
                //{
                //    //atualiza flag data
                //    string data = DateTime.Now.AddHours(3).AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ");
                //    var query = SQLSupport.GetConsultas("SetLastModificationDateCot");
                //    query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, cot.Id);
                //    var result = await _sqlAdapter.QueryReaderString(query);

                //}

                return response;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _PatchUnitOrder: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_PatchUnitOrder",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processOrders2(List<OrderPatch2> cots2, OrderPatch cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots2)
                {
                    if (cots.records[0].BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cots.records[0].BillingCountry.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cots.records[0].BillingCountry.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;

                    //    cont++;
                    //    string query = "";
                    //    List<string> result = null;


                    //query = SQLSupport.GetConsultas("VerificaSeExisteCotByIDSF");
                    //query = string.Format(query, basePais, cot.Id);
                    //result = await _sqlAdapter.QueryListString(query);



                    //if (result.Count > 0)
                    var ret = await _PatchUnitOrder2(cot);



                    if (ret != null)
                    {

                        //delete draft                    
                        //var responseDraft4 = await _serviceLayerAdapter.Call<QuotationsDTO>(
                        //          $"Drafts({cot.records[0].numeroDraft.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.records[0].pais);

                        //atualiza flag cot
                        var query = SQLSupport.GetConsultas("AtualizaFlagPed");
                        query = string.Format(query, basePais, cots.records[0].NumeroPedidoSAP__c.ToString());
                        var result = await _sqlAdapter.QueryReaderString(query);

                        string msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id;
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Envio de retorno Linhas pro SalesForce: {ret[0].id} - {msg}",
                            Owner = "OrdersRetornoSF",
                            Method = "_processOrders2",
                            Key = ret[0].id,
                            Key2 = ret[0].id,
                            Company = "",
                            RequestObject = JsonSerializer.Serialize<OrderPatch2>(cot),
                            ResponseObject = JsonSerializer.Serialize<List<ReturnAccountPatch>>(ret)
                        });
                    }
                }

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _processOrders: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_processOrders",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<OrderPatch>> _getOrdersPendent()
        {

            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            string nomeBaseAR = _configuration.Value.SalesForceBusiness.NomeBaseAR;
            string nomeBaseCH = _configuration.Value.SalesForceBusiness.NomeBaseCH;

            try
            {



                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetPedRetorno");
                query = string.Format(query, nomeBaseBR, nomeBaseAR, nomeBaseCH);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);
                Attributes atrib = new Attributes();
                atrib.type = "Order";
                OrderPatch records = null;
                List<OrderPatch> patches = new List<OrderPatch>();

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {
                        List<Record> listaImp = new List<Record>();
                        records = new OrderPatch();
                        Record ci = new Record();
                        ci.attributes = atrib;

                        ci.Id = sql.GetValue(0).ToString();                      
                        ci.Status = sql.GetValue(2).ToString();
                        ci.NumeroPedidoSAP__c = sql.GetValue(4).ToString();
                        ci.DataPedidoSAP__c = Convert.ToDateTime(sql.GetValue(5).ToString()).ToString("yyyy-MM-dd");
                        ci.Valor_Total_com_Impostos__c = Convert.ToDouble(sql.GetValue(7).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.BillingCountry = sql.GetValue(8).ToString();
                        ci.TaxaR__c = Convert.ToDouble(sql.GetValue(9).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.COFINS__c = Convert.ToDouble(sql.GetValue(10).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.Valor_Cofins__c = Convert.ToDouble(sql.GetValue(11).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.ICMS__c = Convert.ToDouble(sql.GetValue(12).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.Valor_ICMS__c = Convert.ToDouble(sql.GetValue(13).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.IPI__c = Convert.ToDouble(sql.GetValue(14).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        //ci.ValorIPI__c = Convert.ToDouble(sql.GetValue(15).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.PIS__c = Convert.ToDouble(sql.GetValue(16).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.Valor_PIS__c = Convert.ToDouble(sql.GetValue(17).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.numeroDraft = sql.GetValue(18).ToString();
                        ci.pais = sql.GetValue(19).ToString();
                        ci.Filial_Faturamento__c = sql.GetValue(20).ToString();
                        ci.DataPedidoIntegrado__c = DateTime.Today.ToString("yyyy-MM-dd");
                        ci.Transportadora__c = sql.GetValue(21).ToString(); 
                        ci.CondicaoPagamento__c = sql.GetValue(22).ToString();


                        listaImp.Add(ci);
                        records.records = listaImp;
                        patches.Add(records);
                        //i++;
                    }


                }
                else
                {
                    return null;
                }

                sql.Close();
                conexao.Close();


                return patches;

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _getOrdersPendent: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_getOrdersPendent",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<OrderPatch2>> _getOrdersPendent2()
        {

            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            string nomeBaseAR = _configuration.Value.SalesForceBusiness.NomeBaseAR;
            string nomeBaseCH = _configuration.Value.SalesForceBusiness.NomeBaseCH;

            try
            {



                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetPedRetorno2");
                query = string.Format(query, nomeBaseBR, nomeBaseAR, nomeBaseCH);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);
                Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Attributes atrib = new Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Attributes();
                atrib.type = "OrderItem";
                OrderPatch2 records = null;
                List<OrderPatch2> patches = new List<OrderPatch2>();

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {
                        List<Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Record> listaImp = new List<Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Record>();
                        records = new OrderPatch2();
                        Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Record ci = new Integracao.SAP.SalesForce.Services.Models.SF.OrderPatch2.Record();
                        ci.attributes = atrib;

                        ci.Id = sql.GetValue(0).ToString();
                        ci.ProdutoCodSAP__c = sql.GetValue(1).ToString();

                        listaImp.Add(ci);
                        records.records = listaImp;
                        patches.Add(records);
                        //i++;
                    }


                }
                else
                {
                    return null;
                }

                sql.Close();
                conexao.Close();


                return patches;

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _getOrdersPendent2: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_getOrdersPendent2",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processOrders(List<OrderPatch> cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots)
                {
                    if (cot.records[0].BillingCountry.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cot.records[0].BillingCountry.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cot.records[0].BillingCountry.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;
                  
                    var ret = await _PatchUnitOrder(cot);

                    if (ret != null)
                    {

                        //delete draft                    
                        //var responseDraft4 = await _serviceLayerAdapter.Call<OrdersDTO>(
                        //          $"Drafts({cot.records[0].numeroDraft.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.records[0].pais);

                        //atualiza flag cot
                        //var query = SQLSupport.GetConsultas("AtualizaFlagPed");
                        //query = string.Format(query, basePais, cot.records[0].NumeroPedidoSAP__c.ToString());
                        //var result = await _sqlAdapter.QueryReaderString(query);

                        string msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id;
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Envio de retorno pro SalesForce: {ret[0].id} - {msg}",
                            Owner = "OrdersRetornoSF",
                            Method = "_processOrders",
                            Key = ret[0].id,
                            Key2 = ret[0].id,
                            Company = "",
                            RequestObject = JsonSerializer.Serialize<OrderPatch>(cot),
                            ResponseObject = JsonSerializer.Serialize<List<ReturnAccountPatch>>(ret)
                        });
                    }
                }

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _processOrders: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_processOrders",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _PatchUnitOrder(OrderPatch rec)
        {

            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            //string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/composite/sobjects/Order/Id", apiVersion);
            List<Record> allPartners = new List<Record>();
            Token token = new Token();
            string tokenDB = "";
            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            List<ReturnAccountPatch> response = null;
            try
            {
            

                endpointPN = String.Format("/services/data/{0}/composite/sobjects/Order/Id", apiVersion);

                //validaToken
                var query = SQLSupport.GetConsultas("GetTokenBD2");
                query = string.Format(query, nomeBaseBR);
                var result = await _sqlAdapter.QueryReaderString(query);



                while (token.access_token != "OK")
                {
                    if (!String.IsNullOrEmpty(result))
                        token.access_token = result;
                    if (token.access_token != null)
                        token.access_token = token.access_token;

                    response = await _httpAdapter.Call<List<ReturnAccountPatch>>(HttpMethod.Patch, endpointPN, rec, _sfHttp.Uri, token.access_token);

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

                        token.access_token = "OK";
                    }
                }

                //if (response != null)
                //{
                //    //atualiza flag data
                //    string data = DateTime.Now.AddHours(3).AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ");
                //    var query = SQLSupport.GetConsultas("SetLastModificationDateCot");
                //    query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, cot.Id);
                //    var result = await _sqlAdapter.QueryReaderString(query);

                //}

                return response;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _PatchUnitOrder: {ex.Message}",
                    Owner = "OrdersRetornoSF",
                    Method = "_PatchUnitOrder",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

    }
}
