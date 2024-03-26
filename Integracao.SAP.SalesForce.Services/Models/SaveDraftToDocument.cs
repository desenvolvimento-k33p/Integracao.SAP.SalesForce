using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integracao.SAP.SalesForce.Services.Models
{
   

    public class Document
    {
        public string DocDueDate { get; set; }
        public string DocEntry { get; set; }
    }

    public class SaveDraftToDocument
    {
        public Document Document { get; set; }
    }

}
