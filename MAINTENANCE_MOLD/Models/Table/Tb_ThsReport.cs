using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MAINTENANCE_MOLD.Models.Table
{
    public class Tb_ThsReport
    {
        [Table("Login")]
        public class ViewLogin
        {
            [Key]
            public string UserId { get; set; }
            public string Password { get; set; }
            public string Program { get; set; }
            public string Empcode { get; set; }
            public string Permission { get; set; }
        }

        [Table("mmMaPlanActual")]
        public class mmMaPlanActual
        {
            [Key]
            public string paMonth { get; set; }
            public string paPlant { get; set; }
            public string paLine { get; set; }
            public string paMoldNo { get; set; }
            public string paMoldName { get; set; }
            public string paModel { get; set; }
            public string paMold_Control { get; set; }
            public string paIcsNoInj { get; set; }
            public string paMA_Plan_Date { get; set; }
            public double? paMA_Plan_Qty { get; set; }
            public string paMA_Actual_Date { get; set; }
            public double? paMA_Actual_Qty { get; set; }
            public string paMA_Actual_By { get; set; }
            public double? paMA_Std_Shot { get; set; }
            public string paMA_Status { get; set; }
            public string paMA_Remark { get; set; }
            public string paIssueBy { get; set; }
            public string paUpdateBy { get; set; }
        }

        public class Error
        {
            public string validation { get; set; }
        }
    }
}
