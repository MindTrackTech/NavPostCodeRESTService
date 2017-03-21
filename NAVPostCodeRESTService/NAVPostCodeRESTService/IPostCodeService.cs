using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NAVPostCodeRESTService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPostCodeService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetPostcodeList/{postalCode}")]
        IEnumerable<AddressDetails> RetrieveAddress(string postalCode);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetCoverCode")]
        //IEnumerable<Details> RetrieveCoverCode();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetLOBClass")]
        //IEnumerable<Details> RetrieveLOBClass();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetRegulatoryClass")]
        //IEnumerable<Details> RetrieveRegulatoryClass();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetReportingClass")]
        //IEnumerable<Details> RetrieveReportingClass();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetContactCommissionist")]
        IEnumerable<BrokerDetails> RetrieveContactCommissionist();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBrokerUpdateFromCRM")]
        IEnumerable<BrokerDetails> RetrieveBrokerUpdateFromCRM();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetClaimBroker")]
        IEnumerable<BrokerDetails> RetrieveClaimBroker();
        //List<string> GetPostcodeList();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetPolicyHolderContact")]
        IEnumerable<BrokerDetails> RetrievePolicyHolderContact();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetRiskClass")]
        IEnumerable<Details> RetrieveRiskClass();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetRiskSubClass")]
        IEnumerable<Details> RetrieveRiskSubClass();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBroker")]
        IEnumerable<BrokerDetails> RetrieveBroker();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetReInsuranceParticipant")]
        IEnumerable<BrokerDetails> RetrieveReInsuranceParticipant();
        
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetPolicyHolder")]
        IEnumerable<BrokerDetails> RetrievePolicyHolder();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetReInsurer")]
        //IEnumerable<BrokerDetails> RetrieveReInsurer();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetCommissionist")]
        IEnumerable<BrokerDetails> RetrieveCommissionist();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetVendor")]
        IEnumerable<BrokerDetails> RetrieveVendor();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "brokersfromnav")]
        IEnumerable<ErrorData> BrokerFromNAV(IEnumerable<BrokerDetails> brokers);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "contactsfromnav")]
        IEnumerable<ErrorData> ContactsFromNAV(IEnumerable<ContactDetails> contacts);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/manualaddress")]
        string CreateManualAddress(AddressDetails address);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetProduct")]
        IEnumerable<ProductDetails> RetrieveProductFromCRM();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetScheme")]
        IEnumerable<Details> RetrieveScheme();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBrand")]
        IEnumerable<Brand> RetrieveBrand();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetCoverSection")]
        IEnumerable<CoverSection> RetrieveCoverSection();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetRiskObject")]
        //IEnumerable<RiskObjectDetails> RetrieveRiskObject();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetContacts")]
        //IEnumerable<BrokerDetails> RetrieveContacts();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetTerritory")]
        IEnumerable<Details> RetrieveTerritory();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBusinessSector")]
        IEnumerable<Details> RetrieveBusinessSector();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBusinessType")]
        IEnumerable<Details> RetrieveBusinessType();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetBusinessLine")]
        IEnumerable<Details> RetrieveBusinessLine();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "GetPerilSectionsWeightings")]
        IEnumerable<Aggregates> RetrievePerilSections();
    }

    #region DataMember

    public class Brand
    {
        [DataMember]
        public string BrandName {get; set;}

        [DataMember]
        public string BrandCode {get; set;}

        [DataMember]
        public string FeedType {get; set;}

        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public int RecordType { get; set; }

       
    }
    #region AddressDetails
    public class AddressDetails
    {
        [DataMember]
        public string street1 { get; set; }

        [DataMember]
        public string street2 { get; set; }

        [DataMember]
        public string street3 { get; set; }

        [DataMember]
        public string AddressName { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string BuildingDetails { get; set; }

        [DataMember]
        public string AddressIdentifier { get; set; }

        [DataMember]
        public string Number { get; set; }
    }
    #endregion

    public class BrokerDetails
    {
        [DataMember]
        public string CRMCompanyCode { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        [DataMember]
        public string MainContactName { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string MobileNumber { get; set; }

        [DataMember]
        public string CurrencyCode { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string HomePage { get; set; }

        [DataMember]
        public string NavCode { get; set; }

        [DataMember]
        public string AddressIdentifier { get; set; }

        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public int RecordType { get; set; }

    }

    public class ContactDetails
    {
        [DataMember]
        public string CRMCode { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string County { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        //[DataMember]
        //public string MainContactName { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string MobileNumber { get; set; }

        //[DataMember]
        //public string CurrencyCode { get; set; }

        [DataMember]
        public string Email { get; set; }

        //[DataMember]
        //public string HomePage { get; set; }

        [DataMember]
        public string NavCode { get; set; }

        [DataMember]
        public string AddressIdentifier { get; set; }

        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public int RecordType { get; set; }

    }

    public class ProductDetails
    {
        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public string TerritoryCode { get; set; }

        [DataMember]
        public string TerritoryName { get; set; }

        [DataMember]
        public string CoverTypeCode { get; set; }

        [DataMember]
        public string CoverTypeName { get; set; }

        [DataMember]
        public string BrandCode { get; set; }

        [DataMember]
        public string BrandName { get; set; }

        [DataMember]
        public string SchemeCode { get; set; }

        [DataMember]
        public string SchemeName { get; set; }

        [DataMember]
        public string BusinessLineCode { get; set; }

        [DataMember]
        public string BusinessLineName { get; set; }

        [DataMember]
        public string BusinessSectorCode { get; set; }

        [DataMember]
        public string BusinessSectorName { get; set; }

        [DataMember]
        public string BusinessTypeCode { get; set; }

        [DataMember]
        public string BusinessTypeName { get; set; }

        [DataMember]
        public int RecordType { get; set; }

    }

    public class SchemeDetails
    {
        [DataMember]
        public string SchemeCode { get; set; }

        [DataMember]
        public string SchemeName { get; set; }

        [DataMember]
        public int RecordType { get; set; }

        //[DataMember]
        //public string ProductLineCode { get; set; }

        //[DataMember]
        //public string ProductLineName { get; set; }

        //[DataMember]
        //public string SchemeLevel { get; set; }

        //[DataMember]
        //public string ProductLineLevel { get; set; }

        [DataMember]
        public string FeedType { get; set; }
    }

    public class CoverSection
    {
        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public string CoverCode { get; set; }

        [DataMember]
        public string CoverName { get; set; }

        [DataMember]
        public string RiskSubClassCode { get; set; }

        [DataMember]
        public string RiskSubClassName { get; set; }

        [DataMember]
        public string BasicCoverCode { get; set; }

        [DataMember]
        public string BasicCoverName { get; set; }

        [DataMember]
        public string RegulatoryClassCode { get; set; }

        [DataMember]
        public string RegulatoryClassName { get; set; }

        [DataMember]
        public string LOBClassCode { get; set; }

        [DataMember]
        public string LOBClassName { get; set; }

        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public string CoverPercentage { get; set; }

        [DataMember]
        public string ReportingClassCode { get; set; }

        [DataMember]
        public string ReportingClassName { get; set; }
    }

    public class RiskObjectDetails
    {
        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public string RiskCode { get; set; }

        [DataMember]
        public string RiskName { get; set; }

        [DataMember]
        public string RiskClassCode { get; set; }

        [DataMember]
        public string RiskClassName { get; set; }

        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public string ProductName { get; set; }
    }

    public class Details
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public int RecordType { get; set; }

    }

    public class Aggregates
    {
        [DataMember]
        public string ProductCode { get; set; }

        [DataMember]
        public string PerilSection { get; set; }

        [DataMember]
        public string RegulatoryClass { get; set; }

        [DataMember]
        public string ReportingClass { get; set; }

        [DataMember]
        public string LOBClass { get; set; }

        [DataMember]
        public decimal Weighting { get; set; }

        [DataMember]
        public string CoverCode { get; set; }

        [DataMember]
        public string CoverName { get; set; }

        [DataMember]
        public string FeedType { get; set; }

        [DataMember]
        public int RecordType { get; set; }
    }

    [DataContract]
    public class ErrorData
    {
        public ErrorData(string code, string status, string reason)
        {
            Reason = reason;
            Code = code;
            Status = status;
        }

        [DataMember]
        public string Reason { get; private set; }

        [DataMember]
        public string Code { get; private set; }

        [DataMember]
        public string Status { get; private set; }
    }

    #endregion

    
}
