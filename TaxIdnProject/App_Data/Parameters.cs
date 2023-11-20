using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxIdnProject.Miscs
{
    public class Parameters
    {
        public IDictionary<string, string> uriAPI;
        public IDictionary<string, string> securityAPI;
        public IDictionary<string, string> securityWorkcenter;
        public IDictionary<string, string> tableName;

        public string workcenterURI;
        public string customWorkcenterURI;

        public Parameters()
        {
            //workcenterURI = "https://my359301.sapbydesign.com/sap/byd/odata/analytics/ds/";
            //customWorkcenterURI = "https://my359301.sapbydesign.com/sap/byd/odata/cust/v1/";

            workcenterURI = "https://my432668.businessbydesign.cloud.sap/sap/byd/odata/analytics/ds/";
            customWorkcenterURI = "https://my432668.businessbydesign.cloud.sap/sap/byd/odata/cust/v1/";

            uriAPI = new Dictionary<string, string>();

            uriAPI["SupplierInvoice"] = workcenterURI + "SrmivB03.svc/SrmivB03?$format=json";
            //uriAPI["SupplierInvoiceItem"] = customWorkcenterURI + "khsupplierinvoice/ItemCollection?$format=json&$expand=ItemTaxCalculation";
            uriAPI["SupplierInvoiceItem"] = customWorkcenterURI + "khsupplierinvoice/SupplierInvoiceCollection?$format=json&$expand=ExternalReference,SellerParty,SellerParty/SellerPartyName,SellerParty/SellerPartyAddress,Item";
            uriAPI["SupplierMaster"] = customWorkcenterURI + "khsupplier/SupplierCollection?$format=json&$expand=TaxNumber";
            uriAPI["CustomerMaster"] = customWorkcenterURI + "khcustomer/CustomerCollection?$format=json&$expand=TaxNumber";
            uriAPI["CustomerInvoice"] = customWorkcenterURI + "khcustomerinvoice/CustomerInvoiceQueryByElements2?$format=json&$expand=BuyerParty,BuyerParty/BuyerPartyName,BuyerParty/BuyerPartyFormattedAddress,Item";
            uriAPI["PostNoFakturOut"] = customWorkcenterURI + "khcustomerinvoice/TextCollectionCollection";

            tableName = new Dictionary<string, string>();

            tableName["SupplierInvoice"] = "EFakturContainer";
            tableName["CustomerInvoice"] = "EFakturContainer";

            securityAPI = new Dictionary<string, string>();

            //securityAPI["Username"] = "_TAXINDONESI";
            //securityAPI["Password"] = "BravuraTax1ndonesia";
            
            securityAPI["Username"] = "_BRV_INTEGRA";
            securityAPI["Password"] = "P@$$w0rd@987654321";

            securityWorkcenter = new Dictionary<string, string>();

            //securityWorkcenter["Username"] = "Rizki.Adjie";
            //securityWorkcenter["Password"] = "P@$$w0rd90";

            securityWorkcenter["Username"] = "rizaldi@bravura.co.id";
            securityWorkcenter["Password"] = "P@$$w0rd@987654321";
        }
    }
}