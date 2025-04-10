using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MAINTENANCE_MOLD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QA_APPROVAL_REQUEST.Models.DBConn;
using static MAINTENANCE_MOLD.Models.Table.Tb_Mold;
using static MAINTENANCE_MOLD.Models.Table.Tb_Search;


namespace MAINTENANCE_MOLD.Controllers
{
    public class SearchController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public HomeController _HomeController;

        public SearchController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT, HomeController HomeController)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
            _HomeController = HomeController;
        }

        [Authorize("Checked")]
        //[HttpPost]
        public IActionResult SearchPage(Class @class)
        {
            List<mmMastFlowApprove> _ListmmMastFlowApprove = new List<mmMastFlowApprove>();
            _ListmmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "01").OrderBy(x => x.mfStep).ToList();


            //TempData["empcode"] = TempData["empcode"];
            SelectList s_docno = new SelectList(_MOLD.mmMastPlaning.Select(x => x.mpDocumentNo).Distinct());
            SelectList s_status = new SelectList(_ListmmMastFlowApprove.Select(x => x.mfSubject).Distinct());
            ViewBag.s_docno = s_docno;
            ViewBag.s_status = s_status;
            //ViewBag.v_count = ;
            return View();
        }


        [HttpPost]
        public IActionResult SearchData(Class @class)
        {
            List<mmMastFlowApprove> _ListmmMastFlowApprove = new List<mmMastFlowApprove>();
            _ListmmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "01").OrderBy(x => x.mfStep).ToList();
            SelectList s_status = new SelectList(_ListmmMastFlowApprove.Select(x => x.mfSubject).Distinct());

            var v_emp = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            //TempData["empcode"] = TempData["empcode"];
            SelectList s_docno = new SelectList(_MOLD.mmMastPlaning.Select(x => x.mpDocumentNo).Distinct());
            //SelectList s_status = new SelectList(_MOLD.mmMastFlowApprove.Select(x => x.mfSubject).Distinct());
           // SelectList s_status = new SelectList(_MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "01").OrderByDescending(x => x.mfStep).Select(x => x.mfSubject).Distinct());

            List<mmMastPlaning> _mmMastPlaning = new List<mmMastPlaning>();
            _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo.Contains("MP")).OrderByDescending(w => w.mpDocumentNo).ToList();
            if (@class._SearchPage.s_plant != null) { _mmMastPlaning = _mmMastPlaning.Where(w => w.mpPlant == @class._SearchPage.s_plant).ToList(); }

            //  var query2 = _mmMastPlaning.Where(x => x.mpMonth >= "Argentina" && x.mpMonth <= "Jamaica");
            if (@class._SearchPage.s_MonthFrom != null) { _mmMastPlaning = _mmMastPlaning.Where(w => DateTime.Parse(w.mpMonth) >= DateTime.Parse(@class._SearchPage.s_MonthFrom)).ToList(); }
            if (@class._SearchPage.s_MonthTo != null) { _mmMastPlaning = _mmMastPlaning.Where(w => DateTime.Parse(w.mpMonth) <= DateTime.Parse(@class._SearchPage.s_MonthTo)).ToList(); }

            if (@class._SearchPage.s_Docno != null) { _mmMastPlaning = _mmMastPlaning.Where(w => w.mpDocumentNo.Contains(@class._SearchPage.s_Docno)).ToList(); }
            //if (@class._SearchPage.s_Docno != null) { _mmMastPlaning = _mmMastPlaning.Where(w => w.mpDocumentNo == @class._SearchPage.s_Docno).ToList(); }
            if (@class._SearchPage.s_Status != null) { _mmMastPlaning = _mmMastPlaning.Where(w => w.mpStatus == @class._SearchPage.s_Status).ToList(); }



            var _mmHistoryApproved = from r in _MOLD.mmHistoryApproved
                                     group r by r.htDocumentNo into g
                                     select g.OrderByDescending(x => x.htStep).First();
            var mmHistoryApproved1 = _mmHistoryApproved.GroupBy(r => r.htDocumentNo).Select(g => g.OrderByDescending(x => x.htStep).First()).ToList();


            var join_mmMastPlaning = (from mm in _mmMastPlaning
                                      join mt in mmHistoryApproved1
                                      on mm.mpDocumentNo equals mt.htDocumentNo
                                      into d2
                                      from f in d2.DefaultIfEmpty()
                                      select new ViewmmMastPlaning
                                      {
                                          mpDocumentNo = mm.mpDocumentNo,
                                          mpMonth = mm.mpMonth,
                                          mpPlant = mm.mpPlant,
                                          mpFlow = mm.mpFlow,
                                          mpStep = mm.mpStep,
                                          mpStatus = mm.mpStatus,
                                          mpdateSent = f == null ? "-" : f.htDate + " " + f.htTime,
                                          mpEmpcode_Issue = mm.mpEmpcode_Issue,
                                          mpName_Issue = mm.mpName_Issue,
                                          mpName_Approve = mm.mpName_Approve != "" ? mm.mpName_Approve : "-"
                                      }
                                      ).ToList();

            //_mmMastPlaning = _MOLD.mmMastPlaning.OrderByDescending(w => w.mpDocumentNo).ToList();
            join_mmMastPlaning = join_mmMastPlaning.OrderByDescending(x => x.mpDocumentNo).ToList();
            ViewBag.s_docno = s_docno;
            ViewBag.s_status = s_status;
            ViewBag.SearchData = join_mmMastPlaning;
            ViewBag.v_count = "Total " + join_mmMastPlaning.Count() + " Row";
            return View("SearchPage", @class);

        }


        public ActionResult MoreData(string docno)
        {
            var v_empcode = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            Class @classs = new Class();
            var t_docno = docno;
            mmMastPlaning mastPlaning = new mmMastPlaning();
            mmMastPlaning _mmMastPlaning = _MOLD.mmMastPlaning.FirstOrDefault(s => s.mpDocumentNo == t_docno);
            @classs._ViewSearch = new ViewSearch();
            @classs._ViewSearch.v_Docno = _mmMastPlaning.mpDocumentNo.ToString();
            @classs._ViewSearch.v_plant = _mmMastPlaning.mpPlant.ToString();
            @classs._ViewSearch.v_Date = _mmMastPlaning.mpMonth.ToString();
            @classs._ViewSearch.V_Status = _mmMastPlaning.mpStatus.ToString();
            @classs._ViewSearch.v_empcode = v_empcode;
            @classs._ViewSearch.v_page = "More";

            //   _HomeController.ActionEvent("SEARCH", @classs);
            // return RedirectToAction("Index", "Home");
            // return RedirectToAction("ActionEvent", "Home", new { submitButton = "SEARCH", @class = @classs._ViewSearch });

            // return RedirectToAction("ActionEvent", "Home", new { submitButton = "SEARCH", @class = @classs });
            string v_class_ViewSearch = JsonConvert.SerializeObject(@classs._ViewSearch);
            return RedirectToAction("ActionEvent", "Home", new { submitButton = "More", @class = @classs, v_class = v_class_ViewSearch });
        }

        [HttpPost]
        public JsonResult Delete(string docno, int stepno)//only status create
        {
            string vpath = "SearchPage";

            int rmrun = int.Parse(docno.Substring(8, 3));
            var rmplant = docno.Substring(2, 4);
            var rmyear = docno.Substring(6, 2);


            using (var dbContextTransaction = _MOLD.Database.BeginTransaction())
            {
                try
                {

                    string str1 = "Delete from mmRunDocument where rmRunNo =" + rmrun + "  and rmPlant ='" + rmplant + "' and rmYear = '" + rmyear + "'";
                    _Excute excute1 = new _Excute();
                    int exe1 = excute1.DeleteData(str1, "MOLD", "thsdb");

                    string str2 = "Delete from mmMastPlaning where mpDocumentNo  ='" + docno + "'";
                    _Excute excute2 = new _Excute();
                    int exe2 = excute2.DeleteData(str2, "MOLD", "thsdb");




                    // var _mmMastPlaning = _MOLD.mmMastPlaning.Find(docno);
                    //// var _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo == docno);
                    // _MOLD.mmMastPlaning.RemoveRange(_mmMastPlaning);
                    // _MOLD.SaveChanges();

                    // var _mmRunDocument1= _MOLD.mmRunDocument.Find( rmrun, rmplant,rmyear);

                    // var _mmRunDocument = _MOLD.mmRunDocument.Where(x => x.rmRunNo == rmrun && x.rmPlant == rmplant && x.rmYear == rmyear);
                    // _MOLD.mmRunDocument.RemoveRange(_mmRunDocument);
                    // _MOLD.SaveChanges();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return Json("error");

                }
            }

            return Json(new { res = "success", rfsh = "refresh", path = vpath });
        }

    }
}