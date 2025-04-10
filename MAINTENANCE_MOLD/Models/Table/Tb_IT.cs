using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MAINTENANCE_MOLD.Models.Table
{
    public class Tb_IT
    {
        [Table("rpemail", Schema = "dbo")] //IT
        public class Email //qs9000
        {
            [Key]
            public string emEmail { get; set; }
            public string emDeptCode { get; set; }
            public string emEmpcode { get; set; }
            public string emEmail_M365 { get; set; }
            public string emDept_M365 { get; set; }
            public string emName_M365 { get; set; }
           

        }

    }
}
