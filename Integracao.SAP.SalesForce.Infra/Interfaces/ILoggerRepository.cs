using Integracao.SAP.SalesForce.Domain.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Infra.Interfaces
{
    public interface ILoggerRepository
    {
        Task Logger(LogIntegration logIntegration);
    }
}
