using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByDSync.Miscs
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
            workcenterURI = "https://my361178.sapbydesign.com/sap/byd/odata/analytics/ds/";
            customWorkcenterURI = "https://my361178.sapbydesign.com/sap/byd/odata/cust/v1/";

            uriAPI = new Dictionary<string, string>();

            uriAPI["GeneralLedger"] = workcenterURI + "Finglau02.svc/Finglau02?$select=C_Fiscyear,C_Chofaccts,C_AddrBusPart,C_BusPartUuid,C_EmployeeUuid,C_CustomCode1,C_AccDocUuid,C_CustomCode2,C_CustomCode3,C_Accdoctype,C_AccDocItUuid,C_LaChangeDate,C_CreationDate,C_PostingDate,C_ProductUuid,C_Glacct,C_Debitcredit,C_GlacctTc,K_Amtlit,K_ValQuantity,K_Amttra,K_Amtcomp,to_ProfitctrUuid/C_Prftctrid,to_ProjTaskUuid/C_ProjectId,to_Glacct/T_Description,C_Oedpartner,C_Accdoctype,C_OedItemTc,C_Closestep&$expand=to_ProfitctrUuid,to_ProjTaskUuid,to_Glacct&$format=json";
            uriAPI["JournalEntry"] = workcenterURI + "Finglau03.svc/Finglau03?$select=C_AccDocUuid,C_SaLcIdUuid,C_SaLcDateTime,C_SaCrIdUuid,C_CreationDate,C_GlacctTc,C_Fiscyear,C_Debitcredit,C_Glacct,C_NoteIt,C_AccDocId,K_Amtcomp,K_Amttra,K_Amtlit&$format=json";
            uriAPI["Project"] = workcenterURI + "Proprjb.svc/Proprjb?$select=C_BuyerPtyUuid,C_CompanyUuid,C_RiskLevel,C_StatusBlocking,C_StatusLfc,C_ProjAEndDat,C_ProjAStDat,C_ProjectUuid,C_ProjType,C_ProjectId,T_Description,to_RespCcUuid/C_CcType,to_RespCcUuid/C_Costctr,to_RespCcUuid/C_Costctrid,to_ReqCcUuid/C_Profitctr,to_RespCcUuid/C_Profitctr,Count&$expand=to_ReqCcUuid,to_RespCcUuid&$format=json";
            uriAPI["ProfitCenter"] = workcenterURI + "xMOMxProfitctr.svc/xMOMxProfitctr?$select=C_Prftctr,C_Prftctrid,T_Name,C_ManPos,C_MgrEe,C_ForPost,C_ForPlan,C_ExtensionOrgId,Count&$format=json";
            uriAPI["Supplier"] = workcenterURI + "Supplier.svc/Supplier?$select=C_AddrAddStreetPrefix,C_AddrAddStreetSuffix,C_IndssctrCode,C_EmailUri,C_AddrStPrefix,C_CntryCode,C_CityName,T_BpFrmtdName,C_BpUuid,C_AddrStSuffix,C_BpLastChgDt,C_BpIntId,C_PoboxId,C_BpCrnDatetime,C_Indssystem,C_FrmtdPstlAddr,C_AddrFrmtdPh,C_StName,C_RegionCode,C_PoboxPostlcode&$format=json";
            uriAPI["ProjectTask"] = workcenterURI + "Propobb.svc/Propobb?$select=C_LastChangeDateTime,C_CreationDateTime,C_TaskUuid,C_ProjectUuid,C_TRespCcUuid,C_ProjectId,C_RespCcUuid,C_TaskId&$format=json";
            uriAPI["CustomCode3"] = customWorkcenterURI + "customcode3new/AccountingDocumentItemCustomCode3Collection?$format=json";
            uriAPI["CustomCode2"] = customWorkcenterURI + "customcode2/AccountingDocumentItemCustomCode2Collection?$format=json";
            uriAPI["CustomCode1"] = customWorkcenterURI + "customcode1/AccountingDocumentItemCustomCode1Collection?$format=json";

            tableName = new Dictionary<string, string>();

            tableName["GeneralLedger"] = "gen_ledger";
            tableName["JournalEntry"] = "journal_entries";
            tableName["Project"] = "project";
            tableName["ProfitCenter"] = "profit_center";
            tableName["CustomCode3"] = "code_list";
            tableName["CustomCode2"] = "code_list";
            tableName["CustomCode1"] = "code_list";
            tableName["Supplier"] = "supplier";
            tableName["ProjectTask"] = "project_task";

            securityAPI = new Dictionary<string, string>();

            securityAPI["Username"] = "_ADMIN";
            securityAPI["Password"] = "Admin123";

            securityWorkcenter = new Dictionary<string, string>();

            securityWorkcenter["Username"] = "Administrator";
            securityWorkcenter["Password"] = "Jclec@2022";
        }
    }
}