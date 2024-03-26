using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Core.Interfaces
{
    public interface IHttpAdapter
    {
        Task<T> Call<T>(
            HttpMethod method,
            string endPoint,
            object obj = null,
            string uri = null,string token =null) where T : class;

        Task<T> CallLogin<T>(
          HttpMethod method,
          string endPoint,
          object obj = null,
          string uri = null) where T : class;
        Task<RestResponse<T>> ExecuteRequestAsync<T>(RestClient client, RestRequest request);


        Task<T> CallProduct<T>(
           HttpMethod method,
           string endPoint,
           object obj = null,
           string uri = null) where T : class;

    }
}
