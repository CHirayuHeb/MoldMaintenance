using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAINTENANCE_MOLD.Models.Table
{
    public class Tb_Search
    {
        public class ViewSearch
        {
            public string v_plant { get; set; }
            public string v_Date { get; set; }
            public string V_Status { get; set; }
            public string v_Docno { get; set; }
            public string v_MDocno { get; set; }
            public string v_empcode { get; set; }
            public string v_page { get; set; }
            public string v_MoldNo { get; set; }
        }
        public class ViewSearchPage
        {
            public string s_plant { get; set; }
            public string s_MonthFrom { get; set; }
            public string s_MonthTo { get; set; }
            public string s_Status { get; set; }
            public string s_Docno { get; set; }
            public string s_Moldno { get; set; }


        }

        public class ViewMoldSearch
        {
            public string v_no { get; set; }
            public string v_arModel { get; set; }
            public string v_arMoldName { get; set; }
            public string v_arMold_Control { get; set; }
            public string v_mc_sizeMold { get; set; }
            public int v_mc_sizeMoldcount { get; set; }
            public string v_mcRange { get; set; }
            public string v_mcProdLine { get; set; }
            public string v_mcShortMa { get; set; }
            public string v_arLastShotQty { get; set; }
            public string v_arActual_StartDate { get; set; }
            public string v_arActual_EndDate { get; set; }
            public int? v_date { get; set; }
            public int? v_datePlus { get; set; }

        }

        //view table
        //public class ViewMoldData
        //{
        //    public string v_No { get; set; }
        //    public string v_moldelTypePart { get; set; }
        //    public string v_moldelNoName { get; set; }
        //    public string v_MoldControlNo { get; set; }//key 
        //    public string v_MoldSize { get; set; }
        //    public string v_Rank { get; set; }
        //    public string v_InjectionLine { get; set; }
        //    public string v_ShotMaintenance { get; set; }
        //    public string v_LastShot { get; set; }


        //}

        public class ViewRequest
        {
            public string v_month { get; set; }
            public string v_docno { get; set; }
            public string v_status { get; set; }
            public string v_Issue_date { get; set; }
            public string v_requestSheetNo { get; set; }
            public string v_no { get; set; }
            public string v_customer { get; set; }
            public string v_model { get; set; }
            public string v_moldNo_Name { get; set; }
            public string v_icsMoldNo { get; set; }
            public string v_icsMoldName { get; set; }
            public string v_moldControlNo { get; set; }
            public string v_rankMold { get; set; }
            public string v_icsInjectionNo { get; set; }
            public string v_moldActivityNo { get; set; }
            public int? v_mcShortMa { get; set; }
            public int? v_arLastShotQty { get; set; }
            public double v_moldWeight { get; set; }
            public string v_moldSize { get; set; }
            public string v_moldday { get; set; }
            public string v_injectionProductBy { get; set; }
            public string v_responsibility { get; set; }
            public string v_section { get; set; }
            public string v_InjectionBy { get; set; }

            public string v_planDmStartDate { get; set; }
            public string v_planDmEndDate { get; set; }

            public string v_planLeStartDate { get; set; }
            public string v_planLeEndDate { get; set; }

            public string v_Change_LE_StartDate { get; set; }
            public string v_Change_LE_EndDate { get; set; }

            public string v_ActualStartDate { get; set; }
            public string v_ActualEndDate { get; set; }

            public string v_arplant { get; set; }
            public string v_Remark { get; set; }
            public string v_fileName { get; set; }
            public IFormFile v_fileName1 { get; set; }

            public string chkPLANNED { get; set; }

            public string v_Actual_Remark { get; set; }
            public string v_statusRequestNo { get; set; }

            //public HttpPostedFileBase SomeFile { get; set; }
        }

        public class ViewMoldData
        {
            public string v_no { get; set; }
            public string v_arModel { get; set; }
            public string v_arModelNo { get; set; }
            public string v_arMoldName { get; set; }
            public string v_arMold_Control { get; set; }
            public string v_arIcsNoInj { get; set; }
            public string v_mc_sizeMold { get; set; }
            public int v_mc_sizeMoldcount { get; set; }
            public string v_mcRange { get; set; }
            public string v_mcProdLine { get; set; }
            public string v_mcShortMa { get; set; }
            public string v_arLastShotQty { get; set; }

            public string v_Plan_DM_StartDate { get; set; }
            public string v_Plan_DM_EndDate { get; set; }

            //date lamp
            public string v_Plan_LE_StartDate { get; set; }
            public string v_Plan_LE_EndDate { get; set; }

           

            public int? v_date_LE_StartDate { get; set; }
            public int? v_datePlus_LE_StartDate { get; set; }

            public string v_Change_LE_StartDate { get; set; }
            public string v_Change_LE_EndDate { get; set; }
            public int? v_date_Change_LE_StartDate { get; set; }
            public int? v_datePlus_v_Change_LE_EndDate { get; set; }

            //date Actualc
            public string v_Actual_StartDate { get; set; }
            public string v_Actual_EndDate { get; set; }
            public int? v_date_Actual_StartDate { get; set; }
            public int? v_datePlus_Actual_EndDate { get; set; }


            public int? v_date { get; set; }
            public int? v_datePlus { get; set; }
            public int? v_dateend { get; set; }

            public string v_RequestNo { get; set; }
            public string v_ReqNostatus { get; set; }
            public string v_Remark { get; set; }
        }

        public class Viewemail
        {
            //public string empcode { get; set; }
            public string email { get; set; }
            //public string plant { get; set; }

        }

        public class ViewmmMastPlaning
        {

            public string mpDocumentNo { get; set; }
            public string mpMonth { get; set; }
            public string mpPlant { get; set; }
            public string mpFlow { get; set; }
            public int mpStep { get; set; }
            public string mpStatus { get; set; }
            public string mpdateSent { get; set; }
            public string mpEmpcode_Issue { get; set; }
            public string mpName_Issue { get; set; }
            public string mpIssueDate { get; set; }
            public string mpIssueDept { get; set; }
            public string mpEmpcode_Approve { get; set; }
            public string mpName_Approve { get; set; }
        }
        public class ViewSearchManual
        {

            public string mpDocumentNo { get; set; }
            public string mpMoldNo { get; set; }
            public string mpMonth { get; set; }
            public string mpPlant { get; set; }
            public string mpFlow { get; set; }
            public int mpStep { get; set; }
            public string mpStatus { get; set; }
            public string mpdateSent { get; set; }
            public string mpEmpcode_Issue { get; set; }
            public string mpName_Issue { get; set; }
            public string mpIssueDate { get; set; }
            public string mpIssueDept { get; set; }
            public string mpEmpcode_Approve { get; set; }
            public string mpName_Approve { get; set; }
        }
        public class ViewCreateMold
        {
            public string v_activity { get; set; }
            public string v_Month { get; set; }
            public string V_Dept { get; set; }
          
        }

    }
}
