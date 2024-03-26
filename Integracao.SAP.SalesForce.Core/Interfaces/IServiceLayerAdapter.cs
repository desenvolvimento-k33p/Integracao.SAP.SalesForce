using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Core.Interfaces
{
    public interface IServiceLayerAdapter
    {
        Task<T> Call<T>(string endPoint, HttpMethod method, object obj, string uri = null, string sessionId = null,string nomePais = null) where T : class;
        Task<CookieContainer> Login(string nomePais);
    }
}
