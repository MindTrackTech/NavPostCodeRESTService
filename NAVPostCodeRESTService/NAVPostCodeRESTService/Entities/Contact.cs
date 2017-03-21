using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.Xrm.Sdk.Query;
using System.Text.RegularExpressions;


namespace NAVPostCodeRESTService.Entities
{
    public class Contact
    {
        OrganizationService svc;

        enum AddressOrigin
        {
            Manual = 100000000,
            Bordereaux = 100000001,
            PostCodeSoftware = 100000002
        }

        public Contact(OrganizationService _svc)
        {
            svc = _svc;
        }

        #region RetrieveAddress
        public IEnumerable<AddressDetails> RetrieveAddress(string postalCode)
        {
            List<AddressDetails> addressDetailsList = new List<AddressDetails>();

            var normalized = Regex.Replace(postalCode.ToLowerInvariant(), @"[^a-z0-9]+", "", RegexOptions.CultureInvariant);

            var country = svc.RetrieveCustomEntityByName("United Kingdom", "new_country").FirstOrDefault();

            var postCode = svc.RetrieveOrCreatePostalCode(postalCode, country);

            var addresses = svc.RetrieveAddressForPostCode(postCode.Id);

            foreach (var address in addresses)
            {
                var addressDetails = new AddressDetails();

                var getAddressDetails = new Address(svc, address);

                var addOrigin = (AddressOrigin)getAddressDetails.AddressOrigin;

                if (addOrigin == AddressOrigin.PostCodeSoftware)
                {
                    addressDetails.AddressIdentifier = getAddressDetails.AddressIdentifier;
                    addressDetails.AddressName = getAddressDetails.AddressName;
                    addressDetails.BuildingDetails = getAddressDetails.BuildingDetails;
                    if (getAddressDetails.City != null)
                        addressDetails.City = getAddressDetails.City;
                    addressDetails.Country = getAddressDetails.Country;
                    if (getAddressDetails.County != null)
                        addressDetails.County = getAddressDetails.County;
                    addressDetails.PostalCode = getAddressDetails.PostalCode;
                    addressDetails.street1 = getAddressDetails.street1;
                    addressDetails.street2 = getAddressDetails.street2;
                    addressDetails.street3 = getAddressDetails.street3;
                    addressDetails.Number = getAddressDetails.Number;

                    addressDetailsList.Add(addressDetails);
                }
            }

            return addressDetailsList;
        }
        #endregion

        #region CreateManualAddress
        public string CreateManualAddress(AddressDetails address)
        {
            var postCode = address.PostalCode;
            var country = address.Country;

            if (country != null || country != "")
            {
                if(country != "United Kingdom")
                {
                    var retrievedCountry = svc.RetrieveCustomEntityByName(country, "new_country").FirstOrDefault();
                    var createdPostCode = svc.RetrieveOrCreatePostalCode(address.PostalCode, retrievedCountry);//CreatePostCode(address.Country, address.PostalCode);
                    var checkIfAddressExists = svc.RetrieveAddressForPostCode(createdPostCode.Id);
                    if (checkIfAddressExists == null)
                    {
                        var createdAddress = CreateAddress(address, createdPostCode, retrievedCountry);
                        return createdAddress.GetAttributeValue<string>("new_identifier");
                    }
                }
            }

            return null;
        }
        #endregion

        #region CreateAddress
        private Entity CreateAddress(AddressDetails address, Entity postCode, Entity country)
        {
            Entity city = null;
            Entity county = null;

            if(address.City != null || address.City != "")
                city = FindOrCreateCity(country.ToEntityReference(), address.City);

            if (address.County != null || address.County != "")
                county = FindOrCreateCounty(country.ToEntityReference(), address.County);

            var addrEntity = new Entity("new_address");

            addrEntity["new_addressname"] = address.BuildingDetails;
            addrEntity["new_addressnumbertext"] = address.Number;

            addrEntity["new_county"] = county.ToEntityReference();
            addrEntity["new_country"] = country.ToEntityReference();
            addrEntity["new_city"] = city.ToEntityReference();

            addrEntity["new_street1"] = address.street1;
            addrEntity["new_street2"] = address.street2;
            addrEntity["new_street3"] = address.street3;

            addrEntity["new_addressorigin"] = AddressOrigin.Manual.ToOptionSet();
            addrEntity["new_postalcode"] = postCode.ToEntityReference();

            var createdAddress = svc.Create(addrEntity);

            return svc.Retrieve("new_address", createdAddress, new ColumnSet(true));

        }
        #endregion

        #region FindOrCreateCounty
        private Entity FindOrCreateCounty(EntityReference countryRef, string countyName)
        {
            var countyQuery = new QueryExpression("new_county");

            countyQuery.Criteria.AddCondition("new_name", ConditionOperator.Equal, countyName);
            countyQuery.Criteria.AddCondition("new_country", ConditionOperator.Equal, countryRef.Id);

            var county = svc.RetrieveMultiple(countyQuery).Entities.FirstOrDefault();

            if (county != null)
                return county;

            county = new Entity("new_county");
            county["new_name"] = countyName;
            county["new_country"] = countryRef;
            county.Id = svc.Create(county);

            return county;
        }
        #endregion

        #region FindOrCreateCity
        private Entity FindOrCreateCity(EntityReference countyRef, string cityName)
        {
            var cityQuery = new QueryExpression("new_city");

            cityQuery.Criteria.AddCondition("new_name", ConditionOperator.Equal, cityName);
            cityQuery.Criteria.AddCondition("new_countyid", ConditionOperator.Equal, countyRef.Id);

            var city = svc.RetrieveMultiple(cityQuery).Entities.FirstOrDefault();

            if (city != null)
                return city;

            city = new Entity("new_city");
            city["new_name"] = cityName;
            city["new_countyid"] = countyRef;
            city.Id = svc.Create(city);

            return city;
        }
        #endregion

        public IEnumerable<BrokerDetails> MapContacts(IEnumerable<Entity> contacts, string feedType)
        {
            var details = new List<BrokerDetails>();

             foreach(var contact in contacts)
             {
                 Entity address = null;
                 var addressRef = contact.GetAttributeValue<EntityReference>("new_address");
                 if(addressRef != null)
                    address = svc.Retrieve(addressRef.LogicalName, addressRef.Id, new ColumnSet(true));
                 var fullName = contact.GetAttributeValue<string>("firstname") + " " + contact.GetAttributeValue<string>("lastname");
                 var detail = new BrokerDetails();
                 detail.Name = fullName;
                 if (contact.Contains("transactioncurrencyid"))
                     detail.CurrencyCode = contact.GetAttributeValue<EntityReference>("transactioncurrencyid").Name;
                 detail.CRMCompanyCode = contact.GetAttributeValue<string>("new_contaccode");
                 detail.Email = contact.GetAttributeValue<string>("emailaddress1");
                 detail.PhoneNumber = contact.GetAttributeValue<string>("telephone1");
                 detail.MobileNumber = contact.GetAttributeValue<string>("mobilephone");
                 if (address != null)
                 {
                     detail.Address = address.GetAttributeValue<string>("new_street1");
                     detail.Address2 = address.GetAttributeValue<string>("new_street2");
                     if (contact.Contains("new_city"))
                         detail.City = address.GetAttributeValue<EntityReference>("new_city").Name;
                     if (contact.Contains("new_country"))
                         detail.Country = address.GetAttributeValue<EntityReference>("new_country").Name;
                     if (contact.Contains("new_County"))
                         detail.County = address.GetAttributeValue<EntityReference>("new_County").Name;
                     if (contact.Contains("new_postalcode"))
                         detail.PostCode = address.GetAttributeValue<EntityReference>("new_postalcode").Name;
                 }
                 detail.FeedType = feedType; //"Customer";

                 details.Add(detail);

             }

             return details;

        }



    }
}