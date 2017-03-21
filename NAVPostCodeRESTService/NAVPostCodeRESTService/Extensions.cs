using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.Text.RegularExpressions;
using NAVPostCodeRESTService.Entities;
using Microsoft.Xrm.Sdk.Client;
using NAVPostCodeRESTService;


namespace NAVPostCodeRESTService
{
    internal static class Extension
    {
        /// <summary>
        /// Converts enum to OptionSetValue.
        /// </summary>
        /// <param name="e">Enum to convert to OptionSetValue</param>
        /// <returns>OptionSetValue with integer value of enum.</returns>
        public static OptionSetValue ToOptionSet(this Enum e)
        {
            return new OptionSetValue(Convert.ToInt32(e));
        }

        #region RetrieveCustomEntityByName
        public static IEnumerable<Entity> RetrieveCustomEntityByName(this IOrganizationService svc, string name, string entityName)
        {
            //ThrowIf.Argument.IsNull(svc, "svc");
            //ThrowIf.Argument.IsNullOrEmpty(accountName, "accountName");

            var query = new QueryExpression(entityName);
            query.Criteria.AddCondition("new_name", ConditionOperator.Equal, name);
            query.ColumnSet.AllColumns = true;

            var result = svc.RetrieveMultiple(query);

            return result.Entities;
        }
        #endregion

        #region RetrieveCRMEntityByName
        public static IEnumerable<Entity> RetrieveCRMEntityByName(this IOrganizationService svc, string name, string entityName)
        {
            //ThrowIf.Argument.IsNull(svc, "svc");
            //ThrowIf.Argument.IsNullOrEmpty(accountName, "accountName");

            var query = new QueryExpression(entityName);
            query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
            query.ColumnSet.AllColumns = true;

            var result = svc.RetrieveMultiple(query);

            return result.Entities;
        }
        #endregion

        #region RetrieveUsers
        public static IEnumerable<Entity> RetrieveUsers(this IOrganizationService svc, string userName, Guid? portal = null)
        {
            //ThrowIf.Argument.IsNull(svc, "svc");
            //ThrowIf.Argument.IsNullOrEmpty(entityLogicalName, "entityLogicalName");
            //ThrowIf.Argument.IsNull(policyVersionId, "policyVersionId");

            var query = new QueryByAttribute("new_externaluser");
            query.AddAttributeValue("new_name", userName);
            if (portal != null)
                query.AddAttributeValue("new_portal", portal);
            //query.AddAttributeValue("new_password", password);
            query.ColumnSet = new ColumnSet(true);
            var result = svc.RetrieveMultiple(query);

            return result.Entities;
        }
        #endregion

        #region RetrieveAddressForPostCode
        public static IEnumerable<Entity> RetrieveAddressForPostCode(this IOrganizationService svc, Guid postCode)
        {
            QueryExpression query = new QueryExpression("new_address");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_postalcode", ConditionOperator.Equal, postCode);

            return svc.RetrieveMultiple(query).Entities;
        }
        #endregion

        #region CheckIfPostalCodeExists
        /// <summary>
        /// Checks if postal code already exists in CRM, searches for record by new_codeforsearch.
        /// </summary>
        /// <param name="svc">Organization service.</param>
        /// <param name="postalCode">Normalized postal code for search.</param>
        /// <param name="countryId">Id of country, for which we search for postal code.</param>
        /// <param name="currentRecordId">Id of current record, which should be ignored</param>
        /// <returns></returns>
        public static Entity CheckIfPostalCodeExists(this IOrganizationService svc, string postalCode, Guid countryId, Guid? currentRecordId = null)
        {
            //ThrowIf.Argument.IsNull(svc, "svc");
            //ThrowIf.Argument.IsNullOrEmpty(postalCode, "postalCode");
            //ThrowIf.Argument.IsNotValid(countryId == Guid.Empty, "countryId", "countryId cannot be empty GUID.");

            var pcQuery = new QueryExpression("new_postalcode");
            pcQuery.ColumnSet.AddColumn("new_name");
            pcQuery.TopCount = 1;

            pcQuery.Criteria.AddCondition("new_codeforsearch", ConditionOperator.Equal, postalCode);
            pcQuery.Criteria.AddCondition("new_country", ConditionOperator.Equal, countryId);
            pcQuery.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (currentRecordId != null)
                pcQuery.Criteria.AddCondition("new_postalcodeid", ConditionOperator.NotEqual, currentRecordId.Value);

            var results = svc.RetrieveMultiple(pcQuery);

            return results.Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveOrCreatePostalCode
        public static Entity RetrieveOrCreatePostalCode(this IOrganizationService svc, string postalCode, Entity country)
        {
            var normalized = Regex.Replace(postalCode.ToLowerInvariant(), @"[^a-z0-9]+", "", RegexOptions.CultureInvariant);

            //var checkPostcodeExists = svc.RetrieveCustomEntityByName(postalCode, "new_postalcode");

            var checkPostcodeExists = svc.CheckIfPostalCodeExists(normalized, country.Id);

            //var checkPostcodeExists = svc.CheckIfPostalCodeExists(postalCode, country.Id);

            Entity postalCodeEntity = null;

            if (checkPostcodeExists == null)
            {
                postalCodeEntity = new Entity("new_postalcode");
                postalCodeEntity["new_name"] = postalCode;
                postalCodeEntity["new_country"] = country.ToEntityReference();
                var postcode = svc.Create(postalCodeEntity);
                return svc.Retrieve("new_postalcode", postcode, new ColumnSet(true));

            }

            return checkPostcodeExists;
        }
        #endregion

        #region RetrieveCounty
        public static Entity RetrieveCounty(this IOrganizationService svc, string county, Guid country)
        {
            QueryExpression query = new QueryExpression("new_county");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_name", ConditionOperator.Equal, county);
            query.Criteria.AddCondition("new_country", ConditionOperator.Equal, country);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveCity
        public static Entity RetrieveCity(this IOrganizationService svc, string city, Guid county)
        {
            QueryExpression query = new QueryExpression("new_city");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_name", ConditionOperator.Equal, city);
            query.Criteria.AddCondition("new_countyid", ConditionOperator.Equal, county);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveAddressBasedOnIdentifier
        public static Entity RetrieveAddressBasedOnIdentifier(this IOrganizationService svc, string identifier)
        {
            QueryExpression query = new QueryExpression("new_address");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_identifier", ConditionOperator.Equal, identifier);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveContactByName
        public static IEnumerable<Entity> RetrieveContactByName(this IOrganizationService svc, string firstName, string lastName, string postalCode = null, DateTime? dateOfBirth = null, string email = null, string mobile = null, string niNumber = null)
        {
            //ThrowIf.Argument.IsNull(svc, "svc");
            //ThrowIf.Argument.IsNullOrEmpty(firstName, "firstName");
            //ThrowIf.Argument.IsNullOrEmpty(lastName, "lastName");
            //ThrowIf.Argument.IsNullOrEmpty(postalCode, "postalCode");

            var query = new QueryExpression("contact");
            query.Criteria.AddCondition("firstname", ConditionOperator.Equal, firstName);
            query.Criteria.AddCondition("lastname", ConditionOperator.Equal, lastName);

            if (postalCode != null)
            {
                var postCodeLink = query.AddLink("new_postalcode", "new_postalcode", "new_postalcodeid");
                postCodeLink.LinkCriteria.AddCondition("new_name", ConditionOperator.Like, postalCode);
            }
            else
            {
                return Enumerable.Empty<Entity>();
            }

            if (dateOfBirth != null)
                query.Criteria.AddCondition("birthdate", ConditionOperator.On, dateOfBirth.Value.Date);
            else
                query.Criteria.AddCondition("birthdate", ConditionOperator.Null);

            if (email != null)
                query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);
            else
                query.Criteria.AddCondition("emailaddress1", ConditionOperator.Null);


            if (mobile != null)
                query.Criteria.AddCondition("mobilephone", ConditionOperator.Equal, mobile);
            else
                query.Criteria.AddCondition("mobilephone", ConditionOperator.Null);

            if (niNumber != null)
                query.Criteria.AddCondition("new_clientninumber", ConditionOperator.Equal, niNumber);
            else
                query.Criteria.AddCondition("new_clientninumber", ConditionOperator.Null);

            query.ColumnSet.AllColumns = true;

            var result = svc.RetrieveMultiple(query);
            return result.Entities;
        }
        #endregion

        #region RetrieveBroker
        public static IEnumerable<Entity> RetrieveBroker(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            //query.Criteria.AddCondition("createdon", ConditionOperator.LessEqual, DateTime.Now);
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            //FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.Or);
            //filter.AddCondition("modifiedon", ConditionOperator.LessEqual, DateTime.Now);

            var link = query.AddLink("opportunity", "accountid", "customerid");

            //link.Columns = new ColumnSet(true);


            return svc.RetrieveMultiple(query).Entities;
        }
        #endregion

        public static IEnumerable<Entity> RetrieveCommissionist(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("customertypecode", ConditionOperator.NotEqual, 100000001);

            var link = query.AddLink("new_commissionsalesdetail", "accountid", "new_company");
            link.LinkCriteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            link.Columns.AddColumn("new_commissionsalesdetailid");
            link.EntityAlias = "CM";

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrievePolicyHolder(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            var link = query.AddLink("new_product", "accountid", "new_brokerid");
            link.LinkCriteria.AddCondition("new_customer", ConditionOperator.Equal, 100000001);
            //link.LinkCriteria.AddCondition("new_customer", ConditionOperator.Equals, 100000001);

            var link1 = query.AddLink("new_policy", "accountid", "new_insuredid");
            //link1.LinkCriteria.AddCondition("", ConditionOperator.EndsWith, "");

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrievePolicyHolderContact(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            var link = query.AddLink("new_policy", "contactid", "new_insured_contact");
            link.Columns.AddColumns("new_policyid", "new_productid");
            link.EntityAlias = "P";
            //link1.LinkCriteria.AddCondition("", ConditionOperator.EndsWith, "");

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrieveReInsuranceParticipant(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;

            var link = query.AddLink("new_reinsuranceparticipant", "accountid", "new_reinsurer");
            link.Columns.AddColumn("new_reinsurancecontract");
            link.EntityAlias = "RP";

            return svc.RetrieveMultiple(query).Entities;
            //QueryExpression query = new QueryExpression("new_reinsuranceparticipant");

        }

        //public static Entity RetrieveProduct(this IOrganizationService svc, Guid policyId)
        //{
        //    QueryExpression query = new QueryExpression("new_product");
        //    query.ColumnSet.AllColumns = true;
        //    query.Criteria.AddCondition("")
        //}

        public static IEnumerable<Entity> RetrieveReInsurer(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;

            var link = query.AddLink("new_reinsuranceagreement", "accountid", "new_broker");
            link.LinkCriteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            link.Columns.AddColumn("new_reinsuranceagreementid");
            link.EntityAlias = "RI";

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrieveContacts(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            var link = query.AddLink("new_policy", "contactid", "new_insured_contact");

            return svc.RetrieveMultiple(query).Entities;
        }

        public static Entity RetriveContactWithNavCode(this IOrganizationService svc, string navCode)
        {
            QueryExpression query = new QueryExpression("contact");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_navcode", ConditionOperator.Equal, navCode);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        #region RetrieveVendor
        //public static IEnumerable<Entity> RetrieveVendor(this IOrganizationService svc)
        //{
        //    QueryExpression query = new QueryExpression("account");
        //    query.ColumnSet.AllColumns = true;
        //    //query.Criteria.AddCondition("createdon", ConditionOperator.LessEqual, DateTime.Now);
        //    query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

        //    //FilterExpression filter = query.Criteria.AddFilter(LogicalOperator.Or);
        //    //filter.AddCondition("modifiedon", ConditionOperator.LessEqual, DateTime.Now);

        //    var link = query.AddLink("new_commissionsalesdetail", "accountid", "new_company");

        //    //link.Columns = new ColumnSet(true);


        //    return svc.RetrieveMultiple(query).Entities;
        //}
        #endregion

        public static IEnumerable<Entity> RetrieveVendor(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;

            var link = query.AddLink("new_commissionsalesdetail", "accountid", "new_company");
            link.LinkCriteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            link.Columns.AddColumn("new_commissionsalesdetailid");
            link.EntityAlias = "COMM";

            return svc.RetrieveMultiple(query).Entities;
        }

        #region RetriveBrokerWithBrokerCode
        public static Entity RetriveBrokerWithBrokerCode(this IOrganizationService svc, string brokerCode)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_brokercode", ConditionOperator.Equal, brokerCode);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region CreateBroker
        public static void CreateBroker(this IOrganizationService svc, BrokerDetails details, Entity address)
        {
            Entity broker = new Entity("account");
            broker["name"] = details.Name;
            broker["telephone1"] = details.PhoneNumber;
            broker["telephone2"] = details.MobileNumber;
            broker["emailaddress1"] = details.Email;
            broker["websiteurl"] = details.HomePage;
            broker["new_transferredtoaccountingon"] = DateTime.Now;
            broker["new_navcompanycode"] = details.NavCode;
            broker["new_alreadytransferredtoaccounting"] = true;

            Entity currency = null;
            if (details.CurrencyCode != null)
                currency = svc.RetrieveCurrency(details.CurrencyCode);
            if (currency != null)
                broker["transactioncurrencyid"] = currency.ToEntityReference();

            if (address != null)
            {
                broker["new_address"] = address.ToEntityReference();
                broker["new_countryid"] = address.GetAttributeValue<EntityReference>("new_country");
                broker["new_postalcode"] = address.GetAttributeValue<EntityReference>("new_postalcode");
            }

            //if(details.PostCode != null && details.PostCode != "")
            //{
            //    var postCode = svc.RetrieveCRMEntityByName(details.PostCode, "new_postalcode"); //RetrieveCustomEntityByName(details.PostCode, "new_postalcode");
            //    var manualAddress = svc.CreateManualAddress(details);
            //}

            svc.Create(broker);

        }
        #endregion

        #region CreateContact
        public static void CreateContact(this IOrganizationService svc, ContactDetails details, Entity address)
        {
            Entity contact = new Entity("contact");
            contact["firstname"] = details.FirstName;
            contact["middlename"] = details.MiddleName;
            contact["lastname"] = details.LastName;

            contact["telephone1"] = details.PhoneNumber;
            contact["mobilephone"] = details.MobileNumber;
            contact["emailaddress1"] = details.Email;
            contact["new_transferredtoaccountingon"] = DateTime.Now;
            contact["new_navcode"] = details.NavCode;
            contact["new_alreadytransferredtoaccounting"] = true;

            //Entity currency = null;
            //if (details.CurrencyCode != null)
            //    currency = svc.RetrieveCurrency(details.CurrencyCode);
            //if (currency != null)
            //    broker["transactioncurrencyid"] = currency.ToEntityReference();

            if (address != null)
            {
                contact["new_address"] = address.ToEntityReference();
                contact["new_countryid"] = address.GetAttributeValue<EntityReference>("new_country");
                contact["new_postalcode"] = address.GetAttributeValue<EntityReference>("new_postalcode");
            }

            //if (details.PostCode != null && details.PostCode != "")
            //{
            //    var postCode = svc.RetrieveCRMEntityByName(details.PostCode, "new_postalcode"); //RetrieveCustomEntityByName(details.PostCode, "new_postalcode");
            //    //var manualAddress = svc.CreateManualAddress(details);
            //}

            svc.Create(contact);

        }
        #endregion

        #region UpdateContact
        public static void UpdateContact(this IOrganizationService svc, ContactDetails details, Entity retrievedContact)
        {
            Entity contact = new Entity(retrievedContact.LogicalName);
            contact.Id = retrievedContact.Id;
            contact["firstname"] = details.FirstName;
            contact["middlename"] = details.MiddleName;
            contact["lastname"] = details.LastName;

            contact["telephone1"] = details.PhoneNumber;
            contact["mobilephone"] = details.MobileNumber;
            contact["emailaddress1"] = details.Email;
            contact["new_transferredtoaccountingon"] = DateTime.Now;
            contact["new_alreadytransferredtoaccounting"] = true;
            svc.Update(contact);
            
            
        }
        #endregion

                
        #region UpdateBroker
        public static void UpdateBroker(this IOrganizationService svc, BrokerDetails details, Entity retrievedBroker)
        {
            Entity broker = new Entity("account");
            broker.Id = retrievedBroker.Id;
            broker["name"] = details.Name;
            broker["telephone1"] = details.PhoneNumber;
            broker["telephone2"] = details.MobileNumber;
            broker["emailaddress1"] = details.Email;
            broker["websiteurl"] = details.HomePage;
            broker["new_alreadytransferredtoaccounting"] = true;

            svc.Update(broker);
            broker["new_transferredtoaccountingon"] = DateTime.Now;
            svc.Update(broker);
        }
        #endregion

        #region RetrieveAddres
        public static Entity RetrieveAddres(this IOrganizationService svc, string addressIdentifier)
        {
            QueryExpression query = new QueryExpression("new_address");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_identifier", ConditionOperator.Equal, addressIdentifier);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();

        }
        #endregion

        #region RetriveBrokerWithNavCode
        public static Entity RetriveBrokerWithNavCode(this IOrganizationService svc, string NavCode)
        {
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_navcompanycode", ConditionOperator.Equal, NavCode);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveCurrency
        public static Entity RetrieveCurrency(this IOrganizationService svc, string currencyCode)
        {
            QueryExpression query = new QueryExpression("transactioncurrency");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("isocurrencycode", ConditionOperator.Equal, currencyCode);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }
        #endregion

        #region RetrieveProducts
        public static IEnumerable<Entity> RetrieveProducts(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_product");
            query.ColumnSet.AddColumns("new_code", "new_name", "new_productid", "statecode");
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            //var link1 = query.AddLink("new_covertype", "new_covertype", "new_covertypeid");
            //link1.JoinOperator = JoinOperator.Inner;
            //link1.Columns.AddColumns("new_covertypecode", "new_name");
            //link1.EntityAlias = "CT";

            var link2 = query.AddLink("territory", "new_secondlevelterritoryid", "territoryid");
            link2.JoinOperator = JoinOperator.LeftOuter;
            link2.Columns.AddColumns("new_territorycode", "name");
            link2.EntityAlias = "FT";

            var link3 = query.AddLink("new_lob1", "new_scheme", "new_lob1id");
            link3.JoinOperator = JoinOperator.LeftOuter;
            link3.Columns.AddColumns("new_lobcode", "new_name");
            link3.EntityAlias = "FL";

            var link4 = query.AddLink("new_brand", "new_brandid", "new_brandid");
            link4.JoinOperator = JoinOperator.LeftOuter;
            link4.Columns.AddColumns("new_brandcode", "new_name");
            link4.EntityAlias = "FB";

            var link5 = query.AddLink("new_lob1", "new_secondlevellobid", "new_lob1id");
            link5.JoinOperator = JoinOperator.LeftOuter;
            link5.Columns.AddColumns("new_lobcode", "new_name");
            link5.EntityAlias = "BL";

            var link6 = query.AddLink("new_lob1", "new_firstlevellobid", "new_lob1id");
            link6.JoinOperator = JoinOperator.LeftOuter;
            link6.Columns.AddColumns("new_lobcode", "new_name");
            link6.EntityAlias = "BS";

            var link7 = query.AddLink("new_lob1", "new_thirdlevellobid", "new_lob1id");
            link7.JoinOperator = JoinOperator.LeftOuter;
            link7.Columns.AddColumns("new_lobcode", "new_name");
            link7.EntityAlias = "BT";

            #region commentedcode
            //var link1 = query.AddLink("new_covertype", "new_covertype", "new_covertypeid");
            //link1.JoinOperator = JoinOperator.Inner;
            //link1.Columns.AddColumns("new_covertypecode", "new_name");
            //link1.EntityAlias = "CT";

            //var link2 = query.AddLink("territory", "new_secondlevelterritoryid", "territoryid");
            //link2.JoinOperator = JoinOperator.Inner;
            //link2.Columns.AddColumns("new_territorycode", "name");
            //link2.EntityAlias = "FT";

            //var link3 = query.AddLink("new_lob1", "new_scheme", "new_lob1id");
            //link3.JoinOperator = JoinOperator.Inner;
            //link3.Columns.AddColumns("new_lobcode", "new_name");
            //link3.EntityAlias = "FL";

            //var link4 = query.AddLink("new_brand", "new_brandid", "new_brandid");
            //link4.JoinOperator = JoinOperator.Inner;
            //link4.Columns.AddColumns("new_brandcode", "new_name");
            //link4.EntityAlias = "FB";
            #endregion

            return svc.RetrieveMultiple(query).Entities;
        }
        #endregion

        #region RetrieveSchemes
        public static IEnumerable<Entity> RetrieveSchemes(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_lob1");
            query.ColumnSet.AddColumns("new_lobcode", "new_name", "new_lob1id", "new_loblevel");
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            query.Criteria.AddCondition("new_lobcode", ConditionOperator.NotNull);
            query.Criteria.AddCondition("new_loblevel", ConditionOperator.Equal, 100000003);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_lob1", "new_parentlob", "new_lob1id");
            link.LinkCriteria.AddCondition("new_loblevel", ConditionOperator.Equal, 100000002);
            link.Columns.AddColumns("new_lobcode", "new_name", "new_loblevel");
            link.JoinOperator = JoinOperator.Inner;
            link.EntityAlias = "LB2";

            return svc.RetrieveMultiple(query).Entities;

        }
        #endregion

        #region RetrieveBrands
        public static IEnumerable<Entity> RetrieveBrands(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_brand");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_product", "new_brandid", "new_brandid");
            link.JoinOperator = JoinOperator.LeftOuter;
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;

        }
        #endregion

        #region RetriveCoverSections
        public static IEnumerable<Entity> RetriveCoverSections(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_cover");
            query.ColumnSet.AddColumns("new_covercode", "new_name", "new_coverbasepercentage");
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            var link1 = query.AddLink("new_risk", "new_riskid", "new_riskid");
            link1.JoinOperator = JoinOperator.Inner;
            link1.Columns.AddColumns("new_riskcode", "new_name");
            link1.EntityAlias = "FR";

            var link2 = query.AddLink("new_basiccover", "new_basiccover", "new_basiccoverid");
            link2.JoinOperator = JoinOperator.Inner;
            link2.Columns.AddColumns("new_basiccovercode", "new_name");
            link2.EntityAlias = "BC";

            var link3 = query.AddLink("new_regulatoryclass", "new_firstlevelregulatoryclassid", "new_regulatoryclassid");
            link3.JoinOperator = JoinOperator.Inner;
            link3.Columns.AddColumns("new_regulatoryclasscode", "new_name");
            link3.EntityAlias = "RC";

            var link4 = query.AddLink("new_reportingclass", "new_reportingclass", "new_reportingclassid");
            link4.JoinOperator = JoinOperator.Inner;
            link4.Columns.AddColumns("new_reportingclasscode", "new_name");
            link4.EntityAlias = "RPC";

            var link5 = query.AddLink("new_lobclass", "new_lobclass", "new_lobclassid");
            link5.JoinOperator = JoinOperator.Inner;
            link5.Columns.AddColumns("new_lobclasscode", "new_name");
            link5.EntityAlias = "LC";

            var link6 = query.AddLink("new_product", "new_productid", "new_productid");
            link6.JoinOperator = JoinOperator.Inner;
            link6.Columns.AddColumns("new_code", "new_name");
            link6.EntityAlias = "P";

            return svc.RetrieveMultiple(query).Entities;
        }
        #endregion

        #region RetrieveRiskObject
        public static IEnumerable<Entity> RetrieveRiskObject(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_risk");
            query.ColumnSet.AddColumns("new_riskcode", "new_name");
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            query.Criteria.AddCondition("new_riskcode", ConditionOperator.NotNull);


            var link1 = query.AddLink("new_riskclass", "new_secondlevelriskclassid", "new_riskclassid");
            link1.JoinOperator = JoinOperator.Inner;
            link1.Columns.AddColumns("new_riskclasscode", "new_name");
            link1.EntityAlias = "FC";

            var link2 = query.AddLink("new_product", "new_productid", "new_productid");
            link2.JoinOperator = JoinOperator.Inner;
            link2.Columns.AddColumns("new_code", "new_name");
            link2.EntityAlias = "P";

            return svc.RetrieveMultiple(query).Entities;
        }
        #endregion

        public static IEnumerable<Entity> RetrieveTerritory(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("territory");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            //var link = query.AddLink("new_product", "territoryid", "new_secondlevelterritoryid");
            //link.Columns.AddColumn("new_code");
            //link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;
        }

        public static IEnumerable<Entity> RetrieveBusinessLine(this IOrganizationService svc, int lobType)
        {
            QueryExpression query = new QueryExpression("new_lob1");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_loblevel", ConditionOperator.Equal, lobType);
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_product", "new_lob1id", "new_secondlevellobid");
            link.JoinOperator = JoinOperator.LeftOuter;
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrieveBusinessSector(this IOrganizationService svc, int lobType)
        {
            QueryExpression query = new QueryExpression("new_lob1");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_loblevel", ConditionOperator.Equal, lobType);
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_product", "new_lob1id", "new_firstlevellobid");
            link.JoinOperator = JoinOperator.LeftOuter;
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrieveBusinessType(this IOrganizationService svc, int lobType)
        {
            QueryExpression query = new QueryExpression("new_lob1");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_loblevel", ConditionOperator.Equal, lobType);
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_product", "new_lob1id", "new_thirdlevellobid");
            link.JoinOperator = JoinOperator.LeftOuter;
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;

        }

        public static IEnumerable<Entity> RetrieveBusinessScheme(this IOrganizationService svc, int lobType)
        {
            QueryExpression query = new QueryExpression("new_lob1");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_loblevel", ConditionOperator.Equal, lobType);
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query.AddLink("new_product", "new_lob1id", "new_scheme");
            link.JoinOperator = JoinOperator.LeftOuter;
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            return svc.RetrieveMultiple(query).Entities;

        } 

        public static IEnumerable<Details> RetrieveRiskClass(this IOrganizationService svc, int riskClassLevel)
        {
            
            using (var context = new OrganizationServiceContext(svc))
            {
                var query = (from rc in context.CreateQuery("new_riskclass")
                            join r in context.CreateQuery("new_risk")
                            on rc["new_riskclassid"] equals r["new_firstlevelriskclassid"]
                            join pp in context.CreateQuery("new_product")
                            on r["new_productid"] equals pp["new_productid"]
                            where (DateTime)rc["new_transferredtoaccountingon"] == null
                            & (OptionSetValue)rc["new_riskclasslevel"] == new OptionSetValue(riskClassLevel)
                             select new { Name = rc.GetAttributeValue<string>("new_name"),
                                        Product = pp.GetAttributeValue<string>("new_code"),
                                        Code = rc.GetAttributeValue<string>("new_riskclasscode"),
                                        RecordType = rc.GetAttributeValue<OptionSetValue>("statecode").Value 
                             }).ToList();

                List<Details> details = query.Select(e => new Details
                    {
                        Name = e.Name,
                        ProductCode = e.Product,
                        Code = e.Code,
                        RecordType = e.RecordType
                        
                    }).ToList();

                return details;
                
            }
                       
                        
        }

        public static IEnumerable<Details> RetrieveRiskSubClass(this IOrganizationService svc, int riskClassLevel)
        {
            QueryExpression query = new QueryExpression("new_riskclass");
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            query.Criteria.AddCondition("new_riskclasslevel", ConditionOperator.Equal, riskClassLevel);
            query.ColumnSet.AllColumns = true;

            var result = svc.RetrieveMultiple(query).Entities;

            return new List<Details>(result.Select(s => new Details
            {
                Name = s.GetAttributeValue<string>("new_name"),
                Code = s.GetAttributeValue<string>("new_riskclasscode"),
                RecordType = s.GetAttributeValue<OptionSetValue>("statecode").Value,
                FeedType = "Risk SubClass"
            })).ToList();

            //using (var context = new OrganizationServiceContext(svc))
            //{
            //    var query = (from rc in context.CreateQuery("new_riskclass")
            //                 join r in context.CreateQuery("new_risk")
            //                 on rc["new_riskclassid"] equals r["new_secondlevelriskclassid"]
            //                 join pp in context.CreateQuery("new_product")
            //                 on r["new_productid"] equals pp["new_productid"]
            //                 where (DateTime)rc["new_transferredtoaccountingon"] == null
            //                 & (OptionSetValue)rc["new_riskclasslevel"] == new OptionSetValue(riskClassLevel)
            //                 select new
            //                 {
            //                     Name = rc.GetAttributeValue<string>("new_name"),
            //                     Product = pp.GetAttributeValue<string>("new_code"),
            //                     Code = rc.GetAttributeValue<string>("new_riskclasscode"),
            //                     RecordType = rc.GetAttributeValue<OptionSetValue>("statecode").Value
            //                 }).ToList();

            //    List<Details> details = query.Select(e => new Details
            //    {
            //        Name = e.Name,
            //        ProductCode = e.Product,
            //        Code = e.Code,
            //        RecordType = e.RecordType

            //    }).ToList();

            //    return details;

            //}

                       
        }

        public static Entity RetrieveRiskClassBasedOnCode(this IOrganizationService svc, string code)
        {
            QueryExpression query = new QueryExpression("new_riskclass");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_riskclasscode", ConditionOperator.Equal, code);

            return svc.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public static IEnumerable<Entity> RetrieveEntityWhereTransferredToNAVIsNull(this IOrganizationService svc, string entityName)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);

            return svc.RetrieveMultiple(query).Entities;
        }

        public static IEnumerable<Details> GetDetails(this NAVPostCodeRESTService.PostCodeService.RiskClass riskClass)
        {
            List<Details> details = riskClass.Class.Select(e => new Details
            {
                Name = e.GetAttributeValue<string>("new_name"),
                Code = e.GetAttributeValue<string>("new_riskclasscode"),
                RecordType = e.GetAttributeValue<OptionSetValue>("statecode").Value,
                
            }).ToList();

            
            return details;
 
           // details.AddRange()
        }

        public static IEnumerable<Entity> RetrieveClaimBroker(this IOrganizationService svc)
        {
            var query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("customertypecode", ConditionOperator.NotEqual, 100000001);

            var link = query.AddLink("new_payment", "accountid", "new_accountid");
            link.Columns.AddColumn("new_claim");
            link.EntityAlias = "P";

            return svc.RetrieveMultiple(query).Entities;
        }

        public static IEnumerable<Entity> RetriveBrokerUpdate(this IOrganizationService svc)
        {
            var query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            query.Criteria.AddCondition("new_alreadytransferredtoaccounting", ConditionOperator.Equal, true);

            return svc.RetrieveMultiple(query).Entities;
        }

        public static IEnumerable<Entity> RetrievePerilSections(this IOrganizationService svc)
        {
            QueryExpression query = new QueryExpression("new_cover");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return svc.RetrieveMultiple(query).Entities;
        }
        public static IEnumerable<Entity> RetrievePerilSectionsForProduct(this IOrganizationService svc, Guid productId)
        {
            var query1 = new QueryExpression("new_cover");
            query1.ColumnSet.AddColumns("new_name", "new_coverbasepercentage", "statecode", "new_coverid");
            query1.Criteria.AddCondition("new_productid", ConditionOperator.Equal, productId);
            //query1.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            var link = query1.AddLink("new_product", "new_productid", "new_productid");
            link.Columns.AddColumn("new_code");
            link.EntityAlias = "PR";

            var link1 = query1.AddLink("new_regulatoryclass", "new_firstlevelregulatoryclassid", "new_regulatoryclassid");
            link1.Columns.AddColumn("new_regulatoryclasscode");
            link1.EntityAlias = "REG";

            var link2 = query1.AddLink("new_reportingclass", "new_reportingclass", "new_reportingclassid");
            link2.Columns.AddColumn("new_reportingclasscode");
            link2.EntityAlias = "REP";

            var link3 = query1.AddLink("new_lobclass", "new_lobclass", "new_lobclassid");
            link3.Columns.AddColumn("new_lobclasscode");
            link3.EntityAlias = "LOB";

            var link4 = query1.AddLink("new_risk", "new_riskid", "new_riskid");
            link4.Columns.AddColumns("new_riskcode","new_name");
            link4.EntityAlias = "COV";

            return svc.RetrieveMultiple(query1).Entities;
        }

        public static IEnumerable<Entity> RetrieveContactCommissionist(this IOrganizationService svc)
        {
            var query = new QueryExpression("new_commissionsalesdetail");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("new_transferredtoaccountingon", ConditionOperator.Null);
            query.Criteria.AddCondition("new_contact", ConditionOperator.NotNull);

            return svc.RetrieveMultiple(query).Entities;

        }


        



    }
}