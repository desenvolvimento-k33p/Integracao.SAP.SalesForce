using AutoMapper;
using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Integracao.SAP.SalesForce.Domain.Logger;
using Integracao.SAP.SalesForce.Infra.Interfaces;
using Integracao.SAP.SalesForce.Services.Interfaces;
using Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch;
using Integracao.SAP.SalesForce.Services.Models.SF;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.SQL;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Record = Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch.Record;
using System.Data.SqlClient;
using System.Globalization;
using Integracao.SAP.SalesForce.Services.Models.SF.Quote_QuoteItens;
using Attributes = Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch.Attributes;
using System.Text.Json;
using Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2;

namespace Integracao.SAP.SalesForce.Services.Services
{
    public class QuotationsRetornoSF : IQuotationsRetornoSF
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

        public QuotationsRetornoSF(ISqlAdapter sqlAdapter,
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

                var cots = await _getQuotationsPendent();//select join 3 bases              


                //////agrupa por Pais - campo Pais_da_Venda__c
                //var listGrouped = cots.GroupBy(x => x.Pais_da_Venda__c).Select(grp => grp.ToList()).ToList();
                //foreach (var grouped in listGrouped)
                //{
                //    lista = new List<Record>();
                //    lista = grouped;


                if (cots != null)
                    await _processQuotations(cots);


                var cotsLin = await _getQuotationsPendent2();//select join 3 bases              



                if (cots != null)
                    await _processQuotations2(cotsLin, cots[0]);

                //}

                return true;
            }
            catch (Exception ex)
            {

                await _logger.Logger(new LogIntegration
                {
                    LogTypeCode = 1,
                    Message = $"Erro no método ProcessAsync: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_processQuotations",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<QuotePatch>> _getQuotationsPendent()
        {

            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            string nomeBaseAR = _configuration.Value.SalesForceBusiness.NomeBaseAR;
            string nomeBaseCH = _configuration.Value.SalesForceBusiness.NomeBaseCH;

            try
            {



                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetCotacoesRetorno");
                query = string.Format(query, nomeBaseBR, nomeBaseAR, nomeBaseCH);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);
                Attributes atrib = new Attributes();
                atrib.type = "Quote";
                QuotePatch records = null;
                List<QuotePatch> patches = new List<QuotePatch>();

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {
                        List<Record> listaImp = new List<Record>();
                        records = new QuotePatch();
                        Record ci = new Record();
                        ci.attributes = atrib;

                        ci.Id = sql.GetValue(0).ToString();
                        ci.Name = sql.GetValue(1).ToString();
                        ci.Status = sql.GetValue(2).ToString();
                        ci.ExpirationDate = Convert.ToDateTime(sql.GetValue(3)).ToString("yyyy-MM-dd");
                        ci.NumeroCotacaoSAP__c = sql.GetValue(4).ToString();
                        ci.DataCotacaoSAP__c = Convert.ToDateTime(sql.GetValue(5)).ToString("yyyy-MM-dd");
                        ci.Tax = Convert.ToDouble(sql.GetValue(6).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.Valor_Total_com_Impostos__c = Convert.ToDouble(sql.GetValue(7).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.Pais_da_Venda__c = sql.GetValue(8).ToString();
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
                        ci.Transportadora__c = sql.GetValue(20).ToString();
                        ci.IVA__c = Convert.ToDouble(sql.GetValue(22).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        ci.IVAV__c = Convert.ToDouble(sql.GetValue(23).ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                       

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
                    Message = $"Erro no método _getQuotationsPendent: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_getQuotationsPendent",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<QuotePatch2>> _getQuotationsPendent2()
        {

            string nomeBaseBR = _configuration.Value.SalesForceBusiness.NomeBaseBR;
            string nomeBaseAR = _configuration.Value.SalesForceBusiness.NomeBaseAR;
            string nomeBaseCH = _configuration.Value.SalesForceBusiness.NomeBaseCH;

            try
            {



                Domain.Configuration.SqlConnection cfgFile = _configuration.Value.SqlConnection;
                var _urlConnection = $"Data Source={cfgFile.DataSource};User ID={cfgFile.UserID};Password={cfgFile.Password};Initial Catalog={cfgFile.InitialCatalog}";

                var query = SQLSupport.GetConsultas("GetCotacoesRetorno2");
                query = string.Format(query, nomeBaseBR, nomeBaseAR, nomeBaseCH);

                System.Data.SqlClient.SqlConnection conexao = new System.Data.SqlClient.SqlConnection(_urlConnection);
                SqlDataReader sql = await _sqlAdapter.QuerySqlDataReader(query, conexao);
                Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Attributes atrib = new Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Attributes();
                atrib.type = "QuoteLineItem";
                QuotePatch2 records = null;
                List<QuotePatch2> patches = new List<QuotePatch2>();

                if (sql.HasRows)
                {
                    while (sql.Read())
                    {
                        List<Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Record> listaImp = new List<Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Record>();
                        records = new QuotePatch2();
                        Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Record ci = new Integracao.SAP.SalesForce.Services.Models.SF.QuotePatch2.Record();
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
                    Message = $"Erro no método _getQuotationsPendent2: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_getQuotationsPendent2",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processQuotations(List<QuotePatch> cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots)
                {
                    if (cot.records[0].Pais_da_Venda__c.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cot.records[0].Pais_da_Venda__c.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cot.records[0].Pais_da_Venda__c.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;

                    //    cont++;
                    //    string query = "";
                    //    List<string> result = null;


                    //query = SQLSupport.GetConsultas("VerificaSeExisteCotByIDSF");
                    //query = string.Format(query, basePais, cot.Id);
                    //result = await _sqlAdapter.QueryListString(query);



                    //if (result.Count > 0)
                    var ret = await _PatchUnitQuotation(cot);



                    if (ret != null)
                    {

                        //delete draft                    
                        //var responseDraft4 = await _serviceLayerAdapter.Call<QuotationsDTO>(
                        //          $"Drafts({cot.records[0].numeroDraft.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.records[0].pais);

                        //atualiza flag cot
                        //var query = SQLSupport.GetConsultas("AtualizaFlagCot");
                        //query = string.Format(query, basePais, cot.records[0].NumeroCotacaoSAP__c.ToString());
                        //var result = await _sqlAdapter.QueryReaderString(query);

                        string msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id;
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Envio de retorno pro SalesForce: {ret[0].id} - {msg}",
                            Owner = "QuotationsRetornoSF",
                            Method = "_processQuotations",
                            Key = ret[0].id,
                            Key2 = ret[0].id,
                            Company = "",
                            RequestObject = JsonSerializer.Serialize<QuotePatch>(cot),
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
                    Message = $"Erro no método _processQuotations: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_processQuotations",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task _processQuotations2(List<QuotePatch2> cots2, QuotePatch cots)
        {

            int cont = 0;
            string basePais = "";

            try
            {
                foreach (var cot in cots2)
                {
                    if (cots.records[0].Pais_da_Venda__c.ToString().ToUpper().Equals("BRASIL"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseBR;
                    if (cots.records[0].Pais_da_Venda__c.Equals("Argentina"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseAR;
                    if (cots.records[0].Pais_da_Venda__c.Equals("Chile"))
                        basePais = _configuration.Value.SalesForceBusiness.NomeBaseCH;

                    //    cont++;
                    //    string query = "";
                    //    List<string> result = null;


                    //query = SQLSupport.GetConsultas("VerificaSeExisteCotByIDSF");
                    //query = string.Format(query, basePais, cot.Id);
                    //result = await _sqlAdapter.QueryListString(query);



                    //if (result.Count > 0)
                    var ret = await _PatchUnitQuotation2(cot);



                    if (ret != null)
                    {

                        //delete draft                    
                        //var responseDraft4 = await _serviceLayerAdapter.Call<QuotationsDTO>(
                        //          $"Drafts({cot.records[0].numeroDraft.ToString()})", HttpMethod.Delete, null, _serviceLayerHttp.Uri, null, cot.records[0].pais);

                        //atualiza flag cot
                        var query = SQLSupport.GetConsultas("AtualizaFlagCot");
                        query = string.Format(query, basePais, cots.records[0].NumeroCotacaoSAP__c.ToString());
                        var result = await _sqlAdapter.QueryReaderString(query);

                        string msg = ret[0].errors.Count > 0 ? ret[0].errors[0].ToString() : "Sucesso no retorno: ID: " + ret[0].id;
                        await _logger.Logger(new LogIntegration
                        {
                            LogTypeCode = 1,
                            Message = $"Envio de retorno Linhas pro SalesForce: {ret[0].id} - {msg}",
                            Owner = "QuotationsRetornoSF",
                            Method = "_processQuotations2",
                            Key = ret[0].id,
                            Key2 = ret[0].id,
                            Company = "",
                            RequestObject = JsonSerializer.Serialize<QuotePatch2>(cot),
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
                    Message = $"Erro no método _processQuotations: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_processQuotations",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _PatchUnitQuotation(QuotePatch rec)
        {

            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            //string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/composite/sobjects/Quote/Id", apiVersion);
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

                endpointPN = String.Format("/services/data/{0}/composite/sobjects/Quote/Id", apiVersion);

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
                    Message = $"Erro no método _PatchUnitQuotation: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_PatchUnitQuotation",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

        private async Task<List<ReturnAccountPatch>> _PatchUnitQuotation2(QuotePatch2 rec)
        {

            string apiVersion = _configuration.Value.SalesForceHttp.VersaoAPI;//v55.0
            //string data = _configuration.Value.SalesForceHttp.LastModifiedDate_Cotacoes;// "2022-12-01T10:36:40Z";
            var endpointToken = $"/services/oauth2/token";
            var endpointPN = String.Format("/services/data/{0}/composite/sobjects/QuoteLineItem/Id", apiVersion);
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

                endpointPN = String.Format("/services/data/{0}/composite/sobjects/QuoteLineItem/Id", apiVersion);

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
                    Message = $"Erro no método _PatchUnitQuotation: {ex.Message}",
                    Owner = "QuotationsRetornoSF",
                    Method = "_PatchUnitQuotation",
                    Key = "",
                    Key2 = "",
                    Company = ""
                });
                throw new Exception($"Erro ao realizar processo {ex.Message}", ex);
            }
        }

    }
}
