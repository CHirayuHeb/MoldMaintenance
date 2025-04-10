using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using MAINTENANCE_MOLD.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using Newtonsoft.Json;
using static MAINTENANCE_MOLD.Models.Table.Tb_Hrms;
using static MAINTENANCE_MOLD.Models.Table.Tb_Mold;
using static MAINTENANCE_MOLD.Models.Table.Tb_Search;
using static MAINTENANCE_MOLD.Models.Table.Tb_ThsReport;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace MAINTENANCE_MOLD.Controllers
{
    public class HomeController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public RequestSheetController _RequestSheetController;
        public HomeController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
        }

        public IActionResult Login(Class @class, string json_Error)
        {

            //this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // TempData.Clear();
            string Mode = "";
            string DocumentNo = "";
            string MDocumentNo = "";
            string UserID = "";
            string Plant = "";
            string Datev = "";
            string MoldNo = "";
            string Doc = "";
            if (json_Error != null)
            {

                @class._Error = new Error();
                @class._Error = JsonConvert.DeserializeObject<Error>(json_Error);
            }


            if (HttpContext.Request.Query.Count() > 0)
            {
                //if (!String.IsNullOrEmpty(HttpContext.Request.Query.Count()))
                Mode = HttpContext.Request.Query["mode"];
                DocumentNo = HttpContext.Request.Query["DocumentNo"];
                UserID = HttpContext.Request.Query["UserID"];
                Plant = HttpContext.Request.Query["Plant"];
                Datev = HttpContext.Request.Query["Date"];
                MoldNo = HttpContext.Request.Query["MoldNo"];

                //case manual 
                MDocumentNo = HttpContext.Request.Query["MDocumentNo"];


                //TempData["DocumentNo"] = DocumentNo;
                //TempData["UserID"] = UserID;
                //TempData["Mode"] = Mode;
                //TempData["Plant"] = Plant;
                //TempData["Datev"] = Datev;
                //mmMastPlaning mastPlaning = new mmMastPlaning();
                Doc = DocumentNo != null ? DocumentNo : MDocumentNo;

                mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(s => s.mpDocumentNo == Doc);
                if (_mmMastPlaning != null)
                {
                    @class._ViewSearch = new ViewSearch();
                    @class._ViewSearch.v_Docno = _mmMastPlaning.mpDocumentNo.ToString();
                    @class._ViewSearch.v_MDocno = MDocumentNo;
                    @class._ViewSearch.v_plant = _mmMastPlaning.mpPlant.ToString();
                    @class._ViewSearch.v_Date = _mmMastPlaning.mpMonth.ToString();
                    @class._ViewSearch.V_Status = _mmMastPlaning.mpStatus.ToString();
                    @class._ViewSearch.v_empcode = UserID;
                    @class._ViewSearch.v_MoldNo = MoldNo;
                }


            }
            return View("Login", @class);
            // return View();
        }


        [Authorize("Checked")]
        public ActionResult Index(Class @class, string s_modelclass)
        {
            //string DocnoEdit = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value;
            //string PlantEdit = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Locality)?.Value;
            //string DateEdit = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Expired)?.Value;


            //var s_plant = @class._ViewSearch.v_plant;
            //var s_date = @class._ViewSearch.v_Date;
            //SearchData(@class);
            if (s_modelclass != null)
            {
                @class._ViewSearch = JsonConvert.DeserializeObject<ViewSearch>(s_modelclass);
                ActionEvent("SEARCH", @class, "");
            }

            return View("Index", @class);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Autherize(Class @class)
        {
            string sUsername = @class._ViewLogin.UserId.Trim();
            string sPassword = @class._ViewLogin.Password.Trim();
            MoldLogin login = _MOLD.Login.FirstOrDefault(s => s.UserId == sUsername && s.Password == sPassword && s.Program == "MoldMaintenance");
            if (login != null)
            {

                ViewAccEMPLOYEE acc = _HRMS.AccEMPLOYEE.FirstOrDefault(s => s.EMP_CODE == login.Empcode);
                // TempData["userData"] = JsonConvert.SerializeObject(login);

                //TempData["empcode"] = acc.EMP_CODE;
                //TempData["userData"] = login;
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Country, "MAINTENANCE_MOLD")); //get login id
                claims.Add(new Claim(ClaimTypes.NameIdentifier, login.UserId.ToString()));
                //claims.Add(new Claim(ClaimTypes.SerialNumber, login.Password.ToString()));
                claims.Add(new Claim(ClaimTypes.Authentication, login.Permission.ToString()));
                claims.Add(new Claim(ClaimTypes.System, login.Permission.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, acc.DEPT_CODE.ToString()));
                claims.Add(new Claim(ClaimTypes.Version, acc.SEC_CODE.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, acc.NICKNAME));
                claims.Add(new Claim(ClaimTypes.SerialNumber, acc.EMP_CODE));
                claims.Add(new Claim(ClaimTypes.UserData, acc.EMP_TNAME + " " + acc.LAST_TNAME)); //get login name
                claims.Add(new Claim(ClaimTypes.Dns, @class._ViewSearch.v_Docno != null ? @class._ViewSearch.v_Docno : "")); //doc no edit
                claims.Add(new Claim(ClaimTypes.Actor, @class._ViewSearch.v_empcode != null ? @class._ViewSearch.v_empcode : "")); //user edit
                                                                                                                                   //// claims.Add(new Claim(ClaimTypes.Upn, TempData["Mode"] != null ? TempData["Mode"].ToString() : "")); //Mode edit
                claims.Add(new Claim(ClaimTypes.Locality, @class._ViewSearch.v_plant != null ? @class._ViewSearch.v_plant : "")); //plant edit
                claims.Add(new Claim(ClaimTypes.Expired, @class._ViewSearch.v_Date != null ? @class._ViewSearch.v_Date : "")); //datev edit

                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);


                this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties() {
                    IsPersistent = true
                });
                // this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme , principal, new AuthenticationProperties() { IsPersistent = true }); //true is remember login
                try
                {
                    if (@class._ViewSearch.v_MDocno != null)
                    {
                        var _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpDocumentNo == @class._ViewSearch.v_MDocno).FirstOrDefault();
                        var v_MoldNo = _mmDetailPlaning.dpMoldNo != null ? _mmDetailPlaning.dpMoldNo : "";
                        var v_Docno = _mmDetailPlaning.dpDocumentNo != null ? _mmDetailPlaning.dpDocumentNo : "";
                        var v_Moldname = _mmDetailPlaning.dpDocumentNo != null ? _mmDetailPlaning.dpMoldName : "";
                        var vMonth = @class._ViewSearch.v_Date != null ? @class._ViewSearch.v_Date : "";
                        return RedirectToAction("LoadData", "ManualRequestSheet", new { @class = @class, vMoldActivity = v_MoldNo, Docno = v_Docno, MoldName = v_Moldname, vMonth = vMonth });

                    }
                    else if (@class._ViewSearch.v_Docno != null)
                    {

                        if (@class._ViewSearch.v_Docno.Contains("MP")) //planning
                        {
                            var t_docno = @class._ViewSearch.v_Docno; // TempData["DocumentNo"].ToString();
                            mmMastPlaning mastPlaning = new mmMastPlaning();
                            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(s => s.mpDocumentNo == t_docno);
                            @class._ViewSearch = new ViewSearch();
                            @class._ViewSearch.v_Docno = _mmMastPlaning.mpDocumentNo.ToString();
                            @class._ViewSearch.v_plant = _mmMastPlaning.mpPlant.ToString();
                            @class._ViewSearch.v_Date = _mmMastPlaning.mpMonth.ToString();
                            @class._ViewSearch.V_Status = _mmMastPlaning.mpStatus.ToString();
                            @class._ViewSearch.v_empcode = acc.EMP_CODE;
                            @class._ViewSearch.v_page = "More";
                            ActionEvent("SEARCH", @class, ""); //action button
                            string v_class = JsonConvert.SerializeObject(@class._ViewSearch);
                            return RedirectToAction("Index", "Home", new { @class = @class, s_modelclass = v_class });
                        }
                        else
                        { //if((@class._ViewSearch.v_Docno.Contains("RE"))) { //request

                            var _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpMoldName == @class._ViewSearch.v_MoldNo && x.dpRequestNo == @class._ViewSearch.v_Docno).FirstOrDefault();
                            var v_MoldNo = _mmDetailPlaning.dpMoldNo != null ? _mmDetailPlaning.dpMoldNo : "";
                            var v_Docno = _mmDetailPlaning.dpDocumentNo != null ? _mmDetailPlaning.dpDocumentNo : "";
                            var v_Moldname = _mmDetailPlaning.dpDocumentNo != null ? _mmDetailPlaning.dpMoldName : "";
                            return RedirectToAction("LoadData", "RequestSheet", new { @class = @class, MoldNo = v_MoldNo, Docno = v_Docno, MoldName = v_Moldname });
                            //_RequestSheetController.LoadData(_mmDetailPlaning.dpMoldNo, _mmDetailPlaning.dpDocumentNo, @class);
                        }


                        //return View("Index", @class);
                        //SearchData(@class);
                        // _ViewSearch.v_Docno = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value.ToString();
                    }
                    else
                    {
                        //return View("Index", @class);
                        return RedirectToAction("SearchPage", "Search");
                    }
                }
                catch (Exception e)
                {

                    //  @class.cr;
                    @class._Error = new Error();
                    @class._Error.validation = "Not found Link,please check your link !!!!";
                    // return RedirectToAction("Login", "Home", @class);

                    //return Content("<script language='javascript' type='text/javascript'>alert     ('Requested Successfully ');</script>");
                    string v_Error = JsonConvert.SerializeObject(@class._Error);
                    //return RedirectToAction("Login", "Home");
                    return View("Login", @class);
                }


            }

            else
            {
                @class._Error = new Error();
                @class._Error.validation = "Username or Password invalid";
                return View("Login", @class);
            }

        }
        [HttpPost] //id: id, mode: mode, vplant: i_Plant, vdate: i_date
        public ActionResult SendData(Class @class, string id)
        {
            string partialUrl = Url.Action("SendMail", "Home");//, new { docno = id, mode });

            //if (id != null)
            //{
            //    string hisId = id;
            //    qaMastRequest recentStep = _QS9000.MastRequest.Find(hisId);
            //    List<qaHistoryApproved> listHistory = _QS9000.MastHistoryApproved.Where(x => x.htRequestNo == hisId).ToList();

            //    return Json(new { status = "hasHistory", listHistory, recentStep, partial = partialUrl });
            //}
            return Json(new { status = "empty", partial = partialUrl });


            // return View("Login", @class);
        }


        //[HttpPost]
        public ActionResult ActionEvent(string submitButton, Class @class, string v_class)
        {
            // @classs._ViewSearch = new ViewSearch();
            //ViewSearch _ViewSearch = (ViewSearch)TempData["_ViewSearch"];

            switch (submitButton)
            {
                case "SEARCH":
                    // @class.List_ViewMoldData.Clear();
                    ViewBag.ViewMoldData = "";
                    ViewBag.ViewMoldData_iCount = "";
                    ViewBag.View_status_Data = "";
                    ViewBag.View_MastPlaning_mpFlow = "";
                    ViewBag.ViewMoldData_count = "";
                    ViewBag.chkEdit = "";
                    ViewBag.vPage = "";
                    ViewBag.Docno = "";
                    //SearchData(@class);// (Cancel());
                    return SearchData(@class);
                case "SAVE":
                    return (SaveData(@class));
                case "More":
                    @class._ViewSearch = new ViewSearch();
                    @class._ViewSearch = JsonConvert.DeserializeObject<ViewSearch>(v_class);
                    // @class._ViewSearch = (ViewSearch)JsonConvert.DeserializeObject(TempData[" @classs._ViewSearch"], (typeof(ViewSearch)));
                    //@class._ViewSearch.v_Docno = v_Docno;
                    //@class._ViewSearch.v_plant = v_plant;
                    //@class._ViewSearch.v_Date = v_Date; //= @class._ViewSearch.v_Date;
                    //@class._ViewSearch.V_Status = V_Status; //= @class._ViewSearch.V_Status;
                    //@class._ViewSearch.v_empcode= v_empcode;//@class._ViewSearch.v_empcode;
                    return (SearchData(@class));// (Cancel());
                default:
                    return (View());
            }
            //return PartialView("SendMail");
        }

        [HttpPost]
        public ActionResult SearchData(Class @class)
        {
            var s_plant = @class._ViewSearch.v_plant;
            var s_date = @class._ViewSearch.v_Date;
            var s_Status = @class._ViewSearch.V_Status;
            string[] a_dateMonth = s_date.Split("/");
            ViewBag.sMonth = get_Month(a_dateMonth[1]);
            ViewBag.sYear = a_dateMonth[0];
            int days = DateTime.DaysInMonth(int.Parse(a_dateMonth[0]), int.Parse(a_dateMonth[1]));
            ViewBag.sdays = days;
            var vStep = "";

            string[] chkEdit;
            string s_chkEdit = "";




            //get status
            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(s => s.mpMonth == s_date && s.mpPlant == s_plant && s.mpDocumentNo.IndexOf("RE") == -1);
            List<mmMastPlaning> lmmMastPlaning = new List<mmMastPlaning>();
            if (_mmMastPlaning != null)
            {
                lmmMastPlaning.Add(new mmMastPlaning
                {
                    mpDocumentNo = _mmMastPlaning.mpDocumentNo,
                    mpFlow = _mmMastPlaning.mpFlow,
                    mpStep = _mmMastPlaning.mpStep,
                });
            }

            //if(_mmMastPlaning.)

            @class._ViewSearch.v_Docno = _mmMastPlaning != null ? _mmMastPlaning.mpDocumentNo : "";
            @class._ViewSearch.V_Status = _mmMastPlaning != null ? _mmMastPlaning.mpStatus : "";


            chkEdit = chkCanEdit(@class);
            s_chkEdit = chkEdit[1].ToString();


            if (_mmMastPlaning != null)
            {
                if (_mmMastPlaning.mpFlow.ToString() == "01" && _mmMastPlaning.mpStep == 1)
                {
                    vStep = "02";
                }

            }

            //select 1
            //mmMaPlanActual
            List<mmMaActual_Risk> _mmMaActual_Risk = new List<mmMaActual_Risk>();
            _mmMaActual_Risk = _MOLD.mmMaActual_Risk.Where(x => x.arMonth == @class._ViewSearch.v_Date && x.arPlant == @class._ViewSearch.v_plant).ToList();
            var result_MaActual_Risk = from mm in _MOLD.mmMaActual_Risk
                                       where mm.arMonth == @class._ViewSearch.v_Date && mm.arPlant == @class._ViewSearch.v_plant //&&  mm.paMoldNo == "434080"
                                       select mm;
            var list_MaActual_Risk = result_MaActual_Risk.ToList();

            //join mmMast_SizeCleaning  
            List<mmMast_SizeCleaning> _mmMast_SizeCleaning = new List<mmMast_SizeCleaning>();
            _mmMast_SizeCleaning = _MOLD.mmMast_SizeCleaning.ToList();
            var result_mmMast_SizeCleaning = from mm in _MOLD.mmMast_SizeCleaning
                                             select mm;
            var list_mmMast_SizeCleaning = result_mmMast_SizeCleaning.ToList();


            //mmDetailPlaning  after add le plan
            List<mmDetailPlaning> _mmDetailPlaning = new List<mmDetailPlaning>();
            _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpPlan_DM_Month == s_date).ToList();
            var result_mmDetailPlaning = from vpa in _MOLD._mmDetailPlaning
                                         where vpa.dpPlan_DM_Month == s_date && vpa.dpDocumentNo.IndexOf("RE") == -1
                                         select vpa;
            var list_mmDetailPlaning = result_mmDetailPlaning.ToList();





           // การทำ Left Join ด้วย 2 คีย์
           var result1 = from l1 in result_MaActual_Risk
                        join l2 in _MOLD.mtMaster_Mold_Control
                        on new { Key1 = l1.arMoldName.ToString(), Key2 = l1.arModel.ToString() }
                        equals new { Key1 = l2.mcMoldname.ToString(), Key2 = l2.mcModel.ToString() } into temp
                        from l2 in temp.Where(x => x.mcSize_Mold != "").DefaultIfEmpty() // Left join
                         join l3 in list_mmMast_SizeCleaning
                         on l2.mcSize_Mold equals l3.msMold_Size into categoriesGroup
                        from mm in categoriesGroup.DefaultIfEmpty() // Left join with list3
                         select new
                        {
                            v_month = s_date,
                            v_arModel = l1.arModel,
                            v_arMoldName = l1.arMoldName,
                            v_arModelNo = l1.arMoldNo,
                            v_arMold_Control = l2.mcMold_Control,    //detail.mcMold_Control is null ? detail.mcMold_Control: "",
                             v_mc_sizeMold = l2.mcSize_Mold,
                            v_arIcsNoInj = l1.arIcsNoInj,
                            v_mc_sizeMoldcount = mm == null ? "0" : mm.msDay_Cleaning.ToString(),
                            v_mcRange = l2.mcRange,
                            v_mcProdLine = l1.arLine,
                            v_mcShortMa = l2.mcShortMa,
                            v_arLastShotQty = l1.arLastShotQty,
                            v_Plan_DM_StartDate = l1.arActual_StartDate,
                            v_Plan_DM_EndDate = l1.arActual_EndDate,
                        };

            var list_leftjoin = (from mm in result1 // outer sequence
                                 join mt in list_mmDetailPlaning
                                 on mm.v_arModelNo equals mt.dpMoldNo  // key selector 
                                 into d2
                                 from f in d2.DefaultIfEmpty()

                                 select new
                                 { // result selector 
                                     v_arModel = mm.v_arModel,
                                     v_arMoldName = mm.v_arMoldName,
                                     v_arModelNo = mm.v_arModelNo,
                                     v_arMold_Control = mm.v_arMold_Control,
                                     v_mc_sizeMold = mm.v_mc_sizeMold,
                                     v_arIcsNoInj = mm.v_arIcsNoInj,
                                     v_mc_sizeMoldcount = mm.v_mc_sizeMoldcount,
                                     v_mcRange = mm.v_mcRange,
                                     v_mcProdLine = mm.v_mcProdLine,
                                     v_mcShortMa = mm.v_mcShortMa.ToString(),
                                     v_arLastShotQty = mm.v_arLastShotQty.ToString(), //f == null ? 0 : f.dpLastShotQty ,// mm.v_arLastShotQty.ToString(),
                                     v_Plan_DM_StartDate = mm.v_Plan_DM_StartDate,
                                     v_Plan_DM_EndDate = mm.v_Plan_DM_EndDate,
                                     v_Plan_LE_StartDate = f == null ? "" : (f.dpPlan_LE_StartDate == null || f.dpPlan_LE_StartDate == "0" ? "" : f.dpPlan_LE_StartDate),
                                     v_Plan_LE_EndDate = f == null ? "" : (f.dpPlan_LE_EndDate == null ? "" : f.dpPlan_LE_EndDate),//f.dpPlan_LE_EndDate != null ? f.dpPlan_LE_EndDate.ToString() : "",

                                     v_Change_LE_StartDate = f == null ? "" : (f.dpChange_LE_StartDate == null ? "" : f.dpChange_LE_StartDate),
                                     v_Change_LE_EndDate = f == null ? "" : (f.dpChange_LE_EndDate == null ? "" : f.dpChange_LE_EndDate),

                                     v_Actual_StartDate = f == null ? "" : (f.dpActual_StartDate == null ? "" : f.dpActual_StartDate),//f.dpActual_StartDate != null ? f.dpActual_StartDate.ToString() : "",
                                     v_Actual_EndDate = f == null ? "" : (f.dpActual_EndDate == null ? "" : f.dpActual_EndDate),//f.dpActual_EndDate != null ? f.dpActual_EndDate.ToString() : "",
                                     v_RequestNo = f == null ? "" : (f.dpRequestNo == null ? "" : f.dpRequestNo),//f.dpRequestNo != null ? f.dpRequestNo.ToString() : "",
                                     v_ReqNostatus = f == null ? "" : (f.dpStatus == null ? "" : f.dpStatus),//f.dpRequestNo != null ? f.dpRequestNo.ToString() : "",
                                     v_Remark = f == null ? "" : (f.dpRemark == null ? "" : f.dpRemark),//f.dpRemark != null ? f.dpRemark.ToString() : "",
                                                                                                        // v_arLastShotQty 

                                 }).ToList();
            



      
            List<ViewMoldData> _ViewMoldData = new List<ViewMoldData>();
            for (int i = 0; i < list_leftjoin.Count(); i++)
            {

                try
                {
                    _ViewMoldData.Add(new ViewMoldData
                    {
                        v_no = (i + 1).ToString(),
                        v_arModel = list_leftjoin[i].v_arModel.ToString(),
                        v_arModelNo = list_leftjoin[i].v_arModelNo.ToString(),
                        v_arMoldName = list_leftjoin[i].v_arMoldName.ToString(),
                        v_arMold_Control = list_leftjoin[i].v_arMold_Control.ToString(),
                        v_arIcsNoInj = list_leftjoin[i].v_arIcsNoInj.ToString(),
                        v_mc_sizeMold = list_leftjoin[i].v_mc_sizeMold.ToString(),
                        v_mc_sizeMoldcount = int.Parse(list_leftjoin[i].v_mc_sizeMoldcount),
                        v_mcRange = list_leftjoin[i].v_mcRange.ToString(),
                        v_mcProdLine = list_leftjoin[i].v_mcProdLine is null ? "" : list_leftjoin[i].v_mcProdLine.ToString(),
                        v_mcShortMa = list_leftjoin[i].v_mcShortMa.ToString() == "" ? "0" : list_leftjoin[i].v_mcShortMa.ToString(),
                        v_arLastShotQty = list_leftjoin[i].v_arLastShotQty.ToString() == "" ? "0" : list_leftjoin[i].v_arLastShotQty.ToString(),

                        //Dm plan
                        v_Plan_DM_StartDate = list_leftjoin[i].v_Plan_DM_StartDate.ToString(),
                        v_Plan_DM_EndDate = list_leftjoin[i].v_Plan_DM_EndDate.ToString(),
                        v_date = int.Parse((list_leftjoin[i].v_Plan_DM_StartDate.ToString()).Substring(8, 2)), //get only date start
                        v_datePlus = int.Parse((list_leftjoin[i].v_Plan_DM_StartDate.ToString()).Substring(8, 2)) + int.Parse(list_leftjoin[i].v_mc_sizeMoldcount), //date start + date size
                        v_dateend = vStep == "02" ? int.Parse((list_leftjoin[i].v_Plan_DM_EndDate.ToString()).Substring(5, 2)) > int.Parse((list_leftjoin[i].v_Plan_DM_StartDate.ToString()).Substring(5, 2)) ? days : int.Parse((list_leftjoin[i].v_Plan_DM_EndDate.ToString()).Substring(8, 2)) : 0,

                        //Le plan
                        v_Plan_LE_StartDate = list_leftjoin[i].v_Plan_LE_StartDate != "" ? list_leftjoin[i].v_Plan_LE_StartDate : "0",// list_leftjoin[i].v_Plan_DM_StartDate.ToString(),
                        v_Plan_LE_EndDate = list_leftjoin[i].v_Plan_LE_EndDate,
                        v_date_LE_StartDate = list_leftjoin[i].v_Plan_LE_StartDate != "" ? int.Parse((list_leftjoin[i].v_Plan_LE_StartDate.ToString()).Substring(8, 2)) : 0,//int.Parse((list_leftjoin[i].v_Plan_DM_StartDate.ToString()).Substring(8, 2)),
                        v_datePlus_LE_StartDate = list_leftjoin[i].v_Plan_LE_EndDate != "" ? int.Parse((list_leftjoin[i].v_Plan_LE_EndDate.ToString()).Substring(8, 2)) : 0,


                        v_Change_LE_StartDate = list_leftjoin[i].v_Change_LE_StartDate,
                        v_Change_LE_EndDate = list_leftjoin[i].v_Change_LE_EndDate,

                        v_date_Change_LE_StartDate = list_leftjoin[i].v_Change_LE_StartDate != "" ? int.Parse((list_leftjoin[i].v_Change_LE_StartDate.ToString()).Substring(8, 2)) : 0,
                        v_datePlus_v_Change_LE_EndDate = list_leftjoin[i].v_Change_LE_EndDate != "" ? int.Parse((list_leftjoin[i].v_Change_LE_EndDate.ToString()).Substring(8, 2)) : 0,


                        //actual plan
                        v_Actual_StartDate = list_leftjoin[i].v_Actual_StartDate,
                        v_Actual_EndDate = list_leftjoin[i].v_Actual_EndDate,
                        v_date_Actual_StartDate = list_leftjoin[i].v_Actual_StartDate != "" && list_leftjoin[i].v_Actual_StartDate != "0" ? int.Parse((list_leftjoin[i].v_Actual_StartDate.ToString()).Substring(8, 2)) : 0,
                        v_datePlus_Actual_EndDate = list_leftjoin[i].v_Actual_StartDate != "" && list_leftjoin[i].v_Actual_StartDate != "0" ? int.Parse((list_leftjoin[i].v_Actual_StartDate.ToString()).Substring(8, 2)) + int.Parse(list_leftjoin[i].v_mc_sizeMoldcount) : 0, //date start + date size
                                                                                                                                                                                                                                                                                //v_datePlus_Actual_EndDate =13,
                                                                                                                                                                                                                                                                                //v_date_Actual_StartDate = list_leftjoin[i].v_Actual_StartDate != "" ? int.Parse((list_leftjoin[i].v_Actual_StartDate.ToString()).Substring(8, 2)) : int.Parse((list_leftjoin[i].v_Actual_StartDate.ToString()).Substring(8, 2)),
                                                                                                                                                                                                                                                                                // v_datePlus_Actual_EndDate = list_leftjoin[i].v_Actual_EndDate != "" ? int.Parse((list_leftjoin[i].v_Actual_EndDate.ToString()).Substring(8, 2)) : 32,


                        v_RequestNo = list_leftjoin[i].v_RequestNo,
                        v_ReqNostatus = list_leftjoin[i].v_ReqNostatus,

                        v_Remark = list_leftjoin[i].v_Remark,

                    });
                }
                catch (Exception ex)
                {

                }


            }

            @class.List_ViewMoldData = _ViewMoldData.ToList();
            ViewBag.ViewMoldData = _ViewMoldData;
            ViewBag.ViewMoldData_iCount = _ViewMoldData.Count();
            ViewBag.View_status_Data = @class._ViewSearch.V_Status;
            ViewBag.View_MastPlaning_mpFlow = lmmMastPlaning;//status
            //ViewBag.ViewMoldData_count = "Total Result : " + _ViewMoldData.Count().ToString() + " Rows.";
            ViewBag.ViewMoldData_count = _ViewMoldData.Count().ToString();
            ViewBag.chkEdit = s_chkEdit;
            ViewBag.vPage = @class._ViewSearch.v_page;
            ViewBag.Docno = @class._ViewSearch.v_Docno;
            ViewBag.Month = @class._ViewSearch.v_Date;
            // @classs._ViewSearch.v_page
            return View("Index", @class);

        }

        public String get_Month(String n_month)
        {
            var s_month = "";
            int i_month = int.Parse(n_month);
            switch (i_month)
            {
                case 1:
                    s_month = "January";
                    break;
                case 2:
                    s_month = "February";
                    break;
                case 3:
                    s_month = "March";
                    break;
                case 4:
                    s_month = "April";
                    break;
                case 5:
                    s_month = "May";
                    break;
                case 6:
                    s_month = "June";
                    break;
                case 7:
                    s_month = "July";
                    break;
                case 8:
                    s_month = "August";
                    break;
                case 9:
                    s_month = "September";
                    break;
                case 10:
                    s_month = "October";
                    break;
                case 11:
                    s_month = "November";
                    break;
                case 12:
                    s_month = "December";
                    break;
                default:
                    s_month = "There are only 12 months in a year";
                    break;
            }

            return s_month;

        }

        public ActionResult Search(string term)
        {
            {
                // return Json(_IT.Email.Where(p => p.emEmail.Contains(term)).Select(p => p.emEmail).ToList());
                return Json(_IT.Email.Where(p => p.emName_M365.Contains(term)).Select(p => p.emEmail_M365 + " [" + p.emName_M365 + "-" + p.emDeptCode + "]").ToList());
            }
        }

        public JsonResult History(string id, string mode, string vplant, string vdate)
        {
            string partialUrl = Url.Action("SendMail", "Home", new { docno = id, mode, vplant = vplant, vdate = vdate });
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
            Class model = new Class();
            var v_muDeptCode = "";
            var v_ccemail = "";
            mmHistoryApproved _mmHistoryApproved = new mmHistoryApproved();
            mmMastUserApprove _mmMastUserApprove = new mmMastUserApprove();
            List<Viewemail> _Viewemail = new List<Viewemail>();

            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == docno).FirstOrDefault();

            v_muDeptCode = _mmMastPlaning != null && _mmMastPlaning.mpStep == 1 ? v_muDeptCode = "DMM" : v_muDeptCode = vplant;
            var v_mmMastUserApprove = _mmMastPlaning != null ? _mmMastPlaning.mpStep == 1 ? _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muEmpCode == _mmMastPlaning.mpEmpcode_Issue).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == vplant).ToList();

            //   ? _mmMastPlaning.mpStep == 1 ? _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == 'DMM' && x.muEmpCode == _mmMastPlaning.mpEmpcode_Issue).ToList() : _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == v_muDeptCode).ToList() : v_muDeptCode = vplant;
            if (v_mmMastUserApprove.Count > 0)
            {

                for (int i = 0; i <= v_mmMastUserApprove.Count() - 1; i++)
                {
                    //var v_emailFrom = _IT.Email.Where(x => x.emEmpcode == v_mmMastUserApprove[i].muEmpCode.ToString()).Select(x => x.emEmail).FirstOrDefault(); // lotus note
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
                if (_mmMastPlaning.mpStep == 0)
                {
                    //dmm => lamp vplant
                    // var depTo = _MOLD._mmMastUserApprove.Where(x => x.muEmpCode == v_user).Select(x => x.muDeptCode).FirstOrDefault();
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
                else
                {
                    //lamp => dmm
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
            }
            else
            {
                //dmm => lamp vplant
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


            //add cc
            //step 0 to cc dmm
            if (_mmMastPlaning != null)
            {
                if (_mmMastPlaning.mpStep == 0) //dmm => lamp
                {

                    var depcc = _MOLD._mmMastUserApprove.Where(x => x.muEmpCode == v_user).Select(x => x.muDeptCode).FirstOrDefault();
                    var ccLamp = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == depcc).ToList();
                    for (int i = 0; i <= ccLamp.Count() - 1; i++)
                    {
                        var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == ccLamp[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                        v_ccemail += v_emailCCFrom + ",";
                    }
                }
                else if (_mmMastPlaning.mpStep == 1) // lamp==> dmm
                {
                    v_ccemail = "";
                    var v_Tomail = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muOperator != "main").ToList();
                    for (int i = 0; i <= v_Tomail.Count() - 1; i++)
                    {
                        var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == v_Tomail[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                        v_ccemail += v_emailCCFrom + ",";
                    }
                }
            }
            else //dmm => lamp
            {
                v_ccemail = "";
                var v_Tomail = _MOLD._mmMastUserApprove.Where(x => x.muDeptCode == "DMM" && x.muOperator != "main").ToList();
                for (int i = 0; i <= v_Tomail.Count() - 1; i++)
                {
                    var v_emailCCFrom = _IT.Email.Where(x => x.emEmpcode == v_Tomail[i].muEmpCode.ToString()).Select(p => p.emEmail_M365).FirstOrDefault(); //chg to m365
                    v_ccemail += v_emailCCFrom + ",";
                }

            }



            //step 1 to cc lamp

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
            //var v_Issue = _IT.Email.Where(x => x.emEmpcode == v_Empcode).Select(x => x.emEmail).FirstOrDefault(); //lotus note 
            var v_Issue = _IT.Email.Where(x => x.emEmpcode == v_Empcode).Select(x => x.emEmail_M365).FirstOrDefault(); //m365

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
                    //_mmHistoryApproved.htFrom = _IT.Email.Where(x => x.emEmpcode == _mmHistoryApproved.htFrom.ToString()).Select(x => x.emEmail).FirstOrDefault(); //lotus note
                    _mmHistoryApproved.htFrom = _IT.Email.Where(x => x.emEmpcode == _mmHistoryApproved.htFrom.ToString()).Select(x => x.emEmail_M365).FirstOrDefault(); // m365
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
            ViewBag.UserApprove = s_emailTo;//s_emailFrom;
            // return PartialView("SendMail", _qaHistoryApproved);
            //@ViewBag.ViewTest = "1111111223344"; 

            return PartialView("SendMail", _mmHistoryApproved);

        }
        [HttpPost]
        // public JsonResult SendMail_post(string vmode, string vdocno, string vplant, string vdate, Class @class, mmHistoryApproved _mmHistoryApproved)//{string mode, ViewModel model, qaHistoryApproved _qaHistoryApproved)
        public JsonResult SendMail_post(string vmode, string vplant, Class @class, mmHistoryApproved _mmHistoryApproved)
        {
            //string getDocNO = "";
            string config = "";
            string msg = "";
            @class._ViewSearch.v_plant = vplant;
            string[] chkPermis;
            string[] chkCountPlan;


            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
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

            //check all
            //chkCountPlan = chkConfrimPlan(@class);
            //if (chkCountPlan[1].ToString() == "No")
            //{

            //    msg = "Plase confirm plan all items completed !!!!!";
            //    config = "P";
            //    ViewBag.Config = "P";
            //    TempData["Config"] = ViewBag.Config;
            //    TempData["Msg"] = ViewBag.Msg;

            //    return Json(new { c1 = config, c2 = msg });
            //}

            string[] getDocNO;
            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
            getDocNO = Save(vmode, @class);
            _mmHistoryApproved.htDocumentNo = getDocNO[1]; // getDocNO; //getDocNO[1];
            if (!(int.Parse(getDocNO[0]) > 0)) { return Json(new { c1 = "E", c2 = getDocNO[1] }); }

            ////   ViewlrBuiltDrawing _ViewlrBuiltDrawing = new ViewlrBuiltDrawing();
            var email = new MimeMessage();
            int i_Step = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == _mmHistoryApproved.htDocumentNo).Select(x => x.mpStep).FirstOrDefault();
            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == _mmHistoryApproved.htDocumentNo).FirstOrDefault();



            if (_mmHistoryApproved.htTo != null || (_mmHistoryApproved.htTo == null && _mmHistoryApproved.htStatus == "Disapprove"))
            {
                if (_mmHistoryApproved.htStatus == "Approve")
                {
                    i_Step = i_Step + 1;//  _mmHistoryApproved.htStep + 1;

                    var v_subject = _MOLD.mmMastFlowApprove.Where(x => x.mfStep == i_Step && x.mfFlowNo == _mmMastPlaning.mpFlow).Select(x => x.mfSubject).FirstOrDefault();

                    _mmHistoryApproved.htStatus = v_subject;

                    //        //var v_Issue = _IT.rpEmail.Where(x => x.emEmpcode == _ViewlrAssetClaim.acEmpCodeReq).Select(x => x.emEmail).First();
                    //        //_qaHistoryApproved.htTo = v_Issue;

                    config = "S";
                }
                else if (_mmHistoryApproved.htStatus == "Disapprove")
                {
                    i_Step = 0;

                    var v_Issue = _IT.Email.Where(x => x.emEmpcode == _mmMastPlaning.mpEmpcode_Issue).Select(x => x.emEmail_M365).First(); // m365
                                                                                                                                           // var v_Issue = _IT.Email.Where(x => x.emEmpcode == _mmMastPlaning.mpEmpcode_Issue).Select(x => x.emEmail).First(); //lutus note

                    //        //_HistoryApproved.htStatus = _HistoryApproved.htStatus;
                    _mmHistoryApproved.htTo = v_Issue;
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
                _mmHistoryApproved.htTo = _mmHistoryApproved.htTo != null ? _mmHistoryApproved.htTo.Split(" ")[0] : _mmHistoryApproved.htTo; //split ""

                //var v_Empcode_Issue = _IT.Email.Where(x => x.emEmpcode == _mmMastPlaning.mpEmpcode_Issue).Select(x => x.emEmail).First(); //lotusnote
                //var v_ApproveBy = _IT.Email.Where(x => x.emEmail == _mmHistoryApproved.htTo).Select(x => x.emEmpcode).First();//lotusnote

                var v_Empcode_Issue = _IT.Email.Where(x => x.emEmpcode == _mmMastPlaning.mpEmpcode_Issue).Select(x => x.emEmail_M365).First();
                var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emEmpcode).First();

                var v_emName_M365 = _IT.Email.Where(x => x.emEmail_M365 == _mmHistoryApproved.htTo).Select(x => x.emName_M365).First();



                ViewAccEMPLOYEE acc = _HRMS.AccEMPLOYEE.FirstOrDefault(s => s.EMP_CODE == v_ApproveBy);

                _mmHistoryApproved.htStep = i_Step;
                _mmHistoryApproved.htDate = DateTime.Now.ToString("yyyy/MM/dd");
                _mmHistoryApproved.htTime = DateTime.Now.ToString("HH:mm:ss");

                _mmMastPlaning.mpStep = _mmHistoryApproved.htStep;
                _mmMastPlaning.mpStatus = _mmHistoryApproved.htStatus;
                _mmMastPlaning.mpEmpcode_Approve = v_ApproveBy;
                _mmMastPlaning.mpName_Approve = acc.NICKNAME.ToString();

                _MOLD.mmHistoryApproved.Add(_mmHistoryApproved);

                email.Subject = "Period Maintenance Mold Planning ==> " + _mmMastPlaning.mpStatus.ToString();

                MailboxAddress Formmail365 = new MailboxAddress(v_emName_M365, _mmHistoryApproved.htFrom);
                email.From.Add(Formmail365);
                // email.From.Add(MailboxAddress.Parse(_mmHistoryApproved.htFrom));
                email.To.Add(MailboxAddress.Parse(_mmHistoryApproved.htTo));
                // email.To.Add(MailboxAddress.Parse(v_Empcode_Issue));

                //email.From.Add(MailboxAddress.Parse(_qaHistoryApproved.htFrom));
                //email.To.Add(MailboxAddress.Parse(_qaHistoryApproved.htTo));


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


                for (int i = 0; i <= @class.List_ViewMoldData.Count - 1; i++)
                {
                    var _mmDetailPlaningchk = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpDocumentNo == _mmHistoryApproved.htDocumentNo && x.dpMoldNo == @class.List_ViewMoldData[i].v_arModelNo.ToString());
                    var P_le_StartDate = @class.List_ViewMoldData[i].v_Plan_LE_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_LE_StartDate : "";
                    var P_le_EndDate = @class.List_ViewMoldData[i].v_Plan_LE_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_LE_EndDate : "";
                    if (P_le_StartDate != "")
                    {
                        if (@class._ViewSearch.V_Status == "Finished") //check status
                        {

                        }
                        else if (@class.List_ViewMoldData[i].v_Plan_LE_StartDate.ToString() == "0")
                        {

                        }
                        else
                        {
                            if (@class.List_ViewMoldData[i].v_Plan_LE_StartDate.ToString().Length > 8)
                            {
                                P_le_StartDate = @class.List_ViewMoldData[i].v_Plan_LE_StartDate.Substring(8, 2);
                            }
                            P_le_StartDate = @class._ViewSearch.v_Date.ToString() + "/" + (int.Parse(P_le_StartDate).ToString("00")).ToString();
                            int s_dp = int.Parse(@class.List_ViewMoldData[i].v_mc_sizeMoldcount.ToString()) - 1;
                            if (DateTime.Parse(P_le_StartDate) > DateTime.Parse(@class.List_ViewMoldData[i].v_Plan_DM_EndDate))
                            {

                                P_le_StartDate = ((DateTime.Parse(P_le_StartDate)).AddDays(-s_dp)).ToString("yyyy/MM/dd");
                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                // @class.List_ViewMoldData[i].v_datePlus
                            }
                            else if (P_le_StartDate == @class.List_ViewMoldData[i].v_Plan_DM_EndDate)
                            {

                                //P_le_StartDate = ((DateTime.Parse(P_le_StartDate)).AddDays(-s_dp)).ToString("yyyy/MM/dd");
                                //P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");

                                P_le_StartDate = DateTime.Parse(P_le_StartDate).ToString("yyyy/MM/dd");
                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                // @class.List_ViewMoldData[i].v_datePlus
                            }
                            else
                            {

                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                // @class.List_ViewMoldData[i].v_datePlus
                            }
                        }

                    }

                    if (_mmDetailPlaningchk == null)
                    {
                        //@class.List_ViewMoldData[i].v_Plan_DM_EndDate
                        //var P_le_EndDate = s_date + "/" + (int.Parse(@class.List_ViewMoldData[i].v_Plan_LE_StartDate).ToString("00")).ToString();
                        //@class.List_ViewMoldData[i].v_Plan_LE_EndDate
                        mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning()
                        {
                            dpDocumentNo = _mmHistoryApproved.htDocumentNo,
                            dpMoldNo = @class.List_ViewMoldData[i].v_arModelNo,
                            dpLine = @class.List_ViewMoldData[i].v_mcProdLine,
                            dpMoldName = @class.List_ViewMoldData[i].v_arMoldName,
                            dpModel = @class.List_ViewMoldData[i].v_arModel,
                            dpIcsNoInj = @class.List_ViewMoldData[i].v_arIcsNoInj,
                            dpPlan_DM_StartDate = @class.List_ViewMoldData[i].v_Plan_DM_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_StartDate : "",
                            dpPlan_DM_EndDate = @class.List_ViewMoldData[i].v_Plan_DM_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_EndDate : "",
                            dpPlan_LE_StartDate = P_le_StartDate,
                            dpPlan_LE_EndDate = P_le_EndDate,
                            dpActual_StartDate = @class.List_ViewMoldData[i].v_Actual_StartDate != null ? @class.List_ViewMoldData[i].v_Actual_StartDate : "",
                            dpActual_EndDate = @class.List_ViewMoldData[i].v_Actual_EndDate != null ? @class.List_ViewMoldData[i].v_Actual_EndDate : "",
                            dpLastShotQty = int.Parse(@class.List_ViewMoldData[i].v_arLastShotQty),
                            dpRequestNo = @class.List_ViewMoldData[i].v_RequestNo,
                            dpRemark = @class.List_ViewMoldData[i].v_Remark,
                            dpPlan_DM_Month = @class._ViewSearch.v_Date, //yyyy/MM


                        };
                        _MOLD._mmDetailPlaning.Add(_mmDetailPlaning);
                        _MOLD.SaveChanges();
                    }

                    else
                    {

                        mmDetailPlaning _mmDetailPlaning1 = new mmDetailPlaning();
                        _mmDetailPlaning1 = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpDocumentNo == _mmHistoryApproved.htDocumentNo && x.dpMoldNo == @class.List_ViewMoldData[i].v_arModelNo.ToString());
                        //dpDocumentNo = rundoc.ToString(),
                        //dpMoldNo = @class.List_ViewMoldData[i].v_arModelNo,
                        _mmDetailPlaning1.dpLine = @class.List_ViewMoldData[i].v_mcProdLine;
                        _mmDetailPlaning1.dpMoldName = @class.List_ViewMoldData[i].v_arMoldName;
                        _mmDetailPlaning1.dpModel = @class.List_ViewMoldData[i].v_arModel;
                        _mmDetailPlaning1.dpIcsNoInj = @class.List_ViewMoldData[i].v_arIcsNoInj;
                        _mmDetailPlaning1.dpPlan_DM_StartDate = @class.List_ViewMoldData[i].v_Plan_DM_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_StartDate : "";
                        _mmDetailPlaning1.dpPlan_DM_EndDate = @class.List_ViewMoldData[i].v_Plan_DM_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_EndDate : "";
                        _mmDetailPlaning1.dpPlan_LE_StartDate = P_le_StartDate;
                        _mmDetailPlaning1.dpPlan_LE_EndDate = P_le_EndDate;
                        _mmDetailPlaning1.dpActual_StartDate = @class.List_ViewMoldData[i].v_Actual_StartDate != null ? @class.List_ViewMoldData[i].v_Actual_StartDate : "";
                        _mmDetailPlaning1.dpActual_EndDate = @class.List_ViewMoldData[i].v_Actual_EndDate != null ? @class.List_ViewMoldData[i].v_Actual_EndDate : "";
                        _mmDetailPlaning1.dpLastShotQty = @class.List_ViewMoldData[i].v_arLastShotQty != null ? int.Parse(@class.List_ViewMoldData[i].v_arLastShotQty) : 0;
                        _mmDetailPlaning1.dpRequestNo = @class.List_ViewMoldData[i].v_RequestNo;
                        _mmDetailPlaning1.dpRemark = @class.List_ViewMoldData[i].v_Remark;
                        _mmDetailPlaning1.dpPlan_DM_Month = @class._ViewSearch.v_Date; //yyyy/MM
                        _MOLD._mmDetailPlaning.Update(_mmDetailPlaning1);
                        _MOLD.SaveChanges();
                    }

                }



                var varifyUrl = "http://thsweb/MVCPublish/MoldMaintenance/Home/Login?mode=edit&DocumentNo=" + getDocNO[1] + "&UserID=" + v_ApproveBy + "&Plant=" + vplant + "&Date=" + @class._ViewSearch.v_Date;
                var bodyBuilder = new BodyBuilder();
                string EmailBody = "";
                //    //var image = bodyBuilder.LinkedResources.Add(@"E:\01_My Document\02_Project\_2023\1. PartTransferUnbalance\PartTransferUnbalance\wwwroot\images\btn\OK.png");
                EmailBody = $"<div>" +
                $"<B>PERIOD MAINTENANCE MOLD PLANING : </B>" + "" + "<br>" +
                $"Document No : " + "" + getDocNO[1] + "" + "<br>" +
                $"Plant : " + "" + vplant + "" + "<br>" +
                $"Month : " + "" + @class._ViewSearch.v_Date + "" + "<br>" +
                $"Status :" + "" + " " + _mmMastPlaning.mpStatus.ToString() + "<br>" +
                //$"<a href='" + varifyUrl + "'>" +
                $"<a href='" + varifyUrl + "'> More Detail" +
                //$"<img src = 'http://thsweb/MVCPublish/QA_APPROVAL_REQUEST/images/btn/mail1.png' alt = 'HTML tutorial' style = 'width: 50px; height: 50px;'>" +
                $"</a>" +
                $"</div>";


                //    // bodyBuilder.Attachments.Add(@"E:\01_My Document\02_Project\_2023\1. PartTransferUnbalance\PartTransferUnbalance\dev_rfc.log");

                bodyBuilder.HtmlBody = string.Format(EmailBody);
                email.Body = bodyBuilder.ToMessageBody();

                // send email
                var smtp = new SmtpClient();
                //smtp.Connect("10.200.128.12");
                smtp.Connect("203.146.237.138");
                smtp.Send(email);
                smtp.Disconnect(true);



                return Json(new { c1 = config, c2 = msg });
            }
            else
            {

                //if (config == "E")
                //{
                config = "E";
                ViewBag.Config = "E";
                TempData["Config"] = ViewBag.Config;
                TempData["Msg"] = ViewBag.Msg;

                return Json(new { c1 = config, c2 = msg });
                //}

            }
            //config = "S";
            //return Json(new { c1 = config, c2 = msg });
        }

        public string[] Save(string mode, Class @class)
        {
            string s_issue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " - " + HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_name = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            string s_dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value; // dep
            string s_issuedate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            var s_plant = @class._ViewSearch.v_plant;
            var s_date = @class._ViewSearch.v_Date;
            var s_Docno = @class._ViewSearch.v_Docno;
            var a_month = "";
            var a_year = "";
            string[] a_dateMonth;
            if (s_date != null)
            {
                a_dateMonth = s_date.Split("/");
                a_month = a_dateMonth[1];
                a_year = a_dateMonth[0].Substring(2, 2);
            }

            var rundoc = "";
            int runNo = 0;



            try
            {
                if (s_Docno != null)
                {
                    runNo = int.Parse(s_Docno.Substring(8, 3));
                    rundoc = s_Docno;
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            var _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(x => x.mpDocumentNo == rundoc);
                            _mmMastPlaning.mpDocumentNo = rundoc;
                            _mmMastPlaning.mpMonth = s_date;
                            _mmMastPlaning.mpPlant = s_plant;
                            //_mmMastPlaning.//mpFlow = "01",
                            //_mmMastPlaning.//mpStep = 0,
                            //_mmMastPlaning.//mpStatus = "Create Document",
                            //_mmMastPlaning.mpEmpcode_Issue = s_empcode;
                            //_mmMastPlaning.mpName_Issue = s_name;
                            //_mmMastPlaning.mpIssueDate = s_issuedate;
                            _mmMastPlaning.mpIssueDept = s_dept;
                            _MOLD.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }
                    }

                }
                else
                {
                    var result_mmRunDocument = from mm in _MOLD.mmRunDocument
                                               where mm.rmPlant == s_plant && mm.rmYear == a_year && mm.rmGroup == "MP"
                                               select mm;
                    var list_mmRunDocument = result_mmRunDocument.ToList().OrderByDescending(x => x.rmRunNo).FirstOrDefault();
                    if (list_mmRunDocument == null)
                    {
                        rundoc = "MP" + s_plant + a_year + ((1).ToString("000"));
                        runNo = 1;
                    }
                    else
                    {
                        rundoc = list_mmRunDocument.rmGroup + list_mmRunDocument.rmPlant + list_mmRunDocument.rmYear + (int.Parse(list_mmRunDocument.rmRunNo.ToString()) + 1).ToString("000");
                        runNo = int.Parse(list_mmRunDocument.rmRunNo.ToString()) + 1;
                    }

                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            mmRunDocument _mmRunDocument = new mmRunDocument()
                            {
                                rmRunNo = runNo,// list_mmRunDocument.rmRunNo + 1,
                                rmPlant = s_plant,
                                rmYear = a_year,
                                rmGroup = "MP",
                                rmIssueBy = s_issue,
                                rmUpdateBy = s_issue,
                            };
                            _MOLD.mmRunDocument.Add(_mmRunDocument);
                            _MOLD.SaveChanges();

                            mmMastPlaning _mmMastPlaning = new mmMastPlaning()
                            {
                                mpDocumentNo = rundoc,
                                mpMonth = s_date,
                                mpPlant = s_plant,
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

                            //_MOLD.mmMastPlaning.Update(_mmMastPlaning);
                            dbContextTransaction.Commit();

                        }
                        catch (Exception e)
                        {

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

        //check permission
        public string[] chkPermission(Class @class, string mode)
        {
            var vPermission = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.System)?.Value;
            var doc = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Dns)?.Value; //doc no edit
            var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Actor)?.Value;//user edit
            var dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;//dept edit
            var userlogin = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value; //emp login
            var s_Docno = @class._ViewSearch.v_Docno;
            var v_per = "";
            var m_per = "";
            try
            {
                //create new
                //wait 
                //finish
                if (s_Docno == null)
                {

                    if (vPermission != "admin")
                    {
                        v_per = "No";
                        m_per = "You don't have permission to access'";
                    }
                }
                else
                {
                    var v_Step = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_Docno).OrderByDescending(x => x.htStep).Select(x => x.htStep).FirstOrDefault();
                    if (v_Step == 0)
                    {
                        if (vPermission != "admin")
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
                    else
                    {
                        var v_From = _MOLD.mmHistoryApproved.Where(x => x.htDocumentNo == s_Docno && x.htStep == v_Step).Select(x => x.htTo).FirstOrDefault();
                        // var v_ApproveBy = _IT.Email.Where(x => x.emEmail == (v_From != null ? v_From.ToString() : "")).Select(x => x.emEmpcode).First(); //lotus note
                        var v_ApproveBy = _IT.Email.Where(x => x.emEmail_M365 == (v_From != null ? v_From.ToString() : "")).Select(x => x.emEmpcode).First(); //m365
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
            var s_Docno = @class._ViewSearch.v_Docno;
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
        [HttpPost]
        public ActionResult SaveData(Class @class)
        {
            // HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            string s_issue = DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " - " + HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            string s_name = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            string s_dept = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value; // dep
            string s_issuedate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
            var s_plant = @class._ViewSearch.v_plant;
            var s_date = @class._ViewSearch.v_Date;
            var s_Docno = @class._ViewSearch.v_Docno;
            var s_Status = @class._ViewSearch.V_Status;
            var a_month = "";
            var a_year = "";
            string[] a_dateMonth;
            string config = "";
            string msg = "";
            var rundoc = "";
            int runNo = 0;
            string[] chkPermis;
            // getDocNO = @class._ViewSearch.v_Docno;// vDocno;
            chkPermis = chkPermission(@class, "Save");
            if (chkPermis[1].ToString() == "No")
            {
                config = "N";
                msg = "You don't have permission to access";
                return Json(new
                {
                    c1 = config,
                    c2 = msg,
                    c3 = @class,
                    c4 = s_Docno
                });
            }


            int TcountMold = @class.List_ViewMoldData != null ? @class.List_ViewMoldData.Count() : 0;
            if (s_date != null)
            {
                a_dateMonth = s_date.Split("/");
                a_month = a_dateMonth[1];
                a_year = a_dateMonth[0].Substring(2, 2);
            }




            //check permission

            var result_MaActual_Risk = from mm in _MOLD.mmMaActual_Risk
                                       where mm.arMonth == s_date && mm.arPlant == s_plant //&&  mm.paMoldNo == "434080"
                                       select mm;
            var list_MaActual_Risk = result_MaActual_Risk.ToList();
            if (list_MaActual_Risk.Count() > 0 && TcountMold > 0)
            {
                if (s_Docno != null)
                {
                    runNo = int.Parse(s_Docno.Substring(8, 3));
                    rundoc = s_Docno;
                }
                else
                {
                    //get run doc
                    var result_mmRunDocument = from mm in _MOLD.mmRunDocument
                                               where mm.rmPlant == s_plant && mm.rmYear == a_year && mm.rmGroup == "MP"
                                               select mm;
                    var list_mmRunDocument = result_mmRunDocument.ToList().OrderByDescending(x => x.rmRunNo).FirstOrDefault();
                    if (list_mmRunDocument == null)
                    {
                        rundoc = "MP" + s_plant + a_year + ((1).ToString("000"));
                        runNo = 1;
                    }
                    else
                    {
                        rundoc = list_mmRunDocument.rmGroup + list_mmRunDocument.rmPlant + list_mmRunDocument.rmYear + (int.Parse(list_mmRunDocument.rmRunNo.ToString()) + 1).ToString("000");
                        runNo = int.Parse(list_mmRunDocument.rmRunNo.ToString()) + 1;
                    }

                }

                mmMastPlaning _chk_mmMastPlaning = new mmMastPlaning();
                var query = from mm in _MOLD.mmMastPlaning
                            where mm.mpDocumentNo == rundoc
                            select mm;

                _MOLD._list_mmMastPlaning = query.ToList();

                s_Status = _MOLD._list_mmMastPlaning.Count() > 0 ? _MOLD._list_mmMastPlaning[0].mpStatus : "Create Document";
                //update
                if (s_Docno != null)
                {


                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            var _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(x => x.mpDocumentNo == rundoc);
                            _mmMastPlaning.mpDocumentNo = rundoc;
                            _mmMastPlaning.mpMonth = s_date;
                            _mmMastPlaning.mpPlant = s_plant;

                            //_mmMastPlaning.//mpFlow = "01",
                            //_mmMastPlaning.//mpStep = 0,
                            //_mmMastPlaning.//mpStatus = "Create Document",
                            // _mmMastPlaning.mpEmpcode_Issue = s_empcode;
                            // _mmMastPlaning.mpName_Issue = s_name;
                            _mmMastPlaning.mpIssueDate = s_issuedate;
                            _mmMastPlaning.mpIssueDept = s_dept;

                            _MOLD.SaveChanges();



                            //add mmDetailPlaning case add 
                            if (@class.List_ViewMoldData.Count > 0)
                            {
                                for (int i = 0; i <= @class.List_ViewMoldData.Count - 1; i++)
                                {
                                    var _mmDetailPlaningchk = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpDocumentNo == rundoc && x.dpMoldNo == @class.List_ViewMoldData[i].v_arModelNo.ToString());
                                    var P_le_StartDate = @class.List_ViewMoldData[i].v_Plan_LE_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_LE_StartDate : "";
                                    var P_le_EndDate = @class.List_ViewMoldData[i].v_Plan_LE_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_LE_EndDate : "";
                                    if (P_le_StartDate != "")
                                    {
                                        if (@class._ViewSearch.V_Status == "Finished") //check status
                                        {

                                        }
                                        else
                                        {
                                            if (@class.List_ViewMoldData[i].v_Plan_LE_StartDate.ToString().Length > 8)
                                            {
                                                P_le_StartDate = @class.List_ViewMoldData[i].v_Plan_LE_StartDate.Substring(8, 2);
                                            }

                                            P_le_StartDate = s_date + "/" + (int.Parse(P_le_StartDate).ToString("00")).ToString();
                                            int s_dp = int.Parse(@class.List_ViewMoldData[i].v_mc_sizeMoldcount.ToString()) - 1;

                                            if (DateTime.Parse(P_le_StartDate) > DateTime.Parse(@class.List_ViewMoldData[i].v_Plan_DM_EndDate))
                                            {

                                                P_le_StartDate = ((DateTime.Parse(P_le_StartDate)).AddDays(-s_dp)).ToString("yyyy/MM/dd");
                                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                                // @class.List_ViewMoldData[i].v_datePlus
                                            }
                                            else if (P_le_StartDate == @class.List_ViewMoldData[i].v_Plan_DM_EndDate)
                                            {
                                                P_le_StartDate = DateTime.Parse(P_le_StartDate).ToString("yyyy/MM/dd");
                                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                            }

                                            else
                                            {

                                                P_le_EndDate = ((DateTime.Parse(P_le_StartDate)).AddDays(s_dp)).ToString("yyyy/MM/dd");
                                                // @class.List_ViewMoldData[i].v_datePlus
                                            }
                                        }

                                    }

                                    if (_mmDetailPlaningchk == null)
                                    {
                                        //@class.List_ViewMoldData[i].v_Plan_DM_EndDate


                                        //var P_le_EndDate = s_date + "/" + (int.Parse(@class.List_ViewMoldData[i].v_Plan_LE_StartDate).ToString("00")).ToString();
                                        //@class.List_ViewMoldData[i].v_Plan_LE_EndDate
                                        mmDetailPlaning _mmDetailPlaning = new mmDetailPlaning()
                                        {
                                            dpDocumentNo = rundoc.ToString(),
                                            dpMoldNo = @class.List_ViewMoldData[i].v_arModelNo,
                                            dpLine = @class.List_ViewMoldData[i].v_mcProdLine,
                                            dpMoldName = @class.List_ViewMoldData[i].v_arMoldName,
                                            dpModel = @class.List_ViewMoldData[i].v_arModel,
                                            dpIcsNoInj = @class.List_ViewMoldData[i].v_arIcsNoInj,
                                            dpPlan_DM_StartDate = @class.List_ViewMoldData[i].v_Plan_DM_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_StartDate : "",
                                            dpPlan_DM_EndDate = @class.List_ViewMoldData[i].v_Plan_DM_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_EndDate : "",
                                            dpPlan_LE_StartDate = P_le_StartDate,
                                            dpPlan_LE_EndDate = P_le_EndDate,
                                            dpActual_StartDate = @class.List_ViewMoldData[i].v_Actual_StartDate != null ? @class.List_ViewMoldData[i].v_Actual_StartDate : "",
                                            dpActual_EndDate = @class.List_ViewMoldData[i].v_Actual_EndDate != null ? @class.List_ViewMoldData[i].v_Actual_EndDate : "",
                                            dpLastShotQty = int.Parse(@class.List_ViewMoldData[i].v_arLastShotQty),
                                            dpRequestNo = @class.List_ViewMoldData[i].v_RequestNo,
                                            dpRemark = @class.List_ViewMoldData[i].v_Remark,
                                            dpPlan_DM_Month = s_date, // year/month


                                        };
                                        _MOLD._mmDetailPlaning.Add(_mmDetailPlaning);
                                        _MOLD.SaveChanges();
                                    }

                                    else
                                    {

                                        mmDetailPlaning _mmDetailPlaning1 = new mmDetailPlaning();
                                        _mmDetailPlaning1 = _MOLD._mmDetailPlaning.FirstOrDefault(x => x.dpDocumentNo == rundoc && x.dpMoldNo == @class.List_ViewMoldData[i].v_arModelNo.ToString());
                                        //dpDocumentNo = rundoc.ToString(),
                                        //dpMoldNo = @class.List_ViewMoldData[i].v_arModelNo,
                                        _mmDetailPlaning1.dpLine = @class.List_ViewMoldData[i].v_mcProdLine;
                                        _mmDetailPlaning1.dpMoldName = @class.List_ViewMoldData[i].v_arMoldName;
                                        _mmDetailPlaning1.dpModel = @class.List_ViewMoldData[i].v_arModel;
                                        _mmDetailPlaning1.dpIcsNoInj = @class.List_ViewMoldData[i].v_arIcsNoInj;
                                        _mmDetailPlaning1.dpPlan_DM_StartDate = @class.List_ViewMoldData[i].v_Plan_DM_StartDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_StartDate : "";
                                        _mmDetailPlaning1.dpPlan_DM_EndDate = @class.List_ViewMoldData[i].v_Plan_DM_EndDate != null ? @class.List_ViewMoldData[i].v_Plan_DM_EndDate : "";
                                        _mmDetailPlaning1.dpPlan_LE_StartDate = P_le_StartDate;
                                        _mmDetailPlaning1.dpPlan_LE_EndDate = P_le_EndDate;
                                        _mmDetailPlaning1.dpActual_StartDate = @class.List_ViewMoldData[i].v_Actual_StartDate != null ? @class.List_ViewMoldData[i].v_Actual_StartDate : "";
                                        _mmDetailPlaning1.dpActual_EndDate = @class.List_ViewMoldData[i].v_Actual_EndDate != null ? @class.List_ViewMoldData[i].v_Actual_EndDate : "";
                                        _mmDetailPlaning1.dpLastShotQty = @class.List_ViewMoldData[i].v_arLastShotQty != null ? int.Parse(@class.List_ViewMoldData[i].v_arLastShotQty) : 0;
                                        _mmDetailPlaning1.dpRequestNo = @class.List_ViewMoldData[i].v_RequestNo;
                                        _mmDetailPlaning1.dpRemark = @class.List_ViewMoldData[i].v_Remark;
                                        _mmDetailPlaning1.dpPlan_DM_Month = s_date;
                                        _MOLD._mmDetailPlaning.Update(_mmDetailPlaning1);
                                        _MOLD.SaveChanges();
                                    }

                                }
                            }
                            //mmDetailPlaning _mmDetailPlaning


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


                }
                else //new
                {
                    using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
                    {
                        try
                        {
                            mmRunDocument _mmRunDocument = new mmRunDocument()
                            {
                                rmRunNo = runNo,// list_mmRunDocument.rmRunNo + 1,
                                rmPlant = s_plant,
                                rmYear = a_year,
                                rmGroup = "MP",
                                rmIssueBy = s_issue,
                                rmUpdateBy = s_issue,
                            };
                            _MOLD.mmRunDocument.Add(_mmRunDocument);
                            _MOLD.SaveChanges();

                            mmMastPlaning _mmMastPlaning = new mmMastPlaning()
                            {
                                mpDocumentNo = rundoc,
                                mpMonth = s_date,
                                mpPlant = s_plant,
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

                            //_MOLD.mmMastPlaning.Update(_mmMastPlaning);
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
                }
            }
            else
            {
                config = "N";
                msg = "No data!!!! Please search Data";
            }


            //return RedirectToAction("Index", "Home");
            return Json(new
            {
                c1 = config,
                c2 = msg,
                c3 = @class,
                c4 = rundoc,
                c5 = s_Status

            });
            //return View("Index", @class);

        }

        public IActionResult Logout()
        {
            this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData.Clear();
            //Session.Abandon();
            //this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //TempData.Remove("MyData"); // Remove Particular TempData i.e. userid.
            //TempData.Clear();
            return RedirectToAction("Login", "Home");

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
