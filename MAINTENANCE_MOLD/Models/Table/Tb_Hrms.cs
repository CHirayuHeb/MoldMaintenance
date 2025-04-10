using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MAINTENANCE_MOLD.Models.Table
{
    public class Tb_Hrms
    {
        [Table("AccEMPLOYEE")]
        public class ViewAccEMPLOYEE
        {
            [Key]
            public string EMP_CODE { get; set; }
            public string NICKNAME { get; set; }
            public string EMP_TNAME { get; set; }
            public string LAST_TNAME { get; set; }
            public string DEPT_CODE { get; set; }
            public string SEC_CODE { get; set; }

        }
    }
}
