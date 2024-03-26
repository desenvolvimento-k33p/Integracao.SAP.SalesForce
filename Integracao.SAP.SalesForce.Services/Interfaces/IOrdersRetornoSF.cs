using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Interfaces
{
    
    public interface IOrdersRetornoSF
    {
        Task<bool> ProcessAsync();
    }
}
