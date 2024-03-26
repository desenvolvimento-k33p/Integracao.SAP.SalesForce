using AutoMapper;
using Integracao.SAP.SalesForce.Services.Models;
using Integracao.SAP.SalesForce.Services.Models.SF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Integracao.SAP.SalesForce.Services.Mapper.Profiles.Business_Partner
{
    public class CreateBusinessPartnerProfile : Profile
    {
        public CreateBusinessPartnerProfile()
        {
            //CreateMap<Record, BusinessPartnersDTO>()
                //.ForMember(dst => dst.CardName, org => org.MapFrom(a => String.IsNullOrEmpty(a.Name) ? "" : a.Name))
                //.ForMember(dst => dst.CardForeignName, org => org.MapFrom(a => String.IsNullOrEmpty(a.NomeFantasia__c) ? "" : a.NomeFantasia__c));
              
        }
    }
}
