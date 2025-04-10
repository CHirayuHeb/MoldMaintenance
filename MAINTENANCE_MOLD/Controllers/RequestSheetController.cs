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
    public class RequestSheetController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public string _Location = @"\\thsweb\\MAINTENANCE_MOLD\\";
        public RequestSheetController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
        }


        [Authorize("Checked")]
        public IActionResult RequestPage(Class @class)
        {
            ViewBag.Test = @class.ViewRequest != null ? @class.ViewRequest.v_requestSheetNo : "123";
            ViewRequest _ViewRequest = new ViewRequest();
            if (@class.ViewRequest == null)
            {
                _ViewRequest.v_fileName = "";

            }

            //return View("RequestPage", @class);
            //return RedirectToAction("RequestPage", "RequestSheet", @class);
            return View("RequestPage", @class);
        }
        public IActionResult LoadData(string MoldNo, string Docno, Class @class, string MoldName, string moldday, string vMonth)
        {
            MoldNo = MoldNo != null ? MoldNo : @class.ViewRequest.v_model;
            Docno = Docno != null ? Docno : @class.ViewRequest.v_docno;
            ViewRequest v_ViewRequest = new ViewRequest();
            // mmDetailPlaning _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldNo == MoldNo && x.dpDocumentNo == Docno).FirstOrDefault();
            mmDetailPlaning _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldName == MoldName && x.dpDocumentNo == Docno).FirstOrDefault();
            var r_MaActual_Risk = from mm in _MOLD.mmMaActual_Risk
                                  where mm.arMoldNo == MoldNo && mm.arMonth == vMonth  //&&  mm.paMoldNo == "434080"
                                  select mm;

            var r_mtMaster_Mold_Control = from x in _MOLD.mtMaster_Mold_Control
                                          select x;

            List<ViewRequest> _ViewRequest = new List<ViewRequest>();
            var list_join = (from mm in r_MaActual_Risk // outer sequence
                             join mt in r_mtMaster_Mold_Control //inner sequence  // on pa.paMold_Control equals mc.mcMold_Control // key selector 
                             on mm.arMoldName equals mt.mcMoldname // key selector 
                             //?
                             into d2
                             from f in d2.DefaultIfEmpty()
                                 //where d2 != null && d2. == "SomeMoldName" // where clause
                             select new
                             {
                                 v_status = _mmDetailPlaning != null ? _mmDetailPlaning.dpStatus == null ? "" : _mmDetailPlaning.dpStatus : "",
                                 v_month = mm.arMonth,
                                 v_docno = _mmDetailPlaning != null ? _mmDetailPlaning.dpDocumentNo == null ? "" : _mmDetailPlaning.dpDocumentNo : "",
                                 v_requestSheetNo = _mmDetailPlaning != null ? _mmDetailPlaning.dpRequestNo == null ? "" : _mmDetailPlaning.dpRequestNo : "",
                                 v_Issue_date = DateTime.Now.ToString("yyyy/MM/dd"),
                                 v_customer = f != null ? f.mcCUS : "",//yes
                                 v_model = mm.arModel,  //mcModel
                                 v_modelNo = f != null ? f.mcModel : "",//yes
                                 v_moldNo_Name = f != null ? f.mcMoldname : "", //yes
                                 v_icsMoldNo = f != null ? f.mcIcs_Mold : "", //yes
                                 v_icsMoldName = f != null ? f.mcIcs_Moldname : "", //yes
                                 v_moldControlNo = f != null ? f.mcMold_Control : "", //yes
                                 v_rankMold = f != null ? f.mcRange : "", //yes
                                 v_icsInjectionNo = f != null ? f.mcIcs_Injection_R + "," + f.mcIcs_Injection_L : "",//yes
                                 v_moldActivityNo = f != null ? f.mcActivityType : "",//yes

                                 v_mcShortMa = f != null ? f.mcShortMa : 0, //int yes
                                 v_arLastShotQty = mm.arLastShotQty, //int yes

                                 v_moldWeight = f != null ? f.mcMold_Weight : 0, // yes
                                 v_moldSize = f != null ? f.mcMold_Size_X.ToString() + " x " + f.mcMold_Size_Y.ToString() + " x " + f.mcMold_Size_Z.ToString() : "0", // x*y*z  yes
                                 v_injectionProductBy = _mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? "" : _mmDetailPlaning.dpLine : "", //yes
                                 v_responsibility = _mmDetailPlaning.dpName_Issue == null ? "" : _mmDetailPlaning.dpName_Issue, //yes
                                 v_section = _mmDetailPlaning != null ? _mmDetailPlaning.dpIssueDept == null ? "" : _mmDetailPlaning.dpIssueDept : "",//yes
                                 v_InjectionBy = _mmDetailPlaning != null ? _mmDetailPlaning.dpLine == null ? "" : _mmDetailPlaning.dpLine : "",//yes
                                 v_planDmStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_StartDate : "",
                                 v_planDmEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_DM_EndDate : "",
                                 v_planLeStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_StartDate : "",
                                 v_planLeEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpPlan_LE_EndDate : "",
                                 v_Change_LE_StartDate = _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_StartDate == null ? "" : _mmDetailPlaning.dpChange_LE_StartDate : "",
                                 v_Change_LE_EndDate = _mmDetailPlaning != null ? _mmDetailPlaning.dpChange_LE_EndDate == null ? "" : _mmDetailPlaning.dpChange_LE_EndDate : "",
                                 v_ActualStartDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_StartDate : "",
                                 v_ActualEndDate = _mmDetailPlaning != null ? _mmDetailPlaning == null ? "" : _mmDetailPlaning.dpActual_EndDate : "",

                                 v_Remark = _mmDetailPlaning != null ? _mmDetailPlaning.dpRemark : "",
                                 v_arplant = mm.arPlant,
                                 v_fileName = _mmDetailPlaning != null ? _mmDetailPlaning.dpFileName : "",

                                 v_Actual_Remark = _mmDetailPlaning != null ? _mmDetailPlaning.dpActual_Remark : "", //add new


                             }).ToList();

            list_join = list_join.Where(x => x.v_moldActivityNo == MoldNo).ToList();

            for (int i = 0; i < 1; i++)
            {
                v_ViewRequest.v_month = list_join[i].v_month.ToString(); //month
                v_ViewRequest.v_docno = list_join[i].v_docno.ToString();
                v_ViewRequest.v_status = list_join[i].v_status.ToString();
                v_ViewRequest.v_requestSheetNo = @class.ViewRequest != null ? @class.ViewRequest.v_requestSheetNo : list_join[i].v_requestSheetNo.ToString();
                v_ViewRequest.v_Issue_date = list_join[i].v_Issue_date.ToString();
                v_ViewRequest.v_no = i.ToString();
                v_ViewRequest.v_customer = list_join[i].v_customer.ToString();
                v_ViewRequest.v_model = list_join[i].v_modelNo.ToString(); //  list_join[i].v_model.ToString();
                v_ViewRequest.v_moldNo_Name = list_join[i].v_moldNo_Name.ToString();
                v_ViewRequest.v_icsMoldNo = list_join[i].v_icsMoldNo.ToString();
                v_ViewRequest.v_icsMoldName = list_join[i].v_icsMoldName.ToString();
                v_ViewRequest.v_moldControlNo = list_join[i].v_moldControlNo.ToString();
                v_ViewRequest.v_rankMold = list_join[i].v_rankMold.ToString();
                v_ViewRequest.v_moldday = moldday;
                v_ViewRequest.v_icsInjectionNo = list_join[i].v_icsInjectionNo.ToString();
                v_ViewRequest.v_moldActivityNo = list_join[i].v_moldActivityNo.ToString();
                v_ViewRequest.v_mcShortMa = list_join[i].v_mcShortMa;
                v_ViewRequest.v_arLastShotQty = list_join[i].v_arLastShotQty;
                v_ViewRequest.v_moldWeight = list_join[i].v_moldWeight; //double
                v_ViewRequest.v_moldSize = list_join[i].v_moldSize;
                v_ViewRequest.v_injectionProductBy = list_join[i].v_injectionProductBy.ToString();
                v_ViewRequest.v_responsibility = list_join[i].v_responsibility.ToString();
                v_ViewRequest.v_section = list_join[i].v_section.ToString();
                v_ViewRequest.v_planDmStartDate = list_join[i].v_planDmStartDate.ToString();
                v_ViewRequest.v_planDmEndDate = list_join[i].v_planDmEndDate.ToString();
                v_ViewRequest.v_planLeStartDate = list_join[i].v_planLeStartDate.ToString();
                v_ViewRequest.v_planLeEndDate = list_join[i].v_planLeEndDate.ToString();
                v_ViewRequest.v_Change_LE_StartDate = list_join[i].v_Change_LE_StartDate.ToString();
                v_ViewRequest.v_Change_LE_EndDate = list_join[i].v_Change_LE_EndDate.ToString();
                v_ViewRequest.v_ActualStartDate = list_join[i].v_ActualStartDate.ToString();
                v_ViewRequest.v_ActualEndDate = list_join[i].v_ActualEndDate.ToString();


                v_ViewRequest.chkPLANNED = list_join[i].v_planDmStartDate.ToString() == list_join[i].v_planLeStartDate.ToString() ? "1" : "0"; //check plan

                v_ViewRequest.v_arplant = list_join[i].v_arplant.ToString();
                v_ViewRequest.v_Remark = list_join[i].v_Remark != null ? list_join[i].v_Remark.ToString() : ""; //input remake
                v_ViewRequest.v_fileName = list_join[i].v_fileName != null ? list_join[i].v_fileName.ToString() : ""; //input remake

                v_ViewRequest.v_Actual_Remark = list_join[i].v_Actual_Remark != null ? list_join[i].v_Actual_Remark.ToString() : ""; //chirayu add
                //ViewRequest.v_Actual_Remark

                // v_ViewRequest.v_
                // v_dpRemark
            }
            @class.ViewRequest = v_ViewRequest;
            ViewBag.status = v_ViewRequest.v_status;// status
            return View("RequestPage", @class);
            // return Views("RequestPage", "RequestSheet", @class);

        }
        public FileResult openFile(string pathFile)
        {


            string locationfile = _Location + "/" + pathFile;
            // string locationfile = @"//thsweb//MAINTENANCE_MOLD/denso_requestment.txt";
            string extension = Path.GetExtension(locationfile);
            byte[] fileByte = System.IO.File.ReadAllBytes(locationfile);


            return File(fileByte, "application/octet-stream", locationfile);

        }
        [HttpPost]
        public JsonResult History(Class @class, string vdate)//string id, string mode, string vplant, string vdate)
        {
            var id = @class.ViewRequest != null ? @class.ViewRequest.v_requestSheetNo : "";
            var mode = "edit";
            var vplant = @class.ViewRequest != null ? @class.ViewRequest.v_arplant : "";
            //var vdate = @class.ViewRequest != null ? @class.ViewRequest.v_ : "";
            //id = "MP300124001";
            string partialUrl = Url.Action("SendMail", "RequestSheet", new { docno = id, mode, vplant = vplant, vdate = vdate });
            List<mmHistoryApproved> _listHistory = new List<mmHistoryApproved>();

            if (id != null)
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
                    var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(x => x.emEmail_M365).FirstOrDefault();
                    //var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").FirstOrDefault(); //chg to m365
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
            return PartialView("SendMailR", _mmHistoryApproved);

        }
        public JsonResult SendMail_post(string vmode, string vplant, Class @class, mmHistoryApproved _mmHistoryApproved)
        {

            //string getDocNO = "";
            string config = "";
            string msg = "";

            var v_plant = @class.ViewRequest.v_docno.Substring(2, 4);
            var v_year = @class.ViewRequest.v_docno.Substring(6, 2);
            vplant = @class.ViewRequest.v_arplant;
            string[] chkPermis;
            string[] chkCountPlan;
            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
            string v_docN = _mmHistoryApproved.htDocumentNo;


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



            ////   ViewlrBuiltDrawing _ViewlrBuiltDrawing = new ViewlrBuiltDrawing();
            var email = new MimeMessage();

            int i_Step = v_docN is null ? 0 : _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == _mmHistoryApproved.htDocumentNo).Select(x => int.Parse(x.dpStep)).FirstOrDefault();
            // mmDetailPlaning _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpRequestNo == _mmHistoryApproved.htDocumentNo).FirstOrDefault();

            //int i_Step = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == _mmHistoryApproved.htDocumentNo).Select(x => x.mpStep).FirstOrDefault();
            //mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == _mmHistoryApproved.htDocumentNo).FirstOrDefault();



            if (_mmHistoryApproved.htTo != null || (_mmHistoryApproved.htTo == null && _mmHistoryApproved.htStatus == "Disapprove"))
            {
                if (_mmHistoryApproved.htStatus == "Approve")
                {
                    i_Step = i_Step + 1;//  _mmHistoryApproved.htStep + 1;
                    var v_subject = _MOLD.mmMastFlowApprove.Where(x => x.mfStep == i_Step && x.mfFlowNo == "02").Select(x => x.mfSubject).FirstOrDefault();
                    _mmHistoryApproved.htStatus = v_subject;
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
                try
                {
                    string[] getDocNO;
                    // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
                    getDocNO = Save(vmode, @class, i_Step);
                    _mmHistoryApproved.htDocumentNo = getDocNO[1]; // getDocNO; //getDocNO[1];
                    if (!(int.Parse(getDocNO[0]) > 0)) { return Json(new { c1 = "E", c2 = getDocNO[1] }); }

                    _mmHistoryApproved.htTo = _mmHistoryApproved.htTo != null ? _mmHistoryApproved.htTo.Split(" ")[0] : _mmHistoryApproved.htTo; //split ""
                    var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emEmpcode).First();

                    var v_emName_M365 = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emName_M365).First();


                    ViewAccEMPLOYEE acc = _HRMS.AccEMPLOYEE.FirstOrDefault(s => s.EMP_CODE == v_ApproveBy);

                    _mmHistoryApproved.htStep = i_Step;
                    _mmHistoryApproved.htDate = DateTime.Now.ToString("yyyy/MM/dd");
                    _mmHistoryApproved.htTime = DateTime.Now.ToString("HH:mm:ss");





                    _MOLD.mmHistoryApproved.Add(_mmHistoryApproved);

                    email.Subject = "Period Maintenance Mold Request ==> " + _mmHistoryApproved.htStatus;//  _mmDetailPlaning.dpStatus.ToString();

                    MailboxAddress Formmail365 = new MailboxAddress(v_emName_M365, _mmHistoryApproved.htFrom);
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

                    //    ViewBag.Config = config;
                    //    TempData["Config"] = ViewBag.Config;

                    _MOLD.SaveChanges();


                    if (i_Step == 2)
                    {
                        using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                        {
                            try
                            {
                                mtMaster_Mold_Control _mtMaster_Mold_Control = new mtMaster_Mold_Control();
                                _mtMaster_Mold_Control = _MOLD.mtMaster_Mold_Control.Where(x => x.mcMoldname == @class.ViewRequest.v_moldNo_Name).FirstOrDefault();
                                _mtMaster_Mold_Control.mcLastShotMa = 0;
                                _mtMaster_Mold_Control.mcLastCleaning = @class.ViewRequest.v_ActualStartDate;  //DateTime.Now.ToString("yyyy/MM/dd");
                                _MOLD.mtMaster_Mold_Control.Update(_mtMaster_Mold_Control);
                                _MOLD.SaveChanges();
                                dbContextTransaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                    }

                    var varifyUrl = "http://thsweb/MVCPublish/MoldMaintenance/Home/Login?mode=edit&DocumentNo=" + getDocNO[1] + "&MoldNo=" + @class.ViewRequest.v_moldNo_Name + "&UserID=" + v_ApproveBy + "&Plant=" + vplant + "&Date=" + @class.ViewRequest.v_month;
                    var bodyBuilder = new BodyBuilder();
                    string EmailBody = "";
                    //    //var image = bodyBuilder.LinkedResources.Add(@"E:\01_My Document\02_Project\_2023\1. PartTransferUnbalance\PartTransferUnbalance\wwwroot\images\btn\OK.png");
                    EmailBody = $"<div>" +
                    $"<B>PERIOD MAINTENANCE MOLD REQUEST : </B>" + "" + "<br>" +
                    $"Document No : " + "" + getDocNO[1] + "" + "<br>" +
                    $"Mold Name : " + "" + @class.ViewRequest.v_model + "" + "<br>" +
                    $"Plant : " + "" + vplant + "" + "<br>" +
                    $"Month : " + "" + @class.ViewRequest.v_month + "" + "<br>" +
                    $"Status :" + "" + " " + _mmHistoryApproved.htStatus + "<br>" +
                    //$"Status :" + "" + " " + _mmDetailPlaning.dpStatus.ToString() + "<br>" +
                    $"<a href='" + varifyUrl + "'> More Detail" +
                    // $"<img src = 'http://thsweb/MVCPublish/QA_APPROVAL_REQUEST/images/btn/mail1.png' alt = 'HTML tutorial' style = 'width: 50px; height: 50px;'>" +
                    $"</a>" +
                    $"</div>";


                    // bodyBuilder.Attachments.Add(@"E:\01_My Document\02_Project\_2023\1. PartTransferUnbalance\PartTransferUnbalance\dev_rfc.log");

                    bodyBuilder.HtmlBody = string.Format(EmailBody);
                    email.Body = bodyBuilder.ToMessageBody();

                    // send email
                    var smtp = new SmtpClient();
                    //smtp.Connect("10.200.128.12");
                    smtp.Connect("203.146.237.138");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    config = "S";
                    msg = "";
                    return Json(new { c1 = config, c2 = msg });
                }
                catch (Exception ex)
                {
                    //config = "E";
                    //ViewBag.Config = "E";
                    //TempData["Config"] = ViewBag.Config;
                    //TempData["Msg"] = ex.Message;

                    config = "E";
                    msg = "Error : " + ex.Message;
                    return Json(new { c1 = config, c2 = msg });
                }






                //return Json(new { c1 = config, c2 = msg });
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

        [HttpPost]
        public JsonResult SaveData(Class @class, string pathfile, IFormFile file)
        {
            var filelist2 = HttpContext.Request.Form.Files;
            var filelist = pathfile != null ? pathfile : "";

            string[] filename;
            string v_fileName = "";

            // @class.ViewRequest.v_fileName = filelist;



            string s_empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_name = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            string s_issue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " - " + HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_issuedate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            string s_dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value; // dep
            string[] vRundoc;
            string config = "";
            string msg = "";
            string v_status = "";

            string[] chkPermis;
            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
            chkPermis = chkPermission(@class, "Send");


            if (chkPermis[1].ToString() == "No")
            {

                msg = "You don't have permission to access";
                config = "P";
                ViewBag.Config = "P";
                TempData["Config"] = ViewBag.Config;
                TempData["Msg"] = ViewBag.Msg;

                return Json(new { c1 = config, c2 = msg });
            }


            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
            vRundoc = RunDoc("Save", @class);


            List<mmMastFlowApprove> _mmMastFlowApprove = new List<mmMastFlowApprove>();
            _mmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02").ToList();

            //check permistion 

            //save
            using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
            {
                try
                {
                    if (pathfile != null && pathfile != "")
                    {
                        string[] authorsList = pathfile.Split("\\");
                        v_fileName = authorsList.Length > 0 ? authorsList[authorsList.Length - 1] : "";
                    }



                    mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
                    //_mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldNo == @class.ViewRequest.v_model && x.dpDocumentNo == @class.ViewRequest.v_docno).FirstOrDefault();
                    _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldNo == @class.ViewRequest.v_moldActivityNo && x.dpDocumentNo == @class.ViewRequest.v_docno).FirstOrDefault();
                    _mmDetailPlaning.dpRequestNo = @class.ViewRequest.v_requestSheetNo != null ? @class.ViewRequest.v_requestSheetNo : vRundoc[1];
                    _mmDetailPlaning.dpActual_StartDate = @class.ViewRequest.v_ActualStartDate != null ? @class.ViewRequest.v_ActualStartDate : "";
                    _mmDetailPlaning.dpActual_EndDate = @class.ViewRequest.v_ActualEndDate != null ? @class.ViewRequest.v_ActualEndDate : "";
                    _mmDetailPlaning.dpRequestNo = vRundoc[1];
                    _mmDetailPlaning.dpRemark = @class.ViewRequest.v_Remark;


                    _mmDetailPlaning.dpChange_LE_StartDate = @class.ViewRequest.v_Change_LE_StartDate;
                    _mmDetailPlaning.dpChange_LE_EndDate = @class.ViewRequest.v_Change_LE_EndDate;


                    // _mmDetailPlaning.dpFileName = v_fileName;//filelist;
                    _mmDetailPlaning.dpStatus = _mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus;
                    _mmDetailPlaning.dpStep = _mmDetailPlaning.dpStep == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfStep).FirstOrDefault().ToString() : _mmDetailPlaning.dpStep;

                    _mmDetailPlaning.dpActual_Remark = @class.ViewRequest.v_Actual_Remark;//chirayu add 16/07/2024


                    v_status = _mmDetailPlaning.dpStatus;

                    //_MOLD._mmDetailPlaning.Update(_mmDetailPlaning);
                    //_MOLD.SaveChanges();

                    if (@class.ViewRequest.v_requestSheetNo == null)
                    {
                        @class.ViewRequest.v_requestSheetNo = vRundoc[1];
                        mmRunDocument _mmRunDocument = new mmRunDocument()
                        {
                            rmRunNo = int.Parse(vRundoc[1].Substring(8, 3)),// list_mmRunDocument.rmRunNo + 1,
                            rmPlant = vRundoc[1].Substring(2, 4),
                            rmYear = vRundoc[1].Substring(6, 2),
                            rmGroup = "RE",
                            rmIssueBy = s_issue,
                            rmUpdateBy = s_issue,
                        };
                        _MOLD.mmRunDocument.Add(_mmRunDocument);
                        _MOLD.SaveChanges();

                        mmMastPlaning _mmMastPlaning = new mmMastPlaning()
                        {
                            mpDocumentNo = vRundoc[1],
                            mpMonth = @class.ViewRequest.v_month,
                            mpPlant = @class.ViewRequest.v_arplant,
                            mpFlow = "01",
                            mpStep = 0,
                            mpStatus = "Create Document",
                            mpEmpcode_Issue = s_empcode,
                            mpName_Issue = s_name,
                            mpIssueDate = s_issuedate,
                            mpIssueDept = s_dept,
                            mpEmpcode_Approve = "",
                            mpName_Approve = "",

                            // rmUpdateBy = s_issue,
                        };
                        _MOLD.mmMastPlaning.Add(_mmMastPlaning);
                        _MOLD.SaveChanges();


                        _mmDetailPlaning.dpEmpcode_Issue = s_empcode;
                        _mmDetailPlaning.dpName_Issue = s_name;
                        _mmDetailPlaning.dpIssueDate = s_issuedate;
                        _mmDetailPlaning.dpIssueDept = s_dept;

                    }
                    else
                    {
                        s_name = _mmDetailPlaning.dpName_Issue;
                        s_dept = _mmDetailPlaning.dpIssueDept;
                    }

                    _MOLD._mmDetailPlaning.Update(_mmDetailPlaning);
                    _MOLD.SaveChanges();


                    // _MOLD.SaveChanges();
                    dbContextTransaction.Commit();
                    config = "S";
                }
                catch (Exception e)
                {
                    config = "E";
                    msg = e.ToString();
                    dbContextTransaction.Rollback();
                }
            }


            return Json(new
            {
                c1 = config,
                c2 = msg,
                c3 = @class,
                c4 = vRundoc[1],
                c5 = v_status,
                c6 = v_fileName,
                c7 = s_name,
                c8 = s_dept,
            });
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

        public string[] RunDoc(string mode, Class @class)
        {
            string rundoc = "";

            if (@class.ViewRequest.v_requestSheetNo != null)
            {
                rundoc = @class.ViewRequest.v_requestSheetNo;
            }
            else
            {
                var v_plant = @class.ViewRequest.v_docno.Substring(2, 4);
                var v_year = @class.ViewRequest.v_docno.Substring(6, 2);
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

        //check permission
        public string[] chkPermission(Class @class, string mode)
        {
            var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value;
            var doc = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value; //doc no edit
            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Actor)?.Value;//user edit
            var dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;//dept edit

            var userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value; //emp login
            var s_Docno = @class.ViewRequest.v_requestSheetNo;
            var s_PlanDocno = @class.ViewRequest.v_docno;
            var _mmHistoryApproved = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_PlanDocno && x.htStep == 2).Select(x => x.htFrom).First();
            //var v_empcreate = _IT.Email.Where(x => x.emEmail == _mmHistoryApproved).Select(x => x.emEmpcode).First();
            var v_empcreate = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved).Select(x => x.emEmpcode).First();
            var v_per = "";
            var m_per = "";
            try
            {
                //create new
                //wait 
                //finish
                if (s_Docno == null)
                {

                    if (v_empcreate != userlogin)
                    {
                        v_per = "No";
                        m_per = "You don't have permission to access'";
                    }

                    //if (vPermission != "admin")
                    //{
                    //    v_per = "No";
                    //    m_per = "You don't have permission to access'";
                    //}
                }
                else
                {
                    var v_Step = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_Docno).OrderByDescending(x => x.htStep).Select(x => x.htStep).FirstOrDefault();
                    // v_Step = v_Step != null ? v_Step : 0;
                    if (v_Step == 0)
                    {
                        if (v_empcreate != userlogin)
                        {
                            v_per = "No";
                            m_per = "You don't have permission to access'";
                        }
                        //if (vPermission != "admin")
                        //{
                        //    v_per = "No";
                        //    m_per = "You don't have permission to access'";
                        //}
                    }
                    else if (v_Step == 2)
                    {
                        v_per = "No";
                        m_per = "You don't have permission to access'";
                    }
                    else
                    {

                        var v_From = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_Docno && x.htStep == v_Step).Select(x => x.htTo).FirstOrDefault();
                        //var v_ApproveBy = _IT.Email.Where(x => x.emEmail == (v_From != null ? v_From.ToString() : "")).Select(x => x.emEmpcode).First();
                        var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == (v_From != null ? v_From.ToString() : "")).Select(x => x.emEmpcode).First();
                        if ((v_ApproveBy != null ? v_ApproveBy : "") == userlogin)
                        {
                            v_per = "Yes";
                        }
                        else
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

        public string[] chkConfrimPlan(Class @class)
        {
            var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value;
            var doc = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value; //doc no edit
            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Actor)?.Value;//user edit
            var dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;//dept edit
            var userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value; //emp login
            var s_Docno = @class.ViewRequest.v_requestSheetNo;
            var v_plant_count = "";
            if (@class.List_ViewMoldData.Count() > 0 && @class._ViewSearch.V_Status == "Waiting Approve by Planing Lamp (Adjust Plan)")
            {

                for (int i = 0; i <= @class.List_ViewMoldData.Count() - 1; i++)
                {
                    if (@class.List_ViewMoldData[i].v_Plan_LE_StartDate is null)
                    {
                        v_plant_count = "No";

                    }
                }


            }
            else
            {
                v_plant_count = "yes";

            }
            string[] returnVal = { "1", v_plant_count };
            return returnVal;


        }
        public string[] chkCanEdit(Class @class)
        {

            //var userlogin = "";

            //@class._ViewSearch.v_empcode 
            var m_empcode = @class._ViewSearch.v_empcode;//TempData["empcode"] !=null ? TempData["empcode"].ToString() : @class._ViewSearch.v_empcode;
            var t_empcode = HttpContext != null ? HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value : m_empcode;
            // var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value;

            //userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value !=null? HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value : t_empcode; //emp login
            var s_Docno = @class._ViewSearch.v_Docno;
            var v_chk = "";
            if (@class._ViewSearch.v_Docno != null)
            {
                mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(s => s.mpDocumentNo == @class._ViewSearch.v_Docno);
                v_chk = _mmMastPlaning != null ? _mmMastPlaning.mpEmpcode_Approve == m_empcode && _mmMastPlaning.mpStep == 1 ? "edit" : "No" : "No";
                //v_chk =  _mmMastPlaning.mpEmpcode_Approve == 
                if (_mmMastPlaning != null)
                {
                    if (_mmMastPlaning.mpStep == 2)
                    {
                        v_chk = "edit";
                    }

                }

            }

            else
            {
                v_chk = "No";
            }



            string[] returnVal = { "1", v_chk };
            return returnVal;


        }
        public string[] Save(string mode, Class @class, int vstep)
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
            var a_month = "";
            var a_year = "";
            string[] a_dateMonth;


            var rundoc = "";
            int runNo = 0;

            try
            {
                var v_subject = _MOLD.mmMastFlowApprove.Where(x => x.mfStep == vstep && x.mfFlowNo == "02").Select(x => x.mfSubject).FirstOrDefault();

                if (s_Docno != null)
                {
                    runNo = int.Parse(s_Docno.Substring(8, 3));
                    rundoc = s_Docno;
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            var _mmDetailPlaning = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpRequestNo == rundoc);
                            // _mmDetailPlaning.dpRequestNo = @class.ViewRequest.v_requestSheetNo != null ? @class.ViewRequest.v_requestSheetNo : s_Docno;
                            _mmDetailPlaning.dpActual_StartDate = @class.ViewRequest.v_ActualStartDate != null ? @class.ViewRequest.v_ActualStartDate : "";
                            _mmDetailPlaning.dpActual_EndDate = @class.ViewRequest.v_ActualEndDate != null ? @class.ViewRequest.v_ActualEndDate : "";
                            //_mmDetailPlaning.dpRequestNo = vRundoc[1];
                            _mmDetailPlaning.dpRemark = @class.ViewRequest.v_Remark;

                            _mmDetailPlaning.dpChange_LE_StartDate = @class.ViewRequest.v_Change_LE_StartDate;
                            _mmDetailPlaning.dpChange_LE_EndDate = @class.ViewRequest.v_Change_LE_EndDate;

                            _mmDetailPlaning.dpActual_Remark = @class.ViewRequest.v_Actual_Remark;//chirayu add 16/07/2024
                            _mmDetailPlaning.dpStatus = v_subject;//_mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus;
                            _mmDetailPlaning.dpStep = vstep.ToString(); // _mmDetailPlaning.dpStep == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfStep).FirstOrDefault().ToString() : _mmDetailPlaning.dpStep;
                            //_mmDetailPlaning.dpEmpcode_Issue = s_empcode;
                            //_mmDetailPlaning.dpName_Issue = s_name;
                            //_mmDetailPlaning.dpIssueDate = s_issuedate;
                            //_mmDetailPlaning.dpIssueDept = s_dept;



                            var _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(x => x.mpDocumentNo == rundoc);
                            _mmMastPlaning.mpDocumentNo = rundoc;
                            _mmMastPlaning.mpMonth = @class.ViewRequest.v_month;
                            _mmMastPlaning.mpPlant = @class.ViewRequest.v_arplant;
                            //_mmMastPlaning.//mpFlow = "01",
                            _mmMastPlaning.mpStep = vstep;
                            _mmMastPlaning.mpStatus = v_subject;
                            //_mmMastPlaning.mpEmpcode_Issue = s_empcode;
                            //_mmMastPlaning.mpName_Issue = s_name;
                            //_mmMastPlaning.mpIssueDate = s_issuedate;
                            _mmMastPlaning.mpIssueDept = s_dept;
                            _MOLD.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            string[] returnVal1 = { "0", ex.Message };
                            return returnVal1;
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
                            _MOLD.mmRunDocument.Add(_mmRunDocument);
                            _MOLD.SaveChanges();


                            List<mmMastFlowApprove> _mmMastFlowApprove = new List<mmMastFlowApprove>();
                            _mmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02" && x.mfStep == 0).ToList();
                            mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning();
                            _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldNo == @class.ViewRequest.v_moldActivityNo && x.dpDocumentNo == @class.ViewRequest.v_docno).FirstOrDefault();
                            _mmDetailPlaning.dpRequestNo = @class.ViewRequest.v_requestSheetNo != null ? @class.ViewRequest.v_requestSheetNo : vRundoc[1];
                            _mmDetailPlaning.dpActual_StartDate = @class.ViewRequest.v_ActualStartDate != null ? @class.ViewRequest.v_ActualStartDate : "";
                            _mmDetailPlaning.dpActual_EndDate = @class.ViewRequest.v_ActualEndDate != null ? @class.ViewRequest.v_ActualEndDate : "";

                            _mmDetailPlaning.dpChange_LE_StartDate = @class.ViewRequest.v_Change_LE_StartDate;
                            _mmDetailPlaning.dpChange_LE_EndDate = @class.ViewRequest.v_Change_LE_EndDate;

                            _mmDetailPlaning.dpRequestNo = vRundoc[1];
                            _mmDetailPlaning.dpRemark = @class.ViewRequest.v_Remark;
                            _mmDetailPlaning.dpStatus = v_subject;//_mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus;
                            _mmDetailPlaning.dpStep = vstep.ToString(); //_mmDetailPlaning.dpStep == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfStep).FirstOrDefault().ToString() : _mmDetailPlaning.dpStep;
                            _mmDetailPlaning.dpEmpcode_Issue = s_empcode;
                            _mmDetailPlaning.dpName_Issue = s_name;
                            _mmDetailPlaning.dpIssueDate = s_issuedate;
                            _mmDetailPlaning.dpIssueDept = s_dept;
                            _mmDetailPlaning.dpActual_Remark = @class.ViewRequest.v_Actual_Remark; //chirayu add 16/07/2024
                            _MOLD._mmDetailPlaning.Update(_mmDetailPlaning);
                            _MOLD.SaveChanges();

                            //_MOLD.mmMastPlaning.Update(_mmMastPlaning);


                            mmMastPlaning _mmMastPlaning = new mmMastPlaning()
                            {
                                mpDocumentNo = vRundoc[1],
                                mpMonth = @class.ViewRequest.v_month,
                                mpPlant = @class.ViewRequest.v_arplant,
                                mpFlow = "02",
                                mpStep = vstep,
                                mpStatus = _mmDetailPlaning.dpStatus == null ? _mmMastFlowApprove.Where(x => x.mfStep == 0).Select(x => x.mfSubject).FirstOrDefault().ToString() : _mmDetailPlaning.dpStatus,//"Create Document",
                                mpEmpcode_Issue = s_empcode,
                                mpName_Issue = s_name,
                                mpIssueDate = s_issuedate,
                                mpIssueDept = s_dept,
                                mpEmpcode_Approve = "",
                                mpName_Approve = "",

                                // rmUpdateBy = s_issue,
                            };
                            _MOLD.mmMastPlaning.Add(_mmMastPlaning);
                            _MOLD.SaveChanges();

                            dbContextTransaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            //e.Message;
                            dbContextTransaction.Rollback();
                            string[] returnVal1 = { "0", ex.Message };
                            return returnVal1;
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