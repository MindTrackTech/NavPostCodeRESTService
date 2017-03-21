using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NAVPostCodeRESTService.Entities
{
    public class Address
    {
        Entity _address = null;

        public Address(IOrganizationService svc, Entity address)
        {
            _address = address;
        }

        public string street1
        {
            get { return this._address.GetAttributeValue<string>("new_street1"); }
        }

        public string street2
        {
            get { return this._address.GetAttributeValue<string>("new_street2"); }
        }

        public string street3
        {
            get { return this._address.GetAttributeValue<string>("new_street3"); }
        }

        public string AddressName
        {
            get { return this._address.GetAttributeValue<string>("new_name"); }
        }

        public string City
        {
            get
            {
                if (this._address.Contains("new_city"))
                    return this._address.GetAttributeValue<EntityReference>("new_city").Name;
                else
                    return null;
            }
        }

        public string County
        {
            get
            {
                if (this._address.Contains("new_county"))
                    return this._address.GetAttributeValue<EntityReference>("new_county").Name;
                else
                    return null;
            }
        }
        public string Country
        {
            get 
            {
                if (this._address.Contains("new_country"))
                    return this._address.GetAttributeValue<EntityReference>("new_country").Name;
                else
                    return null;
            }
        }
        public string PostalCode
        {
            get {
                if (this._address.Contains("new_postalcode"))
                    return this._address.GetAttributeValue<EntityReference>("new_postalcode").Name;
                else
                    return null;
            }
        }
        public string BuildingDetails
        {
            get { return this._address.GetAttributeValue<string>("new_addressname"); }
        }
        public string AddressIdentifier
        {
            get { return this._address.GetAttributeValue<string>("new_identifier"); }
        }
        public string Number
        {
            get { return this._address.GetAttributeValue<string>("new_addressnumbertext"); }
        }

        public int AddressOrigin
        {
            get { return this._address.GetAttributeValue<OptionSetValue>("new_addressorigin").Value; }
        }

        public AddressDetails RetrieveAddress()
        {
            var addressDetails = new AddressDetails();
            addressDetails.AddressIdentifier = this.AddressIdentifier;
            addressDetails.AddressName = this.AddressName;
            addressDetails.BuildingDetails = this.BuildingDetails;
            addressDetails.City = this.City;
            addressDetails.Country = this.Country;
            addressDetails.County = this.County;
            addressDetails.Number = this.Number;
            addressDetails.PostalCode = this.PostalCode;
            addressDetails.street1 = this.street1;
            addressDetails.street2 = this.street2;
            addressDetails.street3 = this.street3;

            return addressDetails;
        }

    }
}