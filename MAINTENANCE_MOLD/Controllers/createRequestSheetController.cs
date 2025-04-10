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
    public class createRequestSheetController : Controller
    {
        public HRMS _HRMS; //thsdb
        public ThsReport _ThsReport; //thsdbdb
        public MOLD _MOLD; //thsdbdb
        public IT _IT; //thsdb
        public string _Location = @"\\thsweb\\MAINTENANCE_MOLD\\";
        public createRequestSheetController(ThsReport ThsReport, HRMS HRMS, MOLD MOLD, IT IT)
        {
            _ThsReport = ThsReport;
            _HRMS = HRMS;
            _MOLD = MOLD;
            _IT = IT;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}