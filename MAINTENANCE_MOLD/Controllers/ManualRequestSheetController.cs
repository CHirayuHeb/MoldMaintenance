using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MAINTENANCE_MOLD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using static MAINTENANCE_MOLD.Models.Table.Tb_Hrms;
using static MAINTENANCE_MOLD.Models.Table.Tb_Mold;
using static MAINTENANCE_MOLD.Models.Table.Tb_Search;


namespace MAINTENANCE_MOLD.Controllers
{
    public class ManualRequestSheetController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public string _Location = @"\\thsweb\\MAINTENANCE_MOLD\\";
        public ManualRequestSheetController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
        }

        public IActionResult Index(Class @class)
        {
            return View("Index", @class);
            //return View();
        }
        public IActionResult NoData(Class @class)
        {
            return View("NoData", @class);
            //return View();
        }

        public IActionResult LoadData(string vMoldActivity, string vdep, string vMonth, Class @class, string Docno)
        {
            string s_nickname = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            string s_Dep = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            vMoldActivity = vMoldActivity != null ? vMoldActivity : @class._ViewCreateMold.v_activity;
            //Docno = Docno != null ? Docno : @class.ViewRequest.v_docno;
            ViewRequest v_ViewRequest = new ViewRequest();
            mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
            _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpDocumentNo == Docno).FirstOrDefault();
            vMonth = vMonth != null ? vMonth : @class._ViewCreateMold.v_Month;

            int chk_mold = _MOLD.mtMaster_Mold_Control.Where(x => x.mcActivityType == vMoldActivity).Count();
            if (chk_mold>0)
            {
                var result_mmMast_SizeCleaning = from mm in _MOLD.mmMast_SizeCleaning
                                                 select mm;
                var list_mmMast_SizeCleaning = result_mmMast_SizeCleaning.ToList();

                var list_join = (from x in _MOLD.mtMaster_Mold_Control
                                 where x.mcActivityType == vMoldActivity
                                 select new
                                 {
                                     v_status = "",//_mmDetailPlaning == null ? "0" : "1",
                                     v_month = vMonth,//, @class._ViewCreateMold.v_Month,//mm.arMonth,
                                     v_docno = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpDocumentNo  : "",
                                     v_requestSheetNo = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpRequestNo == null ? "" : _mmDetailPlaning.dpRequestNo : "",
                                     v_Issue_date = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpIssueDate : DateTime.Now.ToString("yyyy/MM/dd"),
                                     v_customer = x.mcCUS,//f != null ? f.mcCUS : "",//yes
                                     v_model = x.mcModel,//mm.arModel,  //mcModel
                                     v_modelNo = x.mcModel,// f != null ? f.mcModel : "",//yes
                                     v_moldNo_Name = x.mcMoldname,//f != null ? f.mcMoldname : "", //yes
                                     v_icsMoldNo = x.mcIcs_Mold,//f != null ? f.mcIcs_Mold : "", //yes
                                     v_icsMoldName = x.mcIcs_Moldname,// f != null ? f.mcIcs_Moldname : "", //yes
                                     v_moldControlNo = x.mcMold_Control,//f != null ? f.mcMold_Control : "", //yes
                                     v_rankMold = x.mcRange,// f != null ? f.mcRange : "", //yes
                                     v_sizeMold = x.mcSize_Mold,// f != null ? f.mcRange : "", //yes
                                     v_icsInjectionNo = x.mcIcs_Injection_R + "," + x.mcIcs_Injection_L,//f != null ? f.mcIcs_Injection_R + "," + f.mcIcs_Injection_L : "",//yes
                                     v_moldActivityNo = x.mcActivityType,//f != null ? f.mcActivityType : "",//yes

                                     v_mcShortMa = x.mcShortMa,//f != null ? f.mcShortMa : 0, //int yes
                                     v_arLastShotQty = x.mcLastShotMa, // mm.arLastShotQty, //int yes

                                     v_moldWeight = x.mcMold_Weight,//f != null ? f.mcMold_Weight : 0, // yes
                                     v_moldSize = x.mcMold_Size_X.ToString() + " x " + x.mcMold_Size_Y.ToString() + " x " + x.mcMold_Size_Z.ToString(),//f != null ? f.mcMold_Size_X.ToString() + " x " + f.mcMold_Size_Y.ToString() + " x " + f.mcMold_Size_Z.ToString() : "0", // x*y*z  yes
                                     v_injectionProductBy = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? "" : _mmDetailPlaning.dpLine : "", //yes
                                     v_responsibility = s_nickname,//_mmDetailPlaning != null ?  _mmDetailPlaning.dpName_Issue: s_nickname, //yes
                                     v_section = s_Dep,//_mmDetailPlaning != null ? _mmDetailPlaning.dpIssueDept == null ? s_Dep : _mmDetailPlaning.dpIssueDept : "",//yes
                                     v_InjectionBy = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? "" : _mmDetailPlaning.dpLine : "",//yes
                                     v_planDmStartDate = "",// _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_StartDate : "",
                                     v_planDmEndDate = "",//_mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_EndDate : "",
                                     v_planLeStartDate = "",//_mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_StartDate : "",
                                     v_planLeEndDate = "",//_mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_EndDate : "",
                                     v_Change_LE_StartDate = "",// _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_StartDate == null ? "" : _mmDetailPlaning.dpChange_LE_StartDate : "",
                                     v_Change_LE_EndDate = "",// _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_EndDate == null ? "" : _mmDetailPlaning.dpChange_LE_EndDate : "",
                                     v_ActualStartDate = "",//_mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_StartDate : "",
                                     v_ActualEndDate = "",//_mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_EndDate : "",

                                     v_Remark = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpRemark : "",
                                     v_arplant = x.mcPlantCode,//mm.arPlant,
                                     v_fileName = "",//_mmDetailPlaning != null ? _mmDetailPlaning.dpFileName : "",

                                     v_Actual_Remark = "",// _mmDetailPlaning != null ? _mmDetailPlaning.dpActual_Remark : "", //add new
                                 }).ToList();






                var innerJoin = (from x in list_join//_mmMaActual_Risk//result_MaActual_Risk // outer sequence
                                 join mt in list_mmMast_SizeCleaning//_mtMaster_Mold_Control//list_mtMaster_Mold_Control //inner sequence  // on pa.paMold_Control equals mc.mcMold_Control // key selector 
                                 on x.v_sizeMold equals mt.msMold_Size // key selector 
                                 into d2
                                 from f in d2.DefaultIfEmpty()
                                 select new
                                 {
                                     v_status = _mmDetailPlaning != null ? _mmDetailPlaning.dpStatus == null ? "" : _mmDetailPlaning.dpStatus : "",
                                     v_month = _mmDetailPlaning != null ? _mmDetailPlaning.dpPlan_DM_Month == null ? x.v_month : _mmDetailPlaning.dpPlan_DM_Month : x.v_month,
                                     v_docno = _mmDetailPlaning != null ? _mmDetailPlaning.dpDocumentNo == null ? "" : _mmDetailPlaning.dpDocumentNo : "",
                                     v_requestSheetNo = _mmDetailPlaning != null ? _mmDetailPlaning.dpRequestNo == null ? "" : _mmDetailPlaning.dpRequestNo : "",
                                     v_Issue_date = _mmDetailPlaning != null ? _mmDetailPlaning.dpIssueDate == null ? DateTime.Now.ToString("yyyy/MM/dd") : _mmDetailPlaning.dpIssueDate : DateTime.Now.ToString("yyyy/MM/dd"),
                                     v_customer = x.v_customer,//f != null ? f.mcCUS : "",//yes
                                     v_model = x.v_model,//mm.arModel,  //mcModel
                                     v_modelNo = x.v_modelNo,// f != null ? f.mcModel : "",//yes
                                     v_moldNo_Name = x.v_moldNo_Name,//f != null ? f.mcMoldname : "", //yes
                                     v_icsMoldNo = x.v_icsMoldNo,//f != null ? f.mcIcs_Mold : "", //yes
                                     v_icsMoldName = x.v_icsMoldName,// f != null ? f.mcIcs_Moldname : "", //yes
                                     v_moldControlNo = x.v_moldControlNo,//f != null ? f.mcMold_Control : "", //yes
                                     v_rankMold = x.v_rankMold, //yes
                                     v_rankMoldDay = f != null ? f.msDay_Cleaning.ToString() : "0", //yes
                                     v_icsInjectionNo = x.v_icsInjectionNo,//f != null ? f.mcIcs_Injection_R + "," + f.mcIcs_Injection_L : "",//yes
                                     v_moldActivityNo = x.v_moldActivityNo,//f != null ? f.mcActivityType : "",//yes

                                     v_mcShortMa = x.v_mcShortMa,//f != null ? f.mcShortMa : 0, //int yes
                                     v_arLastShotQty = x.v_arLastShotQty, // mm.arLastShotQty, //int yes

                                     v_moldWeight = x.v_moldWeight,//f != null ? f.mcMold_Weight : 0, // yes
                                     v_moldSize = x.v_moldSize.ToString(),//f != null ? f.mcMold_Size_X.ToString() + " x " + f.mcMold_Size_Y.ToString() + " x " + f.mcMold_Size_Z.ToString() : "0", // x*y*z  yes
                                     v_injectionProductBy = _mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? "" : _mmDetailPlaning.dpLine : "", //yes
                                     v_responsibility = _mmDetailPlaning != null ? _mmDetailPlaning.dpName_Issue == null ? x.v_responsibility : _mmDetailPlaning.dpName_Issue : x.v_responsibility, //yes
                                     v_section = _mmDetailPlaning != null ? _mmDetailPlaning.dpIssueDept == null ? x.v_section : _mmDetailPlaning.dpIssueDept : x.v_section,//yes
                                     v_InjectionBy = _mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? x.v_InjectionBy : _mmDetailPlaning.dpLine : x.v_InjectionBy,//yes
                                     v_planDmStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_StartDate : "",
                                     v_planDmEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_EndDate : "",
                                     v_planLeStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_StartDate : "",
                                     v_planLeEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_EndDate : "",
                                     v_Change_LE_StartDate = _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_StartDate == null ? "" : _mmDetailPlaning.dpChange_LE_StartDate : "",
                                     v_Change_LE_EndDate = _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_EndDate == null ? "" : _mmDetailPlaning.dpChange_LE_EndDate : "",
                                     v_ActualStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_StartDate : "",
                                     v_ActualEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_EndDate : "",
                                     v_Remark = _mmDetailPlaning != null ? _mmDetailPlaning.dpRemark : "",
                                     v_arplant = x.v_arplant,
                                     v_fileName = _mmDetailPlaning != null ? _mmDetailPlaning.dpFileName : x.v_fileName,
                                     v_Actual_Remark = _mmDetailPlaning != null ? _mmDetailPlaning.dpActual_Remark : "", //add new
                                 }).ToList();




                for (int i = 0; i < 1; i++)
                {
                    v_ViewRequest.v_month = innerJoin[i].v_month.ToString(); //month
                    v_ViewRequest.v_docno = innerJoin[i].v_docno.ToString();
                    v_ViewRequest.v_status = innerJoin[i].v_status.ToString();
                    v_ViewRequest.v_requestSheetNo = innerJoin[i].v_docno.ToString();   // @class.ViewRequest != null ? @class.ViewRequest.v_requestSheetNo : list_join[i].v_requestSheetNo.ToString();
                    v_ViewRequest.v_Issue_date = innerJoin[i].v_Issue_date.ToString();
                    v_ViewRequest.v_no = i.ToString();
                    v_ViewRequest.v_customer = innerJoin[i].v_customer.ToString();
                    v_ViewRequest.v_model = innerJoin[i].v_modelNo.ToString(); //  list_join[i].v_model.ToString();
                    v_ViewRequest.v_moldNo_Name = innerJoin[i].v_moldNo_Name.ToString();
                    v_ViewRequest.v_icsMoldNo = innerJoin[i].v_icsMoldNo.ToString();
                    v_ViewRequest.v_icsMoldName = innerJoin[i].v_icsMoldName.ToString();
                    v_ViewRequest.v_moldControlNo = innerJoin[i].v_moldControlNo.ToString();
                    v_ViewRequest.v_rankMold = innerJoin[i].v_rankMold.ToString();
                    v_ViewRequest.v_moldday = innerJoin[i].v_rankMoldDay.ToString(); ;
                    v_ViewRequest.v_icsInjectionNo = innerJoin[i].v_icsInjectionNo.ToString();
                    v_ViewRequest.v_moldActivityNo = innerJoin[i].v_moldActivityNo.ToString();
                    v_ViewRequest.v_mcShortMa = innerJoin[i].v_mcShortMa;
                    v_ViewRequest.v_arLastShotQty = innerJoin[i].v_arLastShotQty;
                    v_ViewRequest.v_moldWeight = innerJoin[i].v_moldWeight; //double
                    v_ViewRequest.v_moldSize = innerJoin[i].v_moldSize;
                    v_ViewRequest.v_injectionProductBy = innerJoin[i].v_injectionProductBy.ToString();
                    v_ViewRequest.v_responsibility = innerJoin[i].v_responsibility.ToString();
                    v_ViewRequest.v_section = innerJoin[i].v_section.ToString();
                    v_ViewRequest.v_planDmStartDate = innerJoin[i].v_planDmStartDate.ToString();
                    v_ViewRequest.v_planDmEndDate = innerJoin[i].v_planDmEndDate.ToString();
                    v_ViewRequest.v_planLeStartDate = innerJoin[i].v_planLeStartDate.ToString();
                    v_ViewRequest.v_planLeEndDate = innerJoin[i].v_planLeEndDate.ToString();
                    v_ViewRequest.v_Change_LE_StartDate = innerJoin[i].v_Change_LE_StartDate.ToString();
                    v_ViewRequest.v_Change_LE_EndDate = innerJoin[i].v_Change_LE_EndDate.ToString();
                    v_ViewRequest.v_ActualStartDate = innerJoin[i].v_ActualStartDate.ToString();
                    v_ViewRequest.v_ActualEndDate = innerJoin[i].v_ActualEndDate.ToString();


                    v_ViewRequest.chkPLANNED = innerJoin[i].v_planDmStartDate.ToString() == innerJoin[i].v_planLeStartDate.ToString() ? "1" : "0"; //check plan

                    v_ViewRequest.v_arplant = innerJoin[i].v_arplant.ToString();
                    v_ViewRequest.v_Remark = innerJoin[i].v_Remark != null ? innerJoin[i].v_Remark.ToString() : ""; //input remake
                    v_ViewRequest.v_fileName = innerJoin[i].v_fileName != null ? innerJoin[i].v_fileName.ToString() : ""; //input remake

                    v_ViewRequest.v_Actual_Remark = innerJoin[i].v_Actual_Remark != null ? innerJoin[i].v_Actual_Remark.ToString() : ""; //chirayu add
                                                                                                                                         //ViewRequest.v_Actual_Remark

                    // v_ViewRequest.v_
                    // v_dpRemark
                }

                @class.ViewRequest = v_ViewRequest;
                ViewBag.status = v_ViewRequest.v_status;// status
                ViewBag.step = _mmDetailPlaning != null ? int.Parse(_mmDetailPlaning.dpStep).ToString() : "";// status
                ViewBag.DocNo = v_ViewRequest.v_docno;
                return View("Index", @class);
            }
            else
            {
                return View("NoData", @class);
            }



            // return Views("RequestPage", "RequestSheet", @class);

        }
        [HttpPost]
        public ActionResult ActionEvent(string submitButton, Class @class, string v_class)
        {
            var filelist = HttpContext.Request.Form.Files;
            // @classs._ViewSearch = new ViewSearch();
            //ViewSearch _ViewSearch = (ViewSearch)TempData["_ViewSearch"];
            string[] filename;

            switch (submitButton)
            {
                case "SEARCH":
                    return (View());
                case "SAVE":
                    //SaveData(@class);
                    return View("RequestPage", @class);
                //return (View());
                //return (SaveData(@class));
                case "More":
                case "saveFile":
                    filename = savefile(@class, @class.ViewRequest.v_fileName1);
                    ViewRequest _viewRequest1 = new ViewRequest();
                    _viewRequest1.v_fileName = filename[0];
                    return View("RequestPage", @class);
                // return (View());
                case "btnsaveMailfile":

                    filename = savefile(@class, @class.ViewRequest.v_fileName1);
                    ViewRequest _viewRequest = new ViewRequest();
                    _viewRequest.v_fileName = filename[0];
                    return RedirectToAction("SearchPage", "Search", @class);
                // return View("RequestPage", @class);
                default:
                    return (View());
            }
            //return PartialView("SendMail");
        }
        public string[] savefile(Class @class, IFormFile file)
        {



            string v_name = "";
            string fileName = "";


            try
            {
                if (file is null)
                {
                    v_name = "";
                }
                else
                {
                    fileName = file.FileName;//System.IO.Path.GetExtension(file.FileName).ToLower();
                    string filePath = _Location + fileName;
                    var fileLocation = new FileInfo(filePath);
                    //filePaths.Add(filePath);
                    if (!Directory.Exists(filePath))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }

                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
                            _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldNo == @class.ViewRequest.v_moldActivityNo && x.dpDocumentNo == @class.ViewRequest.v_docno).FirstOrDefault();
                            _mmDetailPlaning.dpFileName = file.FileName;
                            _MOLD._mmDetailPlaning.Update(_mmDetailPlaning);
                            _MOLD.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        catch (Exception e)
                        {

                            dbContextTransaction.Rollback();
                        }
                    }



                    v_name = fileName;
                }
            }
            catch (Exception e)
            {
                v_name = "";
            }


            string[] returnVal = { v_name };
            //string[] returnVal = { "1", "" };
            return returnVal;
        }

        [HttpPost]
        public JsonResult History(Class @class, string vdate)// string mode, string vplant, string vdate)
        {
           // id = id != null ? id : "";
            var mode = "edit";
            var vplant = @class.ViewRequest != null ? @class.ViewRequest.v_arplant : "";
            var id = @class.ViewRequest != null ? @class.ViewRequest.v_requestSheetNo : "";
            //var vdate = @class.ViewRequest != null ? @class.ViewRequest.v_ : "";
            //id = "MP300124001";
            string partialUrl = Url.Action("SendMail", "ManualRequestSheet", new { docno = id, mode, vplant = vplant, vdate = vdate });
            List<mmHistoryApproved> _listHistory = new List<mmHistoryApproved>();

            if (id != null || id != "")
            {
                string hisId = id;
                //mmHistoryApproved _mmHistoryApproved = _MOLD.mmHistoryApproved.Find(hisId);
                _listHistory = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == hisId).ToList();

                return Json(new { status = "hasHistory", listHistory = _listHistory, partial = partialUrl });

                //return Json(new { status = "hasHistory", listHistory, partial = partialUrl });
            }
            else
            {
                return Json(new { status = "empty", listHistory = _listHistory, partial = partialUrl });
            }

        }
        public ActionResult SendMail(string docno, string mode, string vplant, string vdate)
        {
            string v_user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string v_userdep = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            var v_ccemail = "";
            Class model = new Class();
            var v_muDeptCode = "";
            mmHistoryApproved _mmHistoryApproved = new mmHistoryApproved();
            mmMastUserApprove _mmMastUserApprove = new mmMastUserApprove();
            List<Viewemail> _Viewemail = new List<Viewemail>();

            //mmDetailPlaning _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == docno).FirstOrDefault();
            //v_muDeptCode = _mmDetailPlaning != null && _mmDetailPlaning.dpStep == "1" ? v_muDeptCode = "DMM" : v_muDeptCode = vplant;
            //var v_mmMastUserApprove = _mmDetailPlaning != null ? 
            //                                _mmDetailPlaning.dpStep == "1" ? 
            //                                        _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muEmpCode == _mmDetailPlaning.dpEmpcode_Issue).ToList() 
            //                                        : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList() 

            //                                : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList();

            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == docno).FirstOrDefault();

            v_muDeptCode = _mmMastPlaning == null || _mmMastPlaning.mpStep == 0 ? v_muDeptCode = "DMM" : v_muDeptCode = vplant;
            var v_mmMastUserApprove = _mmMastPlaning == null || _mmMastPlaning.mpStep == 0 ?
                                        _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == v_muDeptCode).ToList() :
                                        _mmMastPlaning.mpStep == 1 ?
                                                _MOLD._mmMastUserApprove.Where(x => x.muEmpCode == _mmMastPlaning.mpEmpcode_Issue).ToList() :
                                                _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList()
                ;

            // v_muDeptCode = _mmMastPlaning != null && _mmMastPlaning.mpStep == 1 ? v_muDeptCode = "DMM" : v_muDeptCode = vplant;
            // var v_mmMastUserApprove = _mmMastPlaning != null ? _mmMastPlaning.mpStep == 1 ? _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muEmpCode == _mmMastPlaning.mpEmpcode_Issue).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList();

            //   ? _mmMastPlaning.mpStep == 1 ? _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == 'DMM' && x.muEmpCode == _mmMastPlaning.mpEmpcode_Issue).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == v_muDeptCode).ToList() : v_muDeptCode = vplant;
            if (v_mmMastUserApprove.Count > 0)
            {
                for (int i = 0; i <= v_mmMastUserApprove.Count() - 1; i++)
                {
                    // var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(x => x.emEmail).FirstOrDefault();
                    //  var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(x => x.emEmail_M365).FirstOrDefault();
                    var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").FirstOrDefault(); //chg to m365
                    _Viewemail.Add(new Viewemail
                    {
                        email = v_emailFrom,
                        //empcode = v_mmMastUserApprove[i].muEmpCode.ToString(),
                        //plant = vplant,
                    });
                }
            }


            //htto
            List<Viewemail> _ViewemailTo = new List<Viewemail>();
            if (_mmMastPlaning != null)
            {
                if (_mmMastPlaning.mpStep == 0) //lamp=> dmm
                {
                    var v_Tomail = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muOperator == "main").ToList();
                    for (int i = 0; i <= v_Tomail.Count() - 1; i++)
                    {
                        var v_emailto = _IT.Email.Where(x => x.emEmpcode == v_Tomail[i].muEmpCode.ToString()).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").FirstOrDefault(); //chg to m365
                        _ViewemailTo.Add(new Viewemail
                        {
                            email = v_emailto,
                        });
                    }
                }
                else //dmm=> lamp
                {
                    var v_Tomail = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant && x.muOperator == "main").ToList();
                    for (int i = 0; i <= v_Tomail.Count() - 1; i++)
                    {
                        var v_emailto = _IT.Email.Where(x => x.emEmpcode == v_Tomail[i].muEmpCode.ToString()).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").FirstOrDefault(); //chg to m365
                        _ViewemailTo.Add(new Viewemail
                        {
                            email = v_emailto,
                        });
                    }
                }
            }
            else//lamp=> dmm
            {
                var v_Tomail = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muOperator == "main").ToList();
                for (int i = 0; i <= v_Tomail.Count() - 1; i++)
                {
                    var v_emailto = _IT.Email.Where(x => x.emEmpcode == v_Tomail[i].muEmpCode.ToString()).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").FirstOrDefault(); //chg to m365
                    _ViewemailTo.Add(new Viewemail
                    {
                        email = v_emailto,
                    });
                }
            }


            //cc
            if (_mmMastPlaning != null)
            {
                // if (_mmMastPlaning.mpStep == 0)
                //{
                var depcc = _MOLD._mmMastUserApprove.Where(x => x.muEmpCode == v_user).Select(x => x.muDeptCode).FirstOrDefault();
                var ccLamp = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == depcc).ToList();
                for (int i = 0; i <= ccLamp.Count() - 1; i++)
                {
                    var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == ccLamp[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                    v_ccemail += v_emailCCFrom + ",";
                }
                // }
                //else if (_mmMastPlaning.mpStep == 1)
                //{
                //    v_ccemail = "";
                //    for (int i = 0; i <= v_mmMastUserApprove.Count() - 1; i++)
                //    {
                //        var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                //        v_ccemail += v_emailCCFrom + ",";
                //    }
                //}
            }
            else
            {
                var depcc = _MOLD._mmMastUserApprove.Where(x => x.muEmpCode == v_user).Select(x => x.muDeptCode).FirstOrDefault();
                var ccLamp = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == depcc).ToList();
                for (int i = 0; i <= ccLamp.Count() - 1; i++)
                {
                    var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == ccLamp[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                    v_ccemail += v_emailCCFrom + ",";
                }
                //v_user
            }

            SelectList s_emailFrom = new SelectList(_Viewemail.Select(x => x.email).Distinct());
            SelectList s_emailTo = new SelectList(_ViewemailTo.Select(x => x.email).Distinct());
            // model._mmHistoryApproved.htFrom = "chirayu";
            //qaHistoryApproved _qaHistoryApproved = new qaHistoryApproved();

            var v_Step = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == docno).Select(x => x.htStep).FirstOrDefault();



            var v_Empcode = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == docno).Select(x => x.mpEmpcode_Issue).FirstOrDefault();
            if (v_Empcode == null)
            {
                v_Empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            }
            //var v_Issue = _IT.Email.Where(x => x.emEmpcode == v_Empcode).Select(x => x.emEmail).FirstOrDefault();
            var v_Issue = _IT.Email.Where(x => x.emEmpcode == v_Empcode).Select(x => x.emEmail_M365).FirstOrDefault();

            var v_From = _MOLD.mmHistoryApproved
              .Where(x => x.htDocumentNo == docno && x.htStep == v_Step)
              .GroupBy(x => x.htDocumentNo == docno && x.htStep == v_Step)
              .Select(x => x.OrderByDescending(t => t.htNo)
              .First());

            if (v_From.Count() == 0)
            {
                _mmHistoryApproved.htDocumentNo = docno;
                _mmHistoryApproved.htFrom = v_Issue;
                _mmHistoryApproved.htCC = v_Issue;


            }
            else
            {
                _mmHistoryApproved.htFrom = v_From.Select(x => x.htTo).First();
                if (_mmHistoryApproved.htFrom.ToString().Contains("@")) { }
                else
                {
                    // _mmHistoryApproved.htFrom = _IT.Email.Where(x => x.emEmpcode == _mmHistoryApproved.htFrom.ToString()).Select(x => x.emEmail).FirstOrDefault();
                    _mmHistoryApproved.htFrom = _IT.Email.Where(x => x.emEmpcode == _mmHistoryApproved.htFrom.ToString()).Select(x => x.emEmail_M365).FirstOrDefault();
                }
            }






            _mmHistoryApproved.htDocumentNo = docno;
            //_mmHistoryApproved.htTo = "";
            _mmHistoryApproved.htCC = v_ccemail;
            _mmHistoryApproved.htStep = v_Step;
            _mmHistoryApproved.htStatus = "";
            _mmHistoryApproved.htDate = "";
            _mmHistoryApproved.htTime = "";

            //if (mode == "edit")
            //{
            //    ViewBag.Mode = "edit";
            //}
            var v_status = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == docno).OrderByDescending(x => x.htStep).Select(x => x.htStatus).FirstOrDefault();
            ViewBag.vdocno = docno;
            ViewBag.vMode = "edit222";
            ViewBag.vplant = vplant;
            ViewBag.vdate = vdate;
            ViewBag.status = v_status;
            ViewBag.UserApprove = s_emailTo;// s_emailFrom;

            //return  PartialView("~/Views/RequestSheet/SendMailR.cshtml", _mmHistoryApproved);
            return PartialView("SendMailM", _mmHistoryApproved);

        }

        [HttpPost]
        public JsonResult SendMail_post(string vmode, string vplant, Class @class, mmHistoryApproved _mmHistoryApproved)
        {

            string config = "";
            string msg = "";
            string[] filename;
            vplant = @class.ViewRequest.v_arplant;
            string[] chkPermis;
            string[] chkCountPlan;



            chkPermis = chkPermission(@class, "Send");


            if (chkPermis[1].ToString() == "No")
            {

                msg = "You don't have permission to access";
                config = "E";
                ViewBag.Config = "E";
                TempData["Config"] = ViewBag.Config;
                TempData["Msg"] = ViewBag.Msg;

                return Json(new { c1 = config, c2 = msg });
            }

            //step
            int i_Step = 0;
            var v_subject = "";


            i_Step = @class.ViewRequest.v_docno != null ? _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == @class.ViewRequest.v_docno).Select(x => int.Parse(x.dpStep)).FirstOrDefault() : 0;


            if (_mmHistoryApproved.htTo != null || (_mmHistoryApproved.htTo == null && _mmHistoryApproved.htStatus == "Disapprove"))
            {
                if (_mmHistoryApproved.htStatus == "Approve")
                {
                    i_Step = i_Step + 1;//  _mmHistoryApproved.htStep + 1;

                    v_subject = _MOLD.mmMastFlowApprove.Where(x => x.mfStep == i_Step && x.mfFlowNo == "02").Select(x => x.mfSubject).FirstOrDefault();

                    _mmHistoryApproved.htStatus = v_subject;

                    //        //var v_Issue = _IT.rpEmail.Where(x => x.emEmpcode == _ViewlrAssetClaim.acEmpCodeReq).Select(x => x.emEmail).First();
                    //        //_qaHistoryApproved.htTo = v_Issue;

                    config = "S";
                }
                else
                {
                    config = "E";
                    msg = "Please input Status";
                }
            }
            else
            {
                config = "E";
                msg = "Please input e-mail.";
            }


            if (config == "S")
            {

                string[] getDocNO;
                //mmDetailPlanning
                //mmMastPlanning

                getDocNO = Save(vmode, @class, _mmHistoryApproved, i_Step);


                //savefile
                @class.ViewRequest.v_docno = getDocNO[1];
                filename = savefile(@class, @class.ViewRequest.v_fileName1);

                _mmHistoryApproved.htDocumentNo = getDocNO[1]; // getDocNO; //getDocNO[1];

                mmDetailPlaning _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == getDocNO[1]).FirstOrDefault();
                if (!(int.Parse(getDocNO[0]) > 0))
                {
                    return Json(new { c1 = "E", c2 = getDocNO[1] });

                }
                //clear mold
                if (i_Step == 2)
                {
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            mtMaster_Mold_Control _mtMaster_Mold_Control = new mtMaster_Mold_Control();
                            _mtMaster_Mold_Control = _MOLD.mtMaster_Mold_Control.Where(x => x.mcActivityType == @class.ViewRequest.v_moldActivityNo).FirstOrDefault();
                            _mtMaster_Mold_Control.mcLastShotMa = 0;
                            _mtMaster_Mold_Control.mcLastCleaning = @class.ViewRequest.v_ActualStartDate;   //DateTime.Now.ToString("yyyy/MM/dd");
                            _MOLD.mtMaster_Mold_Control.Update(_mtMaster_Mold_Control);
                            _MOLD.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }


                }

                var email = new MimeMessage();
                _mmHistoryApproved.htTo = _mmHistoryApproved.htTo != null ? _mmHistoryApproved.htTo.Split(" ")[0] : _mmHistoryApproved.htTo; //split ""
                var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emEmpcode).First();

                var v_emName_M365 = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emName_M365).First();
                var v_emNameFrom_M365 = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htFrom).Select(x => x.emName_M365).First();
                email.Subject = "Period Maintenance Mold Request ==> " + v_subject;

                //History
                using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                {
                    try
                    {
                        _mmHistoryApproved.htDocumentNo = getDocNO[1]; // getDocNO; //getDocNO[1];
                        _mmHistoryApproved.htStep = i_Step;
                        _mmHistoryApproved.htStatus = _mmHistoryApproved.htStatus;
                        _mmHistoryApproved.htFrom = _mmHistoryApproved.htFrom;
                        _mmHistoryApproved.htTo = _mmHistoryApproved.htTo;
                        _mmHistoryApproved.htCC = _mmHistoryApproved.htCC;
                        _mmHistoryApproved.htDate = DateTime.Now.ToString("yyyy/MM/dd");
                        _mmHistoryApproved.htTime = DateTime.Now.ToString("HH:mm:ss");
                        _mmHistoryApproved.htRemark = _mmHistoryApproved.htRemark; //no
                        _MOLD.mmHistoryApproved.Add(_mmHistoryApproved);
                        _MOLD.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                    }
                }


                //sendmail

                MailboxAddress Formmail365 = new MailboxAddress(v_emNameFrom_M365, _mmHistoryApproved.htFrom);
                email.From.Add(Formmail365);
                email.To.Add(MailboxAddress.Parse(_mmHistoryApproved.htTo));
                if (_mmHistoryApproved.htCC != null)
                {
                    string[] splitCC = _mmHistoryApproved.htCC.Split(',');
                    foreach (var i in splitCC)
                    {
                        if (i != " " & i != "")
                        {
                            email.Cc.Add(MailboxAddress.Parse(i));
                        }
                    }
                }
                var varifyUrl = "http://thsweb/MVCPublish/MoldMaintenance/Home/Login?mode=edit&MDocumentNo=" + getDocNO[1] + "&MoldNo=" + @class.ViewRequest.v_moldNo_Name + "&UserID=" + v_ApproveBy + "&Plant=" + vplant + "&Date=" + @class.ViewRequest.v_month;
                //var varifyUrl = "http://thsweb/MVCPublish/MoldMaintenance/Home/Login?mode=edit&MDocumentNo=" + getDocNO[1] + "&MoldNo=" + @class.ViewRequest.v_moldNo_Name + "&UserID=";
                var bodyBuilder = new BodyBuilder();
                string EmailBody = "";
                //    //var image = bodyBuilder.LinkedResources.Add(@"E:\01_My Document\02_Project\_2023\1. PartTransferUnbalance\PartTransferUnbalance\wwwroot\images\btn\OK.png");
                EmailBody = $"<div>" +
                $"<B>PERIOD MAINTENANCE MOLD MANUAL REQUEST : </B>" + "" + "<br>" +
                $"Document No : " + "" + getDocNO[1] + "" + "<br>" +
                $"Mold Name : " + "" + @class.ViewRequest.v_model + "" + "<br>" +
                $"Plant : " + "" + vplant + "" + "<br>" +
                $"Month : " + "" + @class.ViewRequest.v_month + "" + "<br>" +
                $"Status :" + "" + " " + _mmHistoryApproved.htStatus.ToString() + "<br>" +
                $"<a href='" + varifyUrl + "'> More Detail" +
                // $"<img src = 'http://thsweb/MVCPublish/QA_APPROVAL_REQUEST/images/btn/mail1.png' alt = 'HTML tutorial' style = 'width: 50px; height: 50px;'>" +
                $"</a>" +
                $"</div>";

                bodyBuilder.HtmlBody = string.Format(EmailBody);
                email.Body = bodyBuilder.ToMessageBody();

                // send email
                var smtp = new SmtpClient();
                //smtp.Connect("10.200.128.12");
                smtp.Connect("203.146.237.138");
                smtp.Send(email);
                smtp.Disconnect(true);



            }
            else
            {

                //if (config == "E")
                //{
                config = "E";
                ViewBag.Config = "E";
                TempData["Config"] = ViewBag.Config;
                TempData["Msg"] = ViewBag.Msg;

                //return Json(new { c1 = config, c2 = msg });
                //}

            }
            //config = "S";
            return Json(new { c1 = config, c2 = msg });
        }

        public string[] chkPermission(Class @class, string mode)
        {
            var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value;
            var doc = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value; //doc no edit
            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Actor)?.Value;//user edit
            var dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;//dept edit
            var userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value; //emp login
            var s_Docno = @class.ViewRequest.v_requestSheetNo;
            var s_PlanDocno = @class.ViewRequest.v_docno != null ? @class.ViewRequest.v_docno : "";

            var v_per = "";
            var m_per = "";
            try
            {
                //create new
                //wait 
                //finish
                if (s_Docno == null)
                {
                    //if (v_empcreate != userlogin)
                    //{
                    //    v_per = "No";
                    //    m_per = "You don't have permission to access'";
                    //}

                    //if (vPermission != "admin")
                    //{
                    //    v_per = "No";
                    //    m_per = "You don't have permission to access'";
                    //}
                }
                else
                {
                    int v_Step = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == s_PlanDocno).Select(x => x.mpStep).First();
                    var v_mpEmpcode_Issue = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == s_PlanDocno).Select(x => x.mpEmpcode_Issue).First();
                    var v_mpEmpcode_Approve = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == s_PlanDocno).Select(x => x.mpEmpcode_Approve).First();
                    //step 0


                    if (v_Step == 0)
                    {
                        if (v_mpEmpcode_Issue != userlogin)
                        {
                            v_per = "No";
                            m_per = "You don't have permission to access'";
                        }
                    }
                    else
                    {
                        var _mmHistoryApproved = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_PlanDocno && x.htStep == v_Step).Select(x => x.htFrom).First();
                        var v_empcreate = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved).Select(x => x.emEmpcode).First();
                        //step 1
                        if (v_Step == 1)
                        {
                            if (v_mpEmpcode_Approve != userlogin)
                            {
                                v_per = "No";
                                m_per = "You don't have permission to access'";
                            }
                        }
                        else if (v_Step == 2)
                        {
                            v_per = "No";
                            m_per = "You don't have permission to access'";
                        }

                    }


                }

                string[] returnVal = { "1", v_per, };
                //string[] returnVal = { "1", "" };
                return returnVal;
            }
            catch (Exception ex)
            {
                string[] returnVal = { "1", "No" };
                return returnVal;
            }

        }
        public string[] Save(string mode, Class @class, mmHistoryApproved _mmHistoryApproved, int i_Step)
        {
            string s_issue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " - " + HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_name = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            string s_dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value; // dep
            string s_issuedate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            //rmPlant = vRundoc[1].Substring(2, 4),
            //            rmYear = vRundoc[1].Substring(6, 2),
            var s_plant = @class.ViewRequest.v_arplant;
            var s_date = @class.ViewRequest.v_month;// @class.ViewRequest.v_requestSheetNo.Substring(6, 2);
            var s_Docno = @class.ViewRequest.v_requestSheetNo;


            var rundoc = "";
            int runNo = 0;


            var v_htTo = _mmHistoryApproved.htTo != null ? _mmHistoryApproved.htTo.Split(" ")[0] : _mmHistoryApproved.htTo; //split ""
            var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == v_htTo).Select(x => x.emEmpcode).First();
            ViewAccEMPLOYEE acc = _HRMS.AccEMPLOYEE.FirstOrDefault(s => s.EMP_CODE == v_ApproveBy);


            try
            {
                var v_subject = _MOLD.mmMastFlowApprove.Where(x => x.mfStep == i_Step && x.mfFlowNo == "02").Select(x => x.mfSubject).FirstOrDefault();
                if (s_Docno != null)
                {
                    runNo = int.Parse(s_Docno.Substring(8, 3));
                    rundoc = s_Docno;
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            //update

                            //_mmDetailPlaning
                            var _mmDetailPlaning = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpDocumentNo == rundoc);
                            _mmDetailPlaning.dpActual_StartDate = @class.ViewRequest.v_ActualStartDate != null ? @class.ViewRequest.v_ActualStartDate : "";
                            _mmDetailPlaning.dpActual_EndDate = @class.ViewRequest.v_ActualEndDate != null ? @class.ViewRequest.v_ActualEndDate : "";
                            _mmDetailPlaning.dpRemark = @class.ViewRequest.v_Remark;
                            _mmDetailPlaning.dpChange_LE_StartDate = @class.ViewRequest.v_Change_LE_StartDate;
                            _mmDetailPlaning.dpChange_LE_EndDate = @class.ViewRequest.v_Change_LE_EndDate;
                            _mmDetailPlaning.dpActual_Remark = @class.ViewRequest.v_Actual_Remark;
                            _mmDetailPlaning.dpStatus = v_subject; //_mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus;
                            _mmDetailPlaning.dpStep = i_Step.ToString();//_mmDetailPlaning.dpStep == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfStep).FirstOrDefault().ToString() : _mmDetailPlaning.dpStep;
                            _mmDetailPlaning.dpEmpcode_Approve = v_ApproveBy;
                            _mmDetailPlaning.dpName_Approve = acc.NICKNAME.ToString();
                            _MOLD._mmDetailPlaning.Update(_mmDetailPlaning);

                            //mmMastPlaning
                            var _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(x => x.mpDocumentNo == rundoc);
                            _mmMastPlaning.mpDocumentNo = rundoc;
                            _mmMastPlaning.mpMonth = @class.ViewRequest.v_month;
                            _mmMastPlaning.mpPlant = @class.ViewRequest.v_arplant;
                            _mmMastPlaning.mpFlow = "02";
                            _mmMastPlaning.mpStep = i_Step;
                            _mmMastPlaning.mpStatus = v_subject;
                            _mmMastPlaning.mpEmpcode_Approve = v_ApproveBy;
                            _mmMastPlaning.mpName_Approve = acc.NICKNAME.ToString();
                            _MOLD.mmMastPlaning.Update(_mmMastPlaning);
                            _MOLD.SaveChanges();


                            dbContextTransaction.Commit();

                        }
                        catch (Exception e)
                        {
                            string a = e.Message;
                            dbContextTransaction.Rollback();
                        }
                    }

                }
                else
                {
                    string[] vRundoc;
                    vRundoc = RunDoc("Save", @class);
                    rundoc = vRundoc[1];
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            //new
                            //@class.ViewRequest.v_requestSheetNo = vRundoc[1];
                            mmRunDocument _mmRunDocument = new mmRunDocument()
                            {
                                rmRunNo = int.Parse(vRundoc[1].Substring(8, 3)),// list_mmRunDocument.rmRunNo + 1,
                                rmPlant = vRundoc[1].Substring(2, 4),
                                rmYear = vRundoc[1].Substring(6, 2),
                                rmGroup = "RE",
                                rmIssueBy = s_issue,
                                rmUpdateBy = s_issue,
                            };
                            _MOLD.mmRunDocument.AddAsync(_mmRunDocument);
                            //_MOLD.SaveChanges();

                            //_mmDetailPlaning

                            //List<mmMastFlowApprove> _mmMastFlowApprove = new List<mmMastFlowApprove>();
                            //_mmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02" && x.mfStep == 0).ToList();
                            mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
                            _mmDetailPlaning.dpDocumentNo = vRundoc[1];// @class.ViewRequest.v_requestSheetNo != null ? @class.ViewRequest.v_requestSheetNo : vRundoc[1];
                            _mmDetailPlaning.dpMoldNo = @class.ViewRequest.v_moldActivityNo;
                            _mmDetailPlaning.dpLine = @class.ViewRequest.v_injectionProductBy == null ? "" : @class.ViewRequest.v_injectionProductBy;
                            _mmDetailPlaning.dpMoldName = @class.ViewRequest.v_moldNo_Name;
                            _mmDetailPlaning.dpModel = @class.ViewRequest.v_model;
                            _mmDetailPlaning.dpIcsNoInj = "-";

                            //plan dmm
                            _mmDetailPlaning.dpPlan_DM_StartDate = @class.ViewRequest.v_planDmStartDate;
                            _mmDetailPlaning.dpPlan_DM_EndDate = @class.ViewRequest.v_planDmEndDate;

                            //for lamp
                            _mmDetailPlaning.dpPlan_LE_StartDate = @class.ViewRequest.v_planLeStartDate == null ? "" : @class.ViewRequest.v_planLeStartDate;
                            _mmDetailPlaning.dpPlan_LE_EndDate = @class.ViewRequest.v_planLeStartDate == null ?"": @class.ViewRequest.v_planLeStartDate;

                            _mmDetailPlaning.dpChange_LE_StartDate = @class.ViewRequest.v_Change_LE_StartDate== null ? "" : @class.ViewRequest.v_Change_LE_StartDate;
                            _mmDetailPlaning.dpChange_LE_EndDate = @class.ViewRequest.v_Change_LE_EndDate == null ? "" : @class.ViewRequest.v_Change_LE_EndDate;


                            //for dmm
                            _mmDetailPlaning.dpActual_StartDate = @class.ViewRequest.v_ActualStartDate != null ? @class.ViewRequest.v_ActualStartDate : "";
                            _mmDetailPlaning.dpActual_EndDate = @class.ViewRequest.v_ActualEndDate != null ? @class.ViewRequest.v_ActualEndDate : "";

                            _mmDetailPlaning.dpLastShotQty = @class.ViewRequest.v_arLastShotQty;
                            _mmDetailPlaning.dpRequestNo = vRundoc[1];
                            _mmDetailPlaning.dpRemark = @class.ViewRequest.v_Remark;

                            _mmDetailPlaning.dpStatus = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02" && x.mfStep == i_Step).Select(x => x.mfSubject).FirstOrDefault().ToString();
                            _mmDetailPlaning.dpStep = i_Step.ToString(); //_mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfStep).FirstOrDefault().ToString();
                            _mmDetailPlaning.dpEmpcode_Issue = s_empcode;
                            _mmDetailPlaning.dpName_Issue = s_name;
                            _mmDetailPlaning.dpIssueDate = s_issuedate;
                            _mmDetailPlaning.dpIssueDept = s_dept;

                            _mmDetailPlaning.dpEmpcode_Approve = v_ApproveBy;
                            _mmDetailPlaning.dpName_Approve = acc.NICKNAME.ToString();
                            _mmDetailPlaning.dpPlan_DM_Month = @class.ViewRequest.v_month;

                            _mmDetailPlaning.dpActual_Remark = @class.ViewRequest.v_Actual_Remark; //chirayu add 16/07/2024

                            _MOLD._mmDetailPlaning.AddAsync(_mmDetailPlaning);
                            // _MOLD.SaveChanges();

                            //mmMastPlaning
                            mmMastPlaning _mmMastPlaning = new mmMastPlaning()
                            {
                                mpDocumentNo = vRundoc[1],
                                mpMonth = @class.ViewRequest.v_month,
                                mpPlant = @class.ViewRequest.v_arplant,
                                mpFlow = "02",
                                mpStep = i_Step,
                                mpStatus = v_subject,//_mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == i_Step).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus,//"Create Document",
                                mpEmpcode_Issue = s_empcode,
                                mpName_Issue = s_name,
                                mpIssueDate = s_issuedate,
                                mpIssueDept = s_dept,
                                mpEmpcode_Approve = v_ApproveBy,
                                mpName_Approve = acc.NICKNAME.ToString(),

                                // rmUpdateBy = s_issue,
                            };
                            _MOLD.mmMastPlaning.AddAsync(_mmMastPlaning);
                            _MOLD.SaveChanges();


                            dbContextTransaction.Commit();

                        }
                        catch (Exception e)
                        {
                            string a = e.Message;
                            dbContextTransaction.Rollback();
                        }
                    }


                }
                string[] returnVal = { "1", rundoc };
                //string[] returnVal = { "1", "" };
                return returnVal;
            }
            catch (Exception ex)
            {
                string[] returnVal = { "0", ex.Message };
                return returnVal;
            }
        }

        public string[] RunDoc(string mode, Class @class)
        {
            string rundoc = "";

            if (@class.ViewRequest.v_requestSheetNo != null)
            {
                rundoc = @class.ViewRequest.v_requestSheetNo;
            }
            else
            {
                var v_plant = @class.ViewRequest.v_docno != null ? @class.ViewRequest.v_docno.Substring(2, 4) : @class.ViewRequest.v_arplant;
                var v_year = @class.ViewRequest.v_docno != null ? @class.ViewRequest.v_docno.Substring(6, 2) : DateTime.Now.ToString("yy"); ;
                var result_mmRunDocument = from mm in _MOLD.mmRunDocument
                                           where mm.rmPlant == v_plant && mm.rmYear == v_year && mm.rmGroup == "RE"
                                           select mm;
                var list_mmRunDocument = result_mmRunDocument.ToList().OrderByDescending(x => x.rmRunNo).FirstOrDefault();
                if (list_mmRunDocument == null)
                {
                    rundoc = "RE" + v_plant + v_year + ((1).ToString("000"));
                    // runNo = 1;
                }
                else
                {
                    rundoc = list_mmRunDocument.rmGroup + list_mmRunDocument.rmPlant + list_mmRunDocument.rmYear + (int.Parse(list_mmRunDocument.rmRunNo.ToString()) + 1).ToString("000");

                }
            }
            string[] returnVal = { "1", rundoc };
            //string[] returnVal = { "1", "" };
            return returnVal;
        }

        [HttpPost]
        public JsonResult Cancel(string DocNo)
        // public JsonResult Cancel(Class @class,string DocNo)
        {
            var dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;//dept edit
            var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value; //admin
            var userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value; //emp login
            string config = "S";
            string msg = "";

            try
            {

                // getDocNO = @class._ViewSearch.v_Docno;// vDocno;

                using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                {
                    try
                    {
                        mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
                        _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == DocNo).FirstOrDefault();
                        if (_mmDetailPlaning != null)
                        {
                            if (vPermission.ToLower() != "admin")
                            {
                                msg = "You don't have permission to access";
                                config = "E";
                                ViewBag.Config = "E";
                                return Json(new { c1 = config, c2 = msg });
                            }
                            else
                            {
                                _mmDetailPlaning.dpStatus = "Cancel";
                                _mmDetailPlaning.dpStep = "0";
                                _MOLD._mmDetailPlaning.Update(_mmDetailPlaning);

                                mmMastPlaning _mmMastPlaning = new mmMastPlaning();
                                _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == DocNo).FirstOrDefault();
                                if (_mmMastPlaning != null)
                                {
                                    _mmMastPlaning.mpStatus = "Cancel";
                                    _mmMastPlaning.mpStep = 0;
                                    _MOLD.mmMastPlaning.Update(_mmMastPlaning);
                                }
                            }

                        }



                        _MOLD.SaveChanges();
                        dbContextTransaction.Commit();
                        config = "S";
                        msg = "Cancel RequestNo: " + DocNo + " Success !!!!!";
                    }
                    catch (Exception ex)
                    {
                        config = "E";
                        msg = "Something is wrong !!!!! : " + ex.Message;
                        dbContextTransaction.Rollback();
                    }

                }


                //if (_mmDetailPlaning.)
                //{

                //    msg = "You don't have permission to access";
                //    config = "E";
                //    ViewBag.Config = "E";


                //    return Json(new { c1 = config, c2 = msg });
                //}


                //config = "S";
                //msg = "Cancel RequestNo: " + DocNo + " Success !!!!!";
                // return Json(new { c1 = config, c2 = msg });

            }
            catch (Exception ex)
            {
                config = "E";
                msg = "Something is wrong !!!!! : " + ex.Message;
                //return Json(new { c1 = config, c2 = msg });
            }
            return Json(new { c1 = config, c2 = msg });
        }
    }
}
