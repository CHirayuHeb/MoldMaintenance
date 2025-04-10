using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MAINTENANCE_MOLD.Models.Table
{
    public class Tb_Mold
    {
        [Table("Login")]
        public class MoldLogin
        {
            [Key]
            public string UserId { get; set; }
            public string Password { get; set; }
            public string Program { get; set; }
            public string Version { get; set; }
            public string Empcode { get; set; }
            public string Permission { get; set; }

        }


        [Table("mtMaster_Mold_Control")]
        public class mtMaster_Mold_Control
        {

            [Key]
            public int mcNo { get; set; }
            public string mcMold_Control { get; set; }
            public string mcActivityType { get; set; }
            public string mcPlant { get; set; }
            public string mcVendor { get; set; }
            public string mcVenName { get; set; }
            public string mcAssetNo { get; set; }
            public string mcStatus { get; set; }
            public string mcRange { get; set; }
            public string mcIcs_Injection_R { get; set; }
            public string mcIcs_Injection_L { get; set; }
            public string mcPartname_Injection_R { get; set; }
            public string mcPartname_Injection_L { get; set; }
            public string mcIcs_Product_Drawing_R { get; set; }
            public string mcPartname_R { get; set; }
            public string mcIcs_Product_Drawing_L { get; set; }
            public string mcPartname_L { get; set; }
            public string mcModel { get; set; }
            public string mcAddress { get; set; }
            public string mcAddress_1 { get; set; }
            public double mcMold_Weight { get; set; }
            public double mcMold_Size_X { get; set; }
            public double mcMold_Size_Y { get; set; }
            public double mcMold_Size_Z { get; set; }
            public string mcMoldname { get; set; }
            public string mcMC { get; set; }
            public string mcCavity { get; set; }
            public string mcCUS { get; set; }
            public string mcT0Date { get; set; }
            public string mcIcs_Resin { get; set; }
            public string mcIcsname_Resin { get; set; }
            public string mcIcs_Mold { get; set; }
            public string mcIcs_Moldname { get; set; }
            public string mcMass_Production_Date { get; set; }
            public string mcMass_Production { get; set; }
            public string mcNew_Mold_Order_Date { get; set; }
            public string mcLedger_Number { get; set; }
            public string mcSize_Mold { get; set; }
            public string mcRemark { get; set; }
            public string mcMore_Then19_Year { get; set; }
            public string mcIssueBy { get; set; }
            public string mcUpdateBy { get; set; }
            public int? mcShortMa { get; set; }
            public int? mcLastShotMa { get; set; }
            public string mcLastCleaning { get; set; }
            public string mcMTControl { get; set; }
            public string mcPlantCode { get; set; }
            public string mcProdLine { get; set; }
        }

        [Table("mmMaActual_Risk")]
        public class mmMaActual_Risk
        {
            [Key]
            public string arMonth { get; set; }
            public string arPlant { get; set; }
            public string arLine { get; set; }
            public string arMoldNo { get; set; }
            public string arMoldName { get; set; }
            public string arModel { get; set; }
            //public string arMold_Control { get; set; }
            public string arIcsNoInj { get; set; }
            public string arActual_StartDate { get; set; }
            public string arActual_EndDate { get; set; }
            public int arLastShotQty { get; set; }
            public string arIssueBy { get; set; }
            public string arUpdateBy { get; set; }


        }

        public class View_mmMaActual_Risk
        {
            [Key]
            public string v_no { get; set; }
            public string v_arModel { get; set; }
            public string v_arMoldName { get; set; }
            public string v_arMold_Control { get; set; }
            public string v_mc_sizeMold { get; set; }
            public string v_mc_sizeMoldcount { get; set; }
            public string v_mcRange { get; set; }
            public string v_mcProdLine { get; set; }
            public string v_mcShortMa { get; set; }
            public string v_arLastShotQty { get; set; }



        }

        [Table("mmMast_SizeCleaning")]
        public class mmMast_SizeCleaning
        {
            [Key]
            public string msMold_Size { get; set; }
            public int msDay_Cleaning { get; set; }
        }

        [Table("mmRunDocument")]
        public class mmRunDocument
        {
            [Key]
            public int? rmRunNo { get; set; }
            public string rmPlant { get; set; }
            public string rmYear { get; set; }
            public string rmGroup { get; set; }
            public string rmIssueBy { get; set; }
            public string rmUpdateBy { get; set; }
        }

        [Table("mmMastFlowApprove")]
        public class mmMastFlowApprove
        {
            [Key]
            public string mfFlowNo { get; set; }
            public int mfStep { get; set; }
            public string mfDept { get; set; }
            public string mfSubject { get; set; }
            public string mfFlowName { get; set; }
            public string mfPermission { get; set; }
            public string mfCC { get; set; }
            public string mfIssueBy { get; set; }
            public string mfUpdateBy { get; set; }
        }

        [Table("mmHistoryApproved", Schema = "dbo")]
        public class mmHistoryApproved
        {
            [Key]
            public int? htNo { get; set; }
            public string htDocumentNo { get; set; }
            public int htStep { get; set; }
            public string htStatus { get; set; }
            public string htFrom { get; set; }
            public string htTo { get; set; }
            public string htCC { get; set; }
            public string htDate { get; set; }
            public string htTime { get; set; }
            public string htRemark { get; set; }
            public string htCCDept { get; set; }
        }

        [Table("mmMastPlaning")]
        public class mmMastPlaning
        {
            [Key]
            public string mpDocumentNo { get; set; }
            public string mpMonth { get; set; }
            public string mpPlant { get; set; }
            public string mpFlow { get; set; }
            public int mpStep { get; set; }
            public string mpStatus { get; set; }
            public string mpEmpcode_Issue { get; set; }
            public string mpName_Issue { get; set; }
            public string mpIssueDate { get; set; }
            public string mpIssueDept { get; set; }
            public string mpEmpcode_Approve { get; set; }
            public string mpName_Approve { get; set; }
        }

        [Table("mmDetailPlaning", Schema = "dbo")]
        public class mmDetailPlaning
        {

            public string dpDocumentNo { get; set; }
            [Key]
            public string dpMoldNo { get; set; }
            public string dpLine { get; set; }
            public string dpMoldName { get; set; }
            public string dpModel { get; set; }
            public string dpIcsNoInj { get; set; }
            public string dpPlan_DM_StartDate { get; set; }
            public string dpPlan_DM_EndDate { get; set; }
            public string dpPlan_LE_StartDate { get; set; }
            public string dpPlan_LE_EndDate { get; set; }
            public string dpChange_LE_StartDate { get; set; } //date change
            public string dpChange_LE_EndDate { get; set; } //date change
            
            public string dpActual_StartDate { get; set; }
            public string dpActual_EndDate { get; set; }
            public int? dpLastShotQty { get; set; }
            public string dpRequestNo { get; set; }
            public string dpRemark { get; set; }
           
            public string dpFileName { get; set; }
            public string dpStatus { get; set; }
            public string dpStep { get; set; }
            public string dpEmpcode_Issue { get; set; }
            public string dpName_Issue { get; set; }
            public string dpIssueDate { get; set; }
            public string dpIssueDept { get; set; }
            public string dpEmpcode_Approve { get; set; }
            public string dpName_Approve { get; set; }
            public string dpPlan_DM_Month { get; set; }
            public string dpActual_Remark { get; set; }
        }


        [Table("mmMastUserApprove", Schema = "dbo")]
        public class mmMastUserApprove
        {
            [Key]
            public int? muNo { get; set; }
            public string muEmpCode { get; set; }
            public string muFlowNo { get; set; }
            public string muDeptCode { get; set; }

            public string muOperator { get; set; }
            public string muCheck { get; set; }
            public string muApprove { get; set; }
            public string muCC { get; set; }
            public string muPosition { get; set; }
            public string muIssueBy { get; set; }
            public string muUpdateBy { get; set; }


        }
    }
}
