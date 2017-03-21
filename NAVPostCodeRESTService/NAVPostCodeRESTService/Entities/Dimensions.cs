using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace NAVPostCodeRESTService.Entities
{
    public class Dimensions
    {
        IOrganizationService _svc;
        public Dimensions(IOrganizationService svc)
        {
            _svc = svc;
        }
        public ProductDetails MapProducts(Entity product)
        {
            ProductDetails detail = new ProductDetails();
            detail.ProductCode = product.GetAttributeValue<string>("new_code");
            detail.ProductName = product.GetAttributeValue<string>("new_name");
            detail.BrandCode = product.GetAttributeValue<AliasedValue>("FB.new_brandcode") == null ? "" : product.GetAttributeValue<AliasedValue>("FB.new_brandcode").Value.ToString();
            detail.BrandName = product.GetAttributeValue<AliasedValue>("FB.new_name") == null ? "" : product.GetAttributeValue<AliasedValue>("FB.new_name").Value.ToString();
            //detail.CoverTypeCode = product.GetAttributeValue<AliasedValue>("CT.new_covertypecode") == null ? "" : product.GetAttributeValue<AliasedValue>("CT.new_covertypecode").Value.ToString();
            //detail.CoverTypeName = product.GetAttributeValue<AliasedValue>("CT.new_name") == null ? "" : product.GetAttributeValue<AliasedValue>("CT.new_name").Value.ToString();
            detail.FeedType = "Product";
            detail.SchemeCode = product.GetAttributeValue<AliasedValue>("FL.new_lobcode") == null ? "" : product.GetAttributeValue<AliasedValue>("FL.new_lobcode").Value.ToString();
            detail.TerritoryCode = product.GetAttributeValue<AliasedValue>("FT.new_territorycode") == null ? "" : product.GetAttributeValue<AliasedValue>("FT.new_territorycode").Value.ToString();
            detail.TerritoryName = product.GetAttributeValue<AliasedValue>("FT.name") == null ? "" : product.GetAttributeValue<AliasedValue>("FT.name").Value.ToString();
            detail.BusinessLineCode = product.GetAttributeValue<AliasedValue>("BL.new_lobcode") == null ? "" : product.GetAttributeValue<AliasedValue>("BL.new_lobcode").Value.ToString();
            detail.BusinessLineName = product.GetAttributeValue<AliasedValue>("BL.new_name") == null ? "" : product.GetAttributeValue<AliasedValue>("BL.new_name").Value.ToString();
            detail.BusinessSectorCode = product.GetAttributeValue<AliasedValue>("BS.new_lobcode") == null ? "" : product.GetAttributeValue<AliasedValue>("BS.new_lobcode").Value.ToString();
            detail.BusinessSectorName = product.GetAttributeValue<AliasedValue>("BS.new_name") == null ? "" : product.GetAttributeValue<AliasedValue>("BS.new_name").Value.ToString();
            detail.BusinessTypeCode = product.GetAttributeValue<AliasedValue>("BT.new_lobcode") == null ? "" : product.GetAttributeValue<AliasedValue>("BT.new_lobcode").Value.ToString();
            detail.BusinessTypeName = product.GetAttributeValue<AliasedValue>("BT.new_name") == null ? "" : product.GetAttributeValue<AliasedValue>("BT.new_name").Value.ToString();
            detail.RecordType = product.GetAttributeValue<OptionSetValue>("statecode").Value;

            return detail;
                
        }

        public SchemeDetails MapScheme(Entity scheme)
        {
            var test = scheme.GetAttributeValue<AliasedValue>("LB2.new_loblevel").Value;
            SchemeDetails detail = new SchemeDetails();
            //detail.ProductLineCode = scheme.GetAttributeValue<AliasedValue>("LB2.new_lobcode") == null ? "" : scheme.GetAttributeValue<AliasedValue>("LB2.new_lobcode").Value.ToString();
            //detail.ProductLineLevel = scheme.GetAttributeValue<AliasedValue>("LB2.new_loblevel") == null ? "" : ((OptionSetValue)(scheme.GetAttributeValue<AliasedValue>("LB2.new_loblevel").Value)).Value.ToString();
            //detail.ProductLineName = scheme.GetAttributeValue<AliasedValue>("LB2.new_name") == null ? "" : scheme.GetAttributeValue<AliasedValue>("LB2.new_name").Value.ToString();
            detail.SchemeCode = scheme.GetAttributeValue<string>("new_lobcode");
            //detail.SchemeLevel = scheme.GetAttributeValue<OptionSetValue>("new_loblevel").Value.ToString();
            detail.SchemeName = scheme.GetAttributeValue<string>("new_name");
            detail.FeedType = "Scheme";

            //detail.ProductLineCode = "";
            //detail.FeedType = "";
            //detail.ProductLineLevel = "";
            //detail.ProductLineName = "";
            //detail.SchemeCode = "";
            //detail.SchemeLevel = "";
            //detail.SchemeName = "";
            

            return detail;
        }

        public void UpdateProduct(Entity product)
        {
            Entity updateProduct = new Entity(product.LogicalName);
            updateProduct.Id = product.Id;
            updateProduct["new_transferredtoaccountingon"] = DateTime.Now;
            _svc.Update(updateProduct);
        }

        public CoverSection MapCoverSection(Entity coverSection)
        {
            CoverSection section = new CoverSection();
            section.BasicCoverCode = coverSection.GetAttributeValue<AliasedValue>("BC.new_basiccovercode") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("BC.new_basiccovercode").Value.ToString();
            section.BasicCoverName = coverSection.GetAttributeValue<AliasedValue>("BC.new_name") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("BC.new_name").Value.ToString();
            section.CoverCode = coverSection.GetAttributeValue<string>("new_covercode");
            section.CoverName = coverSection.GetAttributeValue<string>("new_name");
            section.CoverPercentage = coverSection.GetAttributeValue<decimal>("new_coverbasepercentage").ToString();
            section.FeedType = "Cover Section";
            section.LOBClassCode = coverSection.GetAttributeValue<AliasedValue>("LC.new_lobclasscode") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("LC.new_lobclasscode").Value.ToString();
            section.LOBClassName = coverSection.GetAttributeValue<AliasedValue>("LC.new_name") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("LC.new_name").Value.ToString();
            section.ProductCode = coverSection.GetAttributeValue<AliasedValue>("P.new_code") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("P.new_code").Value.ToString();
            section.RegulatoryClassCode = coverSection.GetAttributeValue<AliasedValue>("RC.new_regulatoryclasscode") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("RC.new_regulatoryclasscode").Value.ToString();
            section.RegulatoryClassName = coverSection.GetAttributeValue<AliasedValue>("RC.new_name") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("RC.new_name").Value.ToString();
            section.RiskSubClassCode = coverSection.GetAttributeValue<AliasedValue>("FR.new_riskcode") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("FR.new_riskcode").Value.ToString();
            section.RiskSubClassName = coverSection.GetAttributeValue<AliasedValue>("FR.new_name") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("FR.new_name").Value.ToString();
            section.ReportingClassCode = coverSection.GetAttributeValue<AliasedValue>("RPC.new_reportingclasscode") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("RPC.new_reportingclasscode").Value.ToString();
            section.ReportingClassName = coverSection.GetAttributeValue<AliasedValue>("RPC.new_name") == null ? "" : coverSection.GetAttributeValue<AliasedValue>("RPC.new_name").Value.ToString();

            return section;
        }

        public RiskObjectDetails MapRiskObjects(Entity riskObjects)
        {
            RiskObjectDetails riskObject = new RiskObjectDetails();
            riskObject.FeedType = "Risk Object";
            riskObject.ProductCode = riskObjects.GetAttributeValue<AliasedValue>("P.new_code") == null ? "" : riskObjects.GetAttributeValue<AliasedValue>("P.new_code").Value.ToString();
            riskObject.ProductName = riskObjects.GetAttributeValue<AliasedValue>("P.new_name") == null ? "" : riskObjects.GetAttributeValue<AliasedValue>("P.new_name").Value.ToString();
            riskObject.RiskClassCode = riskObjects.GetAttributeValue<AliasedValue>("FC.new_riskclasscode") == null ? "" : riskObjects.GetAttributeValue<AliasedValue>("FC.new_riskclasscode").Value.ToString();
            riskObject.RiskClassName = riskObjects.GetAttributeValue<AliasedValue>("FC.new_name") == null ? "" : riskObjects.GetAttributeValue<AliasedValue>("FC.new_name").Value.ToString();
            riskObject.RiskCode = riskObjects.GetAttributeValue<string>("new_riskcode");
            riskObject.RiskName = riskObjects.GetAttributeValue<string>("new_name");

            return riskObject;
        }
    }
}