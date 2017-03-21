using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace NAVPostCodeRESTService.Entities
{
    public class Company
    {
        Entity _company = null;

        public Company(IOrganizationService svc, Entity company)
        {
            _company = company;
        }

        public string Name
        {
            get { return this._company.GetAttributeValue<string>("name"); }
        }

        public string MainContactName
        {
            get 
            {
                if (this._company.Contains("primarycontactid"))
                    return this._company.GetAttributeValue<EntityReference>("primarycontactid").Name;
                else
                    return null;
            }
        }

        public string MainPhone
        {
            get { return this._company.GetAttributeValue<string>("telephone1"); }
        }

        public string MobilePhone
        {
            get { return this._company.GetAttributeValue<string>("telephone2"); }
        }

        public string CurrencyCode
        {
            get
            {
                if (this._company.Contains("TransactionCurrencyId"))
                    return this._company.GetAttributeValue<EntityReference>("TransactionCurrencyId").Name;
                else
                    return null;
            }
        }

        public string Email
        {
            get { return this._company.GetAttributeValue<string>("emailaddress1"); }
        }

        public string HomePage
        {
            get { return this._company.GetAttributeValue<string>("websiteurl"); }
        }

        public string CompanyCode
        {
            get { return this._company.GetAttributeValue<string>("new_brokercode"); }
        }

        public string NavCode
        {
            get { return this._company.GetAttributeValue<string>("new_navcompanycode"); }
        }

        //Do we need this?
        public bool CompareValues(BrokerDetails broker)
        {
            return false;
        }

        public int StateCode
        {
            get { return this._company.GetAttributeValue<OptionSetValue>("statecode").Value; }
        }
    }
}