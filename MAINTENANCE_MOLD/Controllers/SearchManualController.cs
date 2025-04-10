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
    public class SearchManualController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public HomeController _HomeController;

        public SearchManualController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT, HomeController HomeController)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
            _HomeController = HomeController;
        }

        [Authorize("Checked")]
        public IActionResult Index()
        {
            List<mmMastFlowApprove> _ListmmMastFlowApprove = new List<mmMastFlowApprove>();
            _ListmmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02").OrderBy(x => x.mfStep).ToList();
            SelectList s_status = new SelectList(_ListmmMastFlowApprove.Select(x => x.mfSubject).Distinct());
            //TempData["empcode"] = TempData["empcode"];
            SelectList s_docno = new SelectList(_MOLD.mmMastPlaning.Select(x => x.mpDocumentNo).Distinct());
            //SelectList s_status = new SelectList(_MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02").OrderBy(x => x.mfStep).Select(x => x.mfSubject).Distinct());

            ViewBag.s_docno = s_docno;
            ViewBag.s_status = s_status;
            //ViewBag.v_count = ;
            return View();

        }


        [HttpPost]
        public IActionResult SearchData(Class @class)
        {
            var v_emp = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            //TempData["empcode"] = TempData["empcode"];
            SelectList s_docno = new SelectList(_MOLD.mmMastPlaning.Select(x => x.mpDocumentNo).Distinct());
            List<mmMastFlowApprove> _ListmmMastFlowApprove = new List<mmMastFlowApprove>();
            _ListmmMastFlowApprove = _MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02").OrderBy(x => x.mfStep).ToList();
            SelectList s_status = new SelectList(_ListmmMastFlowApprove.Select(x => x.mfSubject).Distinct());

           // SelectList s_status = new SelectList(_MOLD.mmMastFlowApprove.Where(x => x.mfFlowNo == "02").OrderBy(x => x.mfStep).Select(x => x.mfSubject).Distinct());

           // SelectList s_status = new SelectList(_MOLD.mmMastFlowApprove.Select(x => x.mfSubject).Distinct());
            List<mmMastPlaning> _mmMastPlaning = new List<mmMastPlaning>();
            List<mmDetailPlaning> _mmDetailPlaning = new List<mmDetailPlaning>();
            // _mmMastPlaning = _MOLD.mmMastPlaning.Where(x => x.mpDocumentNo.Contains("RE")).OrderByDescending(w => w.mpDocumentNo).ToList();

            _mmDetailPlaning = _MOLD._mmDetailPlaning.Where(x => x.dpDocumentNo.Contains("RE")).OrderByDescending(w => w.dpDocumentNo).ToList();

            if (@class._SearchPage.s_plant != null)
            {
                //_mmMastPlaning = _mmMastPlaning.Where(w => w.mpPlant == @class._SearchPage.s_plant).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => w.dpDocumentNo.Substring(2, 4) == @class._SearchPage.s_plant).ToList();
            }
            if (@class._SearchPage.s_Moldno != null)
            {
                //_mmMastPlaning = _mmMastPlaning.Where(w => w.mpPlant == @class._SearchPage.s_plant).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => w.dpMoldNo.Contains(@class._SearchPage.s_Moldno)).ToList();
            }
            //  var query2 = _mmMastPlaning.Where(x => x.mpMonth >= "Argentina" && x.mpMonth <= "Jamaica");
            if (@class._SearchPage.s_MonthFrom != null)
            {
                //_mmMastPlaning = _mmMastPlaning.Where(w => DateTime.Parse(w.mpMonth) >= DateTime.Parse(@class._SearchPage.s_MonthFrom)).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => DateTime.Parse(w.dpPlan_DM_Month) >= DateTime.Parse(@class._SearchPage.s_MonthFrom)).ToList();
            }
            if (@class._SearchPage.s_MonthTo != null)
            {
                // _mmMastPlaning = _mmMastPlaning.Where(w => DateTime.Parse(w.mpMonth) <= DateTime.Parse(@class._SearchPage.s_MonthTo)).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => DateTime.Parse(w.dpPlan_DM_Month) <= DateTime.Parse(@class._SearchPage.s_MonthTo)).ToList();
            }


            if (@class._SearchPage.s_Docno != null)
            {
                //_mmMastPlaning = _mmMastPlaning.Where(w => w.mpDocumentNo.Contains(@class._SearchPage.s_Docno)).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => w.dpDocumentNo.Contains(@class._SearchPage.s_Docno)).ToList();
            }
            //if (@class._SearchPage.s_Docno != null) { _mmMastPlaning = _mmMastPlaning.Where(w => w.mpDocumentNo == @class._SearchPage.s_Docno).ToList(); }
            if (@class._SearchPage.s_Status != null)
            {
                // _mmMastPlaning = _mmMastPlaning.Where(w => w.mpStatus == @class._SearchPage.s_Status).ToList();
                _mmDetailPlaning = _mmDetailPlaning.Where(w => w.dpStatus == @class._SearchPage.s_Status).ToList();
            }



            var _mmHistoryApproved = from r in _MOLD.mmHistoryApproved
                                     group r by r.htDocumentNo into g
                                     select g.OrderByDescending(x => x.htStep).First();
            var mmHistoryApproved1 = _mmHistoryApproved.GroupBy(r => r.htDocumentNo).Select(g => g.OrderByDescending(x => x.htStep).First()).ToList();


            //var join_mmMastPlaning1 = (from mm in _mmMastPlaning
            //                          join mt in mmHistoryApproved1
            //                          on mm.mpDocumentNo equals mt.htDocumentNo
            //                          into d2
            //                          from f in d2.DefaultIfEmpty()
            //                          select new ViewmmMastPlaning
            //                          {
            //                              mpDocumentNo = mm.mpDocumentNo,
            //                              mpMonth = mm.mpMonth,
            //                              mpPlant = mm.mpPlant,
            //                              mpFlow = mm.mpFlow,
            //                              mpStep = mm.mpStep,
            //                              mpStatus = mm.mpStatus,
            //                              mpdateSent = f == null ? "-" : f.htDate + " " + f.htTime,
            //                              mpEmpcode_Issue = mm.mpEmpcode_Issue,
            //                              mpName_Issue = mm.mpName_Issue,
            //                              mpName_Approve = mm.mpName_Approve != "" ? mm.mpName_Approve : "-"
            //                          }
            //                          ).ToList();

            var join_mmMastPlaning = (from mm in _mmDetailPlaning
                                      join mt in mmHistoryApproved1
                                      on mm.dpDocumentNo equals mt.htDocumentNo
                                      into d2
                                      from f in d2.DefaultIfEmpty()
                                      select new ViewSearchManual
                                      {
                                          mpDocumentNo = mm.dpDocumentNo,
                                          mpMoldNo = mm.dpMoldNo,
                                          mpMonth = mm.dpPlan_DM_Month,
                                          mpPlant = mm.dpDocumentNo.Substring(2, 4),
                                          mpFlow = "02",
                                          mpStep = int.Parse(mm.dpStep),
                                          mpStatus = mm.dpStatus,
                                          mpdateSent = f == null ? "-" : f.htDate + " " + f.htTime,
                                          mpEmpcode_Issue = mm.dpEmpcode_Issue,
                                          mpName_Issue = mm.dpName_Issue,
                                          mpName_Approve = mm.dpName_Approve != "" ? mm.dpName_Approve : "-"
                                      }
                                    ).ToList();

            //_mmMastPlaning = _MOLD.mmMastPlaning.OrderByDescending(w => w.mpDocumentNo).ToList();
            join_mmMastPlaning = join_mmMastPlaning.OrderByDescending(x => x.mpDocumentNo).ToList();
            ViewBag.s_docno = s_docno;
            ViewBag.s_status = s_status;
            ViewBag.SearchData = join_mmMastPlaning;
            ViewBag.v_count = "Total " + join_mmMastPlaning.Count() + " Row";
            return View("Index", @class);

        }
    }
}