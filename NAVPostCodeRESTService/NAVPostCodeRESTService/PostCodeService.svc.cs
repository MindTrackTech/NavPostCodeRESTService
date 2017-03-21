using Microsoft.Xrm.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using NAVPostCodeRESTService.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Net;
using Microsoft.Xrm.Sdk.Client;


namespace NAVPostCodeRESTService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class PostCodeService : IPostCodeService
    {
        private OrganizationService _svc;
        private bool _disposed;

        public PostCodeService()
        {
            _svc = new OrganizationService("CRM");
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _svc.Dispose();
                _disposed = true;
            }
        }


        #endregion

        public class RiskClass
        {
            public IEnumerable<string> ProductCode { get; set; }

            public IList<Entity> Class { get; set; }
        }

        #region RetrieveAddress
        public IEnumerable<AddressDetails> RetrieveAddress(string postalCode)
        {
            try
            {
                //log.Info("RetrieveAddress method Invoked");

                //if (retrievedUser == null)
                //    throw new FaultException<Exception>(new Exception("CreateOrUpdateContact"), "User not logged in, please login to access the service");
                var contact = new Contact(_svc);
                postalCode = postalCode.Replace("{", "").Replace("}", "");
                var addressDetailsList = contact.RetrieveAddress(postalCode);

                return addressDetailsList;
            }
            catch (Exception ex)
            {
                //log.Error("Error in RetrieveAddress method : ", ex);
                throw new FaultException<Exception>(new Exception("RetrieveAddress"), ex.Message);
            }
        }
        #endregion

        public string CreateManualAddress(AddressDetails address)
        {
            var contact = new Contact(_svc);

            return contact.CreateManualAddress(address);

        }

        #region RetrieveBroker
        public IEnumerable<BrokerDetails> RetrieveBroker()
        {
            //List<BrokerDetails> detailsList = new List<BrokerDetails>();

            var brokers = _svc.RetrieveBroker();

            return MapBrokers(brokers, "Customer & Vendor");
            #region CommentedCode
            //var uniqueBrokers = brokers.Select(c=>c.GetAttributeValue<string>("new_brokercode")).Distinct();
            
            //foreach(var bro in uniqueBrokers)
            //{
            //    if (bro == null)
            //        continue;
            //    var broker = brokers.Where(c=> c.GetAttributeValue<string>("new_brokercode") == bro.ToString()).FirstOrDefault();
            //    var company = new Company(_svc, broker);
            //    BrokerDetails details = new BrokerDetails();
            //    var address = broker.GetAttributeValue<EntityReference>("new_address");
            //    Address addressDetails = null;
            //    if (address != null)
            //    {
            //        addressDetails = new Address(_svc, _svc.Retrieve(address.LogicalName, address.Id, new ColumnSet(true)));
            //        details.Address = addressDetails.street1;
            //        details.Address2 = addressDetails.street2;
            //        details.City = addressDetails.City;
            //        details.County = addressDetails.County;
            //        details.County = addressDetails.Country;
            //    }
            //    details.CRMCompanyCode = company.CompanyCode;
            //    details.Name = company.Name;
            //    details.MainContactName = company.MainContactName;
            //    details.PhoneNumber = company.MainPhone;
            //    details.MobileNumber = company.MobilePhone;
            //    details.CurrencyCode = company.CurrencyCode;
            //    details.Email = company.Email;
            //    details.HomePage = company.HomePage;

            //    detailsList.Add(details);
            //}
            #endregion

            //UpdateNavSentDates(_svc, brokers);

            //return detailsList;
           
        }
        #endregion

        public IEnumerable<BrokerDetails> RetrievePolicyHolder()
        {
            var policyHolder = _svc.RetrievePolicyHolder();

            return MapBrokers(policyHolder, "Customer");

            //UpdateNavSentDates(_svc, policyHolder);
        }

        public IEnumerable<BrokerDetails> RetrieveClaimBroker()
        {
            var claimBrokers = _svc.RetrieveClaimBroker();
            var brokerList = new List<Entity>();

            foreach (var claimBroker in claimBrokers)
            {
                object claim = null;
                EntityReference claimRef = null;
                if (claimBroker.Contains("P.new_claim"))
                {
                    claim = claimBroker.GetAttributeValue<AliasedValue>("P.new_claim").ToString();
                    claimRef = ((EntityReference)(claim));
                }
                if (claimRef != null)
                {
                    var claimEntity = _svc.Retrieve(claimRef.LogicalName, claimRef.Id, new ColumnSet(true));
                    var brokerInClaim = claimEntity.GetAttributeValue<EntityReference>("new_policyholdercompany");
                    var brokerInPayment = claimBroker.GetAttributeValue<EntityReference>("new_accountid");
                    if (brokerInClaim.Id != brokerInPayment.Id)
                        brokerList.Add(claimBroker);
                }
                else
                    brokerList.Add(claimBroker);
            }

             return MapBrokers(brokerList, "Vendor");
            //UpdateNavSentDates(_svc, brokerList);
        }

        public IEnumerable<BrokerDetails> RetrieveReInsuranceParticipant()
        {
            var reInsuranceParticipant = _svc.RetrieveReInsuranceParticipant();

            List<Entity> reInsuranceParticipantToSend = new List<Entity>();

            foreach(var reInsParticipant in reInsuranceParticipant)
            {
                var reInsuranceContract = reInsParticipant.GetAttributeValue<AliasedValue>("RP.new_reinsurancecontract").Value.ToString();

                if (reInsuranceContract != null)
                    reInsuranceParticipantToSend.Add(reInsParticipant);
            }

            return MapBrokers(reInsuranceParticipantToSend, "Vendor");

            //UpdateNavSentDates(_svc, reInsuranceParticipantToSend);
        }

        #region RetrieveVendor
        public IEnumerable<BrokerDetails> RetrieveVendor()
        {
            var brokers = _svc.RetrieveVendor();

            return MapBrokers(brokers, "Vendor");
        }
        #endregion

        public IEnumerable<BrokerDetails> RetrievePolicyHolderContact()
        {
            List<Entity> contactList = new List<Entity>();

            using (var context = new OrganizationServiceContext(_svc))
            {
                var query = from c in context.CreateQuery("contact")
                            join p in context.CreateQuery("new_policy")
                            on c["contactid"] equals p["new_insured_contact"]
                            join pp in context.CreateQuery("new_product")
                            on p["new_productid"] equals pp["new_productid"]
                            where (OptionSetValue)pp["new_customer"] == new OptionSetValue(100000001)
                            & (OptionSetValue)p["statuscode"] == new OptionSetValue(100000001)
                            select new { Cont = c };
                
                contactList.AddRange(query.ToList().Select(q => q.Cont.ToEntity<Entity>()));
            }

                var mapContacts = new Contact(_svc);
                var mappedContacts = mapContacts.MapContacts(contactList, "Customer");
            

            return mappedContacts;
        }

        #region RetrieveCommissionist
        public IEnumerable<BrokerDetails> RetrieveCommissionist()
        {
            var retrievedCommissions = new List<Entity>();
            var brokers = _svc.RetrieveCommissionist();

            var mappedBrokers = MapBrokers(brokers, "Vendor");

            var commissionId = brokers.Select(b => b.GetAttributeValue<AliasedValue>("CM.new_commissionsalesdetailid").Value.ToString());

            foreach(var comm in commissionId)
            {
                var retrievedCommisssion = _svc.Retrieve("new_commissionsalesdetail", new Guid(comm), new ColumnSet(true));

                if (retrievedCommisssion != null)
                    retrievedCommissions.Add(retrievedCommisssion);
            }

            //UpdateNavSentDates(_svc, retrievedCommissions);

            return mappedBrokers;
               
        }
        #endregion

        public IEnumerable<BrokerDetails> RetrieveReInsurer()
        {
            var retrievedReinsurances = new List<Entity>();
            var reinsurers = _svc.RetrieveReInsurer();
            var mappedReinsurers = MapBrokers(reinsurers, "Vendor");

            var reinsuranceId = reinsurers.Select(b => b.GetAttributeValue<AliasedValue>("RI.new_reinsuranceagreementid").Value.ToString());

            foreach(var reinsurance in reinsuranceId)
            {
                var retrievedReinsurance = _svc.Retrieve("new_reinsuranceagreement", new Guid(reinsurance), new ColumnSet(true));

                if (retrievedReinsurance != null)
                    retrievedReinsurances.Add(retrievedReinsurance);
            }

            //UpdateNavSentDates(_svc, retrievedReinsurances);

            return mappedReinsurers;
        }

        #region MapBrokers
        private List<BrokerDetails> MapBrokers(IEnumerable<Entity> brokers, string feedType)
        {
            List<BrokerDetails> detailsList = new List<BrokerDetails>();

            var uniqueBrokers = brokers.Select(c => c.GetAttributeValue<string>("new_brokercode")).Distinct();

            foreach (var bro in uniqueBrokers)
            {
                if (bro == null)
                    continue;
                var broker = brokers.Where(c => c.GetAttributeValue<string>("new_brokercode") == bro.ToString()).FirstOrDefault();
                var company = new Company(_svc, broker);

                BrokerDetails details = new BrokerDetails();
                var address = broker.GetAttributeValue<EntityReference>("new_address");
                Address addressDetails = null;
                if (address != null)
                {
                    addressDetails = new Address(_svc, _svc.Retrieve(address.LogicalName, address.Id, new ColumnSet(true)));
                    details.Address = addressDetails.street1;
                    details.Address2 = addressDetails.street2;
                    details.City = addressDetails.City;
                    details.County = addressDetails.County;
                    details.Country = addressDetails.Country;
                    details.PostCode = addressDetails.PostalCode;
                    details.AddressIdentifier = addressDetails.AddressIdentifier;
                }
                details.CRMCompanyCode = company.CompanyCode;
                details.Name = company.Name;
                details.MainContactName = company.MainContactName;
                details.PhoneNumber = company.MainPhone;
                details.MobileNumber = company.MobilePhone;
                details.CurrencyCode = company.CurrencyCode;
                details.Email = company.Email;
                details.HomePage = company.HomePage;
                details.FeedType = feedType;
                details.RecordType = company.StateCode;

                detailsList.Add(details);
            }

            //Uncomment the below code when we Go Live
            //UpdateNavSentDates(_svc, brokers);

            return detailsList;
        }
        #endregion

        #region UpdateNavSentDates
        private void UpdateNavSentDatesForDimension(IOrganizationService svc, IEnumerable<Entity> entityToUpdate)
        {
            foreach (var update in entityToUpdate)
            {
                Entity entity = new Entity(update.LogicalName);
                entity.Id = update.Id;
                entity["new_transferredtoaccountingon"] = DateTime.Now;
                //entity["new_alreadytransferredtoaccounting"] = true;
                svc.Update(entity);
            }
        }

        private void UpdateNavSentDates(IOrganizationService svc, IEnumerable<Entity> entityToUpdate)
        {
            foreach (var update in entityToUpdate)
            {
                Entity entity = new Entity(update.LogicalName);
                entity.Id = update.Id;
                entity["new_transferredtoaccountingon"] = DateTime.Now;
                entity["new_alreadytransferredtoaccounting"] = true;
                svc.Update(entity);
            }
        }
        #endregion

        public IEnumerable<BrokerDetails> RetrieveBrokerUpdateFromCRM()
        {
            var updatedBrokers = _svc.RetriveBrokerUpdate();

            return MapBrokers(updatedBrokers, null);
        }
        /// <summary>
        /// Retrieves Contacts linked to a policy as a policy holder
        /// </summary>
        /// <returns>contact details</returns>
        public IEnumerable<BrokerDetails> RetrieveContacts()
        {
            //var contacts = _svc.RetrieveContacts();
            //var mapContacts = new Contact(_svc);
            //var mappedContacts = mapContacts.MapContacts(contacts);
            //UpdateNavSentDates(_svc, contacts);
            //return mappedContacts;
            throw new NotImplementedException();
        }

        #region BrokerFromNAV
        public IEnumerable<ErrorData> BrokerFromNAV(IEnumerable<BrokerDetails> brokers)
        {
            List<ErrorData> errorList = new List<ErrorData>();
            //try
            //{
                foreach (var broker in brokers)
                {
                    var crmCode = broker.CRMCompanyCode;
                    var navCode = broker.NavCode;
                    var retrievedBroker = _svc.RetriveBrokerWithNavCode(navCode);
                    Entity address = null;
                    if (broker.AddressIdentifier != null && broker.AddressIdentifier != "")
                    {
                        address = _svc.RetrieveAddres(broker.AddressIdentifier);
                    }
                    if (retrievedBroker == null)
                    {
                        
                        try
                        {
                            _svc.CreateBroker(broker, address);
                            errorList.Add(new ErrorData(broker.NavCode, "Success", "Success"));

                        }
                        catch(Exception ex)
                        {
                            errorList.Add(new ErrorData(broker.NavCode, "Failed", ex.Message));
                        }
                    }
                    else
                    {
                        try
                        {
                            _svc.UpdateBroker(broker, retrievedBroker);
                            errorList.Add(new ErrorData(broker.NavCode, "Success", "Success"));

                        }
                        catch(Exception ex)
                        {
                            errorList.Add(new ErrorData(broker.NavCode, "Failed", ex.Message));
                        }
                    }
                }
                return errorList;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //    //ErrorData errorData = new ErrorData(ex.Message, "");
            //    //throw new WebFaultException<ErrorData>(errorData, HttpStatusCode.ExpectationFailed);
            //}
        }
        #endregion

        public IEnumerable<ErrorData> ContactsFromNAV(IEnumerable<ContactDetails> contacts)
        {
            List<ErrorData> errorList = new List<ErrorData>();
            //try
            //{
            foreach (var contact in contacts)
            {
                var crmCode = contact.CRMCode;
                var navCode = contact.NavCode;
                var retrievedContact = _svc.RetriveContactWithNavCode(navCode);
                Entity address = null;
                if (contact.AddressIdentifier != null && contact.AddressIdentifier != "")
                {
                    address = _svc.RetrieveAddres(contact.AddressIdentifier);
                }
                if (retrievedContact == null)
                {

                    try
                    {
                        _svc.CreateContact(contact, address);
                        errorList.Add(new ErrorData(contact.NavCode, "Success", "Success"));

                    }
                    catch (Exception ex)
                    {
                        errorList.Add(new ErrorData(contact.NavCode, "Failed", ex.Message));
                    }
                }
                else
                {
                    try
                    {
                        _svc.UpdateContact(contact, retrievedContact);
                        errorList.Add(new ErrorData(contact.NavCode, "Success", "Success"));

                    }
                    catch (Exception ex)
                    {
                        errorList.Add(new ErrorData(contact.NavCode, "Failed", ex.Message));
                    }
                }
            }
            return errorList;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //    //ErrorData errorData = new ErrorData(ex.Message, "");
            //    //throw new WebFaultException<ErrorData>(errorData, HttpStatusCode.ExpectationFailed);
            //}
        }

        #region RetrieveProductFromCRM
        public IEnumerable<ProductDetails> RetrieveProductFromCRM()
        {
            List<ProductDetails> details = new List<ProductDetails>();
            var products = _svc.RetrieveProducts();
            var retrieveProduct = new Dimensions(_svc);

            foreach(var prod in products)
            {
                var retrievedProductDetails = retrieveProduct.MapProducts(prod);
                details.Add(retrievedProductDetails);
                //retrieveProduct.UpdateProduct(prod);

            }

            UpdateNavSentDatesForDimension(_svc, products);

            return details;
        }
        #endregion

        #region RetrieveScheme
        public IEnumerable<Details> RetrieveScheme()
        {
            int lobType = 100000003; 
            List<Details> details = new List<Details>();
            var schemes = _svc.RetrieveBusinessScheme(lobType); //_svc.RetrieveSchemes();
            var dimension = new Details();

            foreach(var scheme in schemes)
            {
                var prodCode = scheme.GetAttributeValue<AliasedValue>("PR.new_code") == null ? "" : scheme.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                var schemeDetails = new Details();
                schemeDetails.Name = scheme.GetAttributeValue<string>("new_name");
                schemeDetails.Code = scheme.GetAttributeValue<string>("new_lobcode");
                schemeDetails.ProductCode = prodCode;
                schemeDetails.FeedType = "Scheme";
                schemeDetails.RecordType = scheme.GetAttributeValue<OptionSetValue>("statecode").Value;

                details.Add(schemeDetails);

            }

            UpdateNavSentDatesForDimension(_svc, schemes);

            return details;
        }
        #endregion

        #region RetrieveBrand
        public IEnumerable<Brand> RetrieveBrand()
        {
            try
            {
                var brands = _svc.RetrieveBrands();

                List<Brand> brandDetails = new List<Brand>();

                foreach (var brand in brands)
                {
                    var prodCode = brand.GetAttributeValue<AliasedValue>("PR.new_code") == null ? "" : brand.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                    var brandDetail = new Brand();
                    brandDetail.BrandCode = brand.GetAttributeValue<string>("new_brandcode");
                    brandDetail.BrandName = brand.GetAttributeValue<string>("new_name");
                    brandDetail.ProductCode = prodCode;
                    brandDetail.FeedType = "Brand";
                    brandDetail.RecordType = brand.GetAttributeValue<OptionSetValue>("statecode").Value;

                    brandDetails.Add(brandDetail);
                }

                UpdateNavSentDatesForDimension(_svc, brands);

                return brandDetails;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public IEnumerable<CoverSection> RetrieveCoverSection()
        {
            List<CoverSection> coverSectionDetails = new List<CoverSection>();
            var RetrievedCoverSections = _svc.RetriveCoverSections();
            var dimension = new Dimensions(_svc);

            foreach (var details in RetrievedCoverSections)
            {
                var mappedDetail = dimension.MapCoverSection(details);
                coverSectionDetails.Add(mappedDetail);
            }

            UpdateNavSentDatesForDimension(_svc, RetrievedCoverSections);
            
            return coverSectionDetails;
        }

        public IEnumerable<RiskObjectDetails> RetrieveRiskObject()
        {
            List<RiskObjectDetails> riskObjectDetails = new List<RiskObjectDetails>();
            var retrievedRiskObjects = _svc.RetrieveRiskObject();
            var dimension = new Dimensions(_svc);

            foreach(var details in retrievedRiskObjects)
            {
                var mappedDetail = dimension.MapRiskObjects(details);
                riskObjectDetails.Add(mappedDetail);
            }

            UpdateNavSentDates(_svc, retrievedRiskObjects);

            return riskObjectDetails;
        }

        public IEnumerable<Details> RetrieveTerritory()
        {
            try
            {
                List<Details> details = new List<Details>();

                var territories = _svc.RetrieveTerritory();

                foreach (var territory in territories)
                {
                    //var prodCode = territory.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                    var territoryDetails = new Details();
                    territoryDetails.Name = territory.GetAttributeValue<string>("name");
                    territoryDetails.Code = territory.GetAttributeValue<string>("new_territorycode");
                    //territoryDetails.ProductCode = prodCode;
                    territoryDetails.FeedType = "Territory";
                    territoryDetails.RecordType = territory.GetAttributeValue<OptionSetValue>("new_territorystatus").Value;

                    details.Add(territoryDetails);
                }

                UpdateNavSentDatesForDimension(_svc, territories);

                return details;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Details> RetrieveBusinessSector()
        {
            try
            {
                int lobType = 100000000;
                List<Details> details = new List<Details>();

                var businessSectors = _svc.RetrieveBusinessSector(lobType);

                foreach(var businessSector in businessSectors)
                {
                    var prodCode = businessSector.GetAttributeValue<AliasedValue>("PR.new_code") == null ? "" : businessSector.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                    var businessSectorDetails = new Details();
                    businessSectorDetails.Name = businessSector.GetAttributeValue<string>("new_name");
                    businessSectorDetails.Code = businessSector.GetAttributeValue<string>("new_lobcode");
                    businessSectorDetails.ProductCode = prodCode;
                    businessSectorDetails.FeedType = "Business Sector";
                    businessSectorDetails.RecordType = businessSector.GetAttributeValue<OptionSetValue>("statecode").Value;

                    details.Add(businessSectorDetails);
                }

                UpdateNavSentDatesForDimension(_svc, businessSectors);

                return details;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Details> RetrieveBusinessType()
        {
            try
            {
                int lobType = 100000002;
                List<Details> details = new List<Details>();

                var businessTypes = _svc.RetrieveBusinessType(lobType);

                foreach(var businessType in businessTypes)
                {
                    var prodCode = businessType.GetAttributeValue<AliasedValue>("PR.new_code") == null ? "" :  businessType.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                    var businessTypeDetails = new Details();
                    businessTypeDetails.Name = businessType.GetAttributeValue<string>("new_name");
                    businessTypeDetails.Code = businessType.GetAttributeValue<string>("new_lobcode");
                    businessTypeDetails.ProductCode = prodCode;
                    businessTypeDetails.FeedType = "Business Type";
                    businessTypeDetails.RecordType = businessType.GetAttributeValue<OptionSetValue>("statecode").Value;

                    details.Add(businessTypeDetails);
                }

                UpdateNavSentDatesForDimension(_svc, businessTypes);

                return details;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Details> RetrieveBusinessLine()
        {
            try
            {
                int lobType = 100000001;
                List<Details> details = new List<Details>();

                var businessLines = _svc.RetrieveBusinessLine(lobType);

                foreach(var businessLine in businessLines)
                {
                    var prodCode = businessLine.GetAttributeValue<AliasedValue>("PR.new_code") == null ? "" : businessLine.GetAttributeValue<AliasedValue>("PR.new_code").Value.ToString();
                    var businessLineDetails = new Details();
                    businessLineDetails.Name = businessLine.GetAttributeValue<string>("new_name");
                    businessLineDetails.Code = businessLine.GetAttributeValue<string>("new_lobcode");
                    businessLineDetails.ProductCode = prodCode;
                    businessLineDetails.FeedType = "Business Line";
                    businessLineDetails.RecordType = businessLine.GetAttributeValue<OptionSetValue>("statecode").Value;

                    details.Add(businessLineDetails);
                }

                UpdateNavSentDatesForDimension(_svc, businessLines);

                return details;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Aggregates> RetrievePerilSections()
        {
            try
            {
                List<Aggregates> aggr = new List<Aggregates>();
                var perilSections = _svc.RetrievePerilSections();

                var products = perilSections.Select(e => e.GetAttributeValue<EntityReference>("new_productid")).Distinct();

                IEnumerable<Entity> perilSection = null;

                foreach(var product in products)
                {
                    if (product == null)
                        continue;
                    perilSection = _svc.RetrievePerilSectionsForProduct(product.Id);

                    foreach(var peril in perilSection)
                    {
                        //var prodCode = ((AliasedValue)peril["PR.new_code"]).Value.ToString();
                        //var regClassCode = ((AliasedValue)peril["REG.new_regulatoryclasscode"]).Value.ToString();
                        //var repClassCode = ((AliasedValue)peril["REP.new_reportingclasscode"]).Value.ToString();
                        //var lobClassCode = ((AliasedValue)peril["LOB.new_lobclasscode"]).Value.ToString();
                        //var perilSectionName = peril.GetAttributeValue<string>("new_name");
                        //var weighting = peril.GetAttributeValue<decimal>("new_coverbasepercentage");

                        var perilSectionAgr = new Aggregates();
                        if (peril.Contains("LOB.new_lobclasscode"))
                            perilSectionAgr.LOBClass = (peril.GetAttributeValue<AliasedValue>("LOB.new_lobclasscode")).Value.ToString();
                        if (peril.Contains("new_name"))
                            perilSectionAgr.PerilSection = peril.GetAttributeValue<string>("new_name");
                        if (peril.Contains("PR.new_code"))
                            perilSectionAgr.ProductCode = (peril.GetAttributeValue<AliasedValue>("PR.new_code")).Value.ToString(); //((AliasedValue)peril["PR.new_code"]).Value.ToString(); 
                        if (peril.Contains("REG.new_regulatoryclasscode"))
                            perilSectionAgr.RegulatoryClass = (peril.GetAttributeValue<AliasedValue>("REG.new_regulatoryclasscode")).Value.ToString(); //((AliasedValue)peril["REG.new_regulatoryclasscode"]).Value.ToString();
                        if (peril.Contains("REP.new_reportingclasscode"))
                            perilSectionAgr.ReportingClass = (peril.GetAttributeValue<AliasedValue>("REP.new_reportingclasscode")).Value.ToString(); //((AliasedValue)peril["REP.new_reportingclasscode"]).Value.ToString();
                        if (peril.Contains("new_coverbasepercentage"))
                            perilSectionAgr.Weighting = peril.GetAttributeValue<decimal>("new_coverbasepercentage");
                        if(peril.Contains("COV.new_riskcode"))
                            perilSectionAgr.CoverCode = (peril.GetAttributeValue<AliasedValue>("COV.new_riskcode")).Value.ToString();
                        if (peril.Contains("COV.new_name"))
                            perilSectionAgr.CoverName = (peril.GetAttributeValue<AliasedValue>("COV.new_name")).Value.ToString();
                        perilSectionAgr.FeedType = "Weighting";
                        perilSectionAgr.RecordType = peril.GetAttributeValue<OptionSetValue>("statecode").Value;

                        aggr.Add(perilSectionAgr);
                    }
                }

                UpdateNavSentDatesPerilSection(perilSection);

                return aggr;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void UpdateNavSentDatesPerilSection(IEnumerable<Entity> perilSection)
        {
            foreach(var peril in perilSection)
            {
                Entity entity = new Entity(peril.LogicalName);
                entity.Id = peril.Id;
                entity["new_transferredtoaccountingon"] = DateTime.Now;
                //entity["new_alreadytransferredtoaccounting"] = true;
                _svc.Update(entity);
            }
        }

        public IEnumerable<Details> RetrieveRiskClass()
        {
            var riskClasses = _svc.RetrieveRiskClass(100000000);

            riskClasses.ToList().ForEach(p => p.FeedType = "Risk Class");

            UpdateNavSentDatesForRiskClass(riskClasses);
            
            return riskClasses;

            //List<Details> retrievedRiskClass = new List<Details>();
        }

        public IEnumerable<Details> RetrieveRiskSubClass()
        {
            var riskClasses = _svc.RetrieveRiskSubClass(100000001);

            //riskClasses.ToList().ForEach(p => p.FeedType = "Risk SubClass");

            UpdateNavSentDatesForRiskClass(riskClasses);

            return riskClasses;
        }

        public IEnumerable<Details> RetrieveReportingClass()
        {
            var reportingClasses = _svc.RetrieveEntityWhereTransferredToNAVIsNull("new_reportingclass");

            UpdateNavSentDatesForDimension(_svc, reportingClasses);

            return new List<Details>(
                reportingClasses.Select(s => new Details
                {
                    Code = s.GetAttributeValue<string>("new_reportingclasscode"),
                    Name = s.GetAttributeValue<string>("new_name"),
                    FeedType = "Reporting Class",
                    RecordType = s.GetAttributeValue<OptionSetValue>("statecode").Value
                }));
        }

        public IEnumerable<Details> RetrieveRegulatoryClass()
        {
            var regClasses = _svc.RetrieveEntityWhereTransferredToNAVIsNull("new_regulatoryclass");

            UpdateNavSentDatesForDimension(_svc, regClasses);

            return new List<Details>(
                regClasses.Select(s => new Details
                {
                    Code = s.GetAttributeValue<string>("new_regulatoryclasscode"),
                    Name = s.GetAttributeValue<string>("new_name"),
                    FeedType = "Regulatory Class",
                    RecordType = s.GetAttributeValue<OptionSetValue>("statecode").Value
                }));
        }

        public IEnumerable<Details> RetrieveLOBClass()
        {
            var lobClasses = _svc.RetrieveEntityWhereTransferredToNAVIsNull("new_lobclass");

            UpdateNavSentDatesForDimension(_svc, lobClasses);

            return new List<Details>(
                lobClasses.Select(s => new Details
                {
                    Code = s.GetAttributeValue<string>("new_lobclasscode"),
                    Name = s.GetAttributeValue<string>("new_name"),
                    FeedType = "LOB Class",
                    RecordType = s.GetAttributeValue<OptionSetValue>("statecode").Value
                }));
        }

        public IEnumerable<Details> RetrieveCoverCode()
        {
            var coverCodes = _svc.RetrieveEntityWhereTransferredToNAVIsNull("new_risk");

            UpdateNavSentDatesForDimension(_svc, coverCodes);

            return new List<Details>(
                coverCodes.Select(s => new Details
                {
                    Code = s.GetAttributeValue<string>("new_riskcode"),
                    Name = s.GetAttributeValue<string>("new_name"),
                    FeedType = "Cover Code",
                    RecordType = s.GetAttributeValue<OptionSetValue>("statecode").Value
                }));
        }

        private void UpdateNavSentDatesForRiskClass(IEnumerable<Details> entityToUpdate)
        {
            List<string> riskClassCode = entityToUpdate.Select(c => c.Code).ToList();

            foreach (var code in riskClassCode)
            {
                var riskClass = _svc.RetrieveRiskClassBasedOnCode(code);
                Entity entity = new Entity(riskClass.LogicalName);
                entity.Id = riskClass.Id;
                entity["new_transferredtoaccountingon"] = DateTime.Now;
                //entity["new_alreadytransferredtoaccounting"] = true;
                _svc.Update(entity);
            }

        }

        public IEnumerable<BrokerDetails> RetrieveContactCommissionist()
        {
            var contact = _svc.RetrieveContactCommissionist();

            var mapContacts = new Contact(_svc);
            var mappedContacts = mapContacts.MapContacts(contact, "Vendor");


            return mappedContacts;

        }


    }
}
