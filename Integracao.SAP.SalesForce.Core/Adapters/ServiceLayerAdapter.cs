using Integracao.SAP.SalesForce.Core.Interfaces;
using Integracao.SAP.SalesForce.Core.Models.Response;
using Integracao.SAP.SalesForce.Core.Models;
using Integracao.SAP.SalesForce.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Core.Adapters
{
    public class ServiceLayerAdapter : IServiceLayerAdapter
    {
        private readonly string _cookie = "B1SESSION";
        private readonly string _type = "application/json";
        private readonly string _header = "Content-Type";
        private readonly string _baseUrl = "/b1s/v1";
        private readonly ILogger<ServiceLayerAdapter> _logger;
        private IOptions<Configuration> _configurations;
        private CookieContainer _httpCookieContainer = null;

        public ServiceLayerAdapter(ILogger<ServiceLayerAdapter> logger, IOptions<Configuration> configurations)
        {
            _logger = logger;
            _configurations = configurations;
        }

        public async Task<T> Call<T>(string endPoint, HttpMethod method, object obj = null, string uri = null, string sessionId = null,string nomePais = null) where T : class
        {
            var cookieContainer = await Login(nomePais);

            if (cookieContainer == null)
                throw new Exception("Não foi possível efetuar login");

            var c = cookieContainer.GetAllCookies();

            var indexPrimeiroCaracter = endPoint.IndexOf("/");
            if (indexPrimeiroCaracter == 0)
                endPoint = endPoint.Remove(0, 1);

            var baseUrl = new Uri($"{uri}{_baseUrl}/");

            var clientHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                CookieContainer = _httpCookieContainer
            };

            using (clientHandler)
            {
                using (var client = new HttpClient(clientHandler))
                {
                    client.BaseAddress = baseUrl;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var json = JsonSerializer.Serialize(obj);

                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        using (var httpRequestMensage = new HttpRequestMessage(method, endPoint))
                        {
                            if (obj != null)
                                httpRequestMensage.Content = stringContent;

                            var response = await client.SendAsync(httpRequestMensage);
                            var statusSuccess = $"{HttpStatusCode.Created}|{HttpStatusCode.OK}";

                            if (!statusSuccess.Contains(response.StatusCode.ToString()))
                                _logger.LogError($"responseStatus={response.StatusCode} - [{method}] - [{baseUrl.AbsoluteUri}Login] payload={json}");
                            else
                                _logger.LogInformation($"responseStatus={response.StatusCode} - [{method}] - [{baseUrl.AbsoluteUri}Login] payload={json}");

                            var content = await response.Content.ReadAsStringAsync();

                            var retZerado = default(T);

                            if (string.IsNullOrEmpty(content))
                                return retZerado;

                            var contentToObject = JsonSerializer.Deserialize<T>(content);
                            return contentToObject;
                        }
                    }
                }
            }
        }

        public async Task<CookieContainer> Login(string pais)
        {
            try
            {

                var serviceLayerConfiguration = _configurations.Value.ServiceLayer;

                string company = "";
                string password = "";
                string userName = "";

                if (pais == "Brasil" || (pais != "Argentina"  && pais !="Chile"))
                {
                    company = serviceLayerConfiguration.CompanyDB_BR;
                    password = serviceLayerConfiguration.Password_BR;
                    userName = serviceLayerConfiguration.Username_BR;
                }
                if (pais == "Argentina")
                {
                    company = serviceLayerConfiguration.CompanyDB_AR;
                    password = serviceLayerConfiguration.Password_AR;
                    userName = serviceLayerConfiguration.Username_AR;
                }

                if (pais == "Chile")
                {
                    company = serviceLayerConfiguration.CompanyDB_CH;
                    password = serviceLayerConfiguration.Password_CH;
                    userName = serviceLayerConfiguration.Username_CH;
                }


                var baseUrl = new Uri($"{serviceLayerConfiguration.Uri}{_baseUrl}/");
                var login = new LoginPost()
                {
                    CompanyDB = company,
                    Password = password,
                    UserName = userName
                };

                if (_httpCookieContainer == null)
                    _httpCookieContainer = new CookieContainer();

                var clientHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                    CookieContainer = _httpCookieContainer
                };

                var client = new HttpClient(clientHandler);
                var cookieContainer = _httpCookieContainer.GetCookies(baseUrl);
                var dataString = string.Empty;
                DateTime? dataValidaSessao = null;
                string nomeBase = "";


                if (cookieContainer["DataValidaSessao"] != null)
                {
                    dataString = cookieContainer["DataValidaSessao"].Value;
                    dataValidaSessao = DateTime.ParseExact(dataString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    nomeBase = cookieContainer["NomeBase"].Value;
                }

                if (dataValidaSessao.HasValue && dataValidaSessao.Value >= DateTime.Now && company == cookieContainer["NomeBase"].Value)
                    return _httpCookieContainer;

                _httpCookieContainer = new CookieContainer();

                using (clientHandler)
                {
                    using (client)
                    {
                        client.BaseAddress = baseUrl;
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        JsonSerializerOptions options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var json = JsonSerializer.Serialize(login);

                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            using (var httpRequestMensage = new HttpRequestMessage(HttpMethod.Post, "Login"))
                            {
                                httpRequestMensage.Content = stringContent;

                                var response = await client.SendAsync(httpRequestMensage);

                                if (response.StatusCode != HttpStatusCode.OK)
                                {
                                    var ret = await response.Content.ReadAsStringAsync();
                                    _logger.LogInformation($"responseStatus={response.StatusCode} - [{HttpMethod.Post}] - [{baseUrl.AbsoluteUri}Login] payload={json}");
                                    throw new Exception($"Erro ao efetuar login: {ret}");
                                }

                                var restResponseCookies = _httpCookieContainer.GetAllCookies().ToList();

                                var content = await response.Content.ReadAsStringAsync();
                                var responseData = JsonSerializer.Deserialize<ServiceLayerResponse>(content);

                                var dataSessao = DateTime.Now.AddMinutes(responseData.SessionTimeout);
                                _httpCookieContainer.Add(baseUrl, new Cookie("DataValidaSessao", dataSessao.ToString("yyyy-MM-dd HH:mm:ss")));
                                _httpCookieContainer.Add(baseUrl, new Cookie("B1SESSION", responseData.SessionId));
                                _httpCookieContainer.Add(baseUrl, new Cookie("NomeBase", company));

                                return _httpCookieContainer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return null;
            }
        }
    }
}
