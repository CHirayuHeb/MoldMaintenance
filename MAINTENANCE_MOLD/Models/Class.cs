using MAINTENANCE_MOLD.Models.Table;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MAINTENANCE_MOLD.Models.Table.Tb_Mold;
using static MAINTENANCE_MOLD.Models.Table.Tb_Search;
using static MAINTENANCE_MOLD.Models.Table.Tb_ThsReport;

namespace MAINTENANCE_MOLD.Models
{
    public class Class
    {
        public MoldLogin _ViewLogin { get; set; }
        public Error _Error { get; set; }
        public ViewSearch _ViewSearch { get; set; }
        public ViewSearchPage _SearchPage { get; set; }
        public mmMaActual_Risk _ViewmmMaActual_Risk { get; set; }
        public View_mmMaActual_Risk View_mmMaActual_Risk { get; set;}
        public ViewMoldData ViewMoldData { get; set; }
        public ViewRequest ViewRequest { get; set; }
        public List<ViewRequest> List_ViewRequest { get; set; }
        public List<ViewMoldData> List_ViewMoldData { get; set; }
        public mmHistoryApproved mmHistoryApproved { get; set; }
        public Viewemail _Viewemail { get; set; }
        public ViewCreateMold _ViewCreateMold { get; set; }
        

    }
}
