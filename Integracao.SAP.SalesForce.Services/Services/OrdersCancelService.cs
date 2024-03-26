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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Record = Integracao.SAP.SalesForce.Services.Models.SF.Order_OrderItem.Record;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class OrdersCancelService : IOrdersCancelService
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

        public OrdersCancelService(ISqlAdapter sqlAdapter,
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

                cots = cots.Where(n => n.Status == "Cancelado").ToList();

                ////atualiza data anterior               
                //var query = SQLSupport.GetConsultas("SetDataAnteriorCot");
                //query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR);
                //var result = await _sqlAdapter.QueryReaderString(query);



                ////agrupa por Pais - campo BillingCountry
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
                    Owner = "OrdersCancelService",
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
            var endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);
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

                endpointPN = String.Format("/services/data/{0}/query/?q=SELECT+FIELDS(ALL),(SELECT+FIELDS(ALL)+FROM+OrderItems+LIMIT+200)+FROM+Order+WHERE+LastModifiedDate>={1}+LIMIT+200", apiVersion, data);

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
                    Owner = "OrdersCancelService",
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



                    if (result.Count > 0)
                        await _CancelaUnitQuotation(cot, basePais, result[0]);


                }

            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método _processOrders: {ex.Message}",
                    Owner = "OrdersCancelService",
                    Method = "_processOrders",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<dynamic> _CancelaUnitQuotation(Record cot, string basePais, string docEntry)
        {
            try
            {

                var response = await _serviceLayerAdapter.Call<QuotationsDTO>(
                          $"Orders({docEntry})/Cancel", HttpMethod.Post, null, _serviceLayerHttp.Uri, null, cot.BillingCountry);


                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Inserção de Cot: {docEntry} ",
                    Owner = "OrdersCancelService",
                    Method = "_CancelaUnitQuotation",
                    Key = docEntry.ToString(),
                    Key2 = cot.Id,
                    Company = basePais,
                    RequestObject = JsonSerializer.Serialize<Record>(cot),
                    ResponseObject = JsonSerializer.Serialize<QuotationsDTO>(response)
                });
                //if (response != null)
                //{
                //    //atualiza flag data
                //    string data = DateTime.Now.AddHours(3).AddSeconds(10).ToString("yyyy-MM-ddTHH:mm:ssZ");
                //    var query = SQLSupport.GetConsultas("SetLastModificationDateCot");
                //    query = string.Format(query, _configuration.Value.SalesForceBusiness.NomeBaseBR, data, cot.Id);
                //    var result = await _sqlAdapter.QueryReaderString(query);

                //}

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
    }
}
