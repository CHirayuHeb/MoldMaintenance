using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MAINTENANCE_MOLD.Models.Table.Tb_Hrms;
using static MAINTENANCE_MOLD.Models.Table.Tb_IT;
using static MAINTENANCE_MOLD.Models.Table.Tb_Mold;
using static MAINTENANCE_MOLD.Models.Table.Tb_ThsReport;

namespace MAINTENANCE_MOLD.Models
{
    public class HRMS : DbContext
    {
        public HRMS(DbContextOptions<HRMS> options) : base(options) { }
        public DbSet<ViewAccEMPLOYEE> AccEMPLOYEE { get; set; }


        //public DbSet<AccEMPLOYEE_DEP> AccEMPLOYEE_DEP { get; set; }
    }
    public class IT : DbContext
    {
        public IT(DbContextOptions<IT> options) : base(options) { }
        public DbSet<Email> Email { get; set; }

    }
    public class ThsReport : DbContext
    {
        public ThsReport(DbContextOptions<ThsReport> options) : base(options) { }
        public DbSet<ViewLogin> Login { get; set; }
        public DbSet<mmMaPlanActual> mmMaPlanActual { get; set; }
        //public DbSet<Email> Email { get; set; }
    }
    public class MOLD : DbContext
    {
        public MOLD(DbContextOptions<MOLD> options) : base(options) { }
        public DbSet<mtMaster_Mold_Control> mtMaster_Mold_Control { get; set; }
        public DbSet<mmMaActual_Risk> mmMaActual_Risk { get; set; }
        public DbSet<mmMast_SizeCleaning> mmMast_SizeCleaning { get; set; }
        public DbSet<mmRunDocument> mmRunDocument { get; set; }
        public DbSet<mmMastFlowApprove> mmMastFlowApprove { get; set; }
        public DbSet<mmHistoryApproved> mmHistoryApproved { get; set; }
        public DbSet<mmMastPlaning> mmMastPlaning { get; set; }
        public List<mmMastPlaning> _list_mmMastPlaning { get; set; }
        public mmMastPlaning _mmMastPlaning { get; set; }
        public DbSet<mmDetailPlaning> _mmDetailPlaning { get; set; }
        public mmDetailPlaning mmDetailPlaning { get; set; }
        public DbSet<MoldLogin> Login { get; set; }
        public DbSet<mmMastUserApprove> _mmMastUserApprove { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<mmDetailPlaning>(entity =>
            {
                entity.HasKey(k => new { k.dpDocumentNo, k.dpMoldNo });
            });
            modelBuilder.Entity<mmRunDocument>(entity =>
            {
                entity.HasKey(k => new { k.rmRunNo, k.rmPlant,k.rmYear,k.rmGroup });
            });
            modelBuilder.Entity<mmMastFlowApprove>(entity =>
            {
                entity.HasKey(k => new { k.mfFlowNo, k.mfStep, k.mfDept });
            });

        }

    }
}
