using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Repository.Models
{
    public partial class ChartDbContext : DbContext
    {
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<FunctionRel> FunctionRels { get; set; }
        public virtual DbSet<FunctionValue> FunctionValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=openinfo.cn;Initial Catalog=OpenInfo.BI.ChartDb;User ID=sa;Password=Openinfo.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Function>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TN_BI_FUNCTION");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BusinessClassid).HasColumnName("CN_BUSINESS_CLASSID");

                entity.Property(e => e.ChartBlock).HasColumnName("CN_CHART_BLOCK");

                entity.Property(e => e.ChartLine).HasColumnName("CN_CHART_LINE");

                entity.Property(e => e.ChartLineBlock).HasColumnName("CN_CHART_LINE_BLOCK");

                entity.Property(e => e.ChartMap).HasColumnName("CN_CHART_MAP");

                entity.Property(e => e.ChartPie).HasColumnName("CN_CHART_PIE");

                entity.Property(e => e.Code).HasColumnName("CN_CODE");

                entity.Property(e => e.CreateBy).HasColumnName("CN_CREATE_BY");

                entity.Property(e => e.CreateLogin).HasColumnName("CN_CREATE_LOGIN");

                entity.Property(e => e.CreateName).HasColumnName("CN_CREATE_NAME");

                entity.Property(e => e.Desc).HasColumnName("CN_DESC");

                entity.Property(e => e.Dimension).HasColumnName("CN_DIMENSION");

                entity.Property(e => e.DomainClassid).HasColumnName("CN_DOMAIN_CLASSID");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IconBlob).HasColumnName("CN_ICON_BLOB");

                entity.Property(e => e.Level1Classid).HasColumnName("CN_LEVEL1_CLASSID");

                entity.Property(e => e.Level2Classid).HasColumnName("CN_LEVEL2_CLASSID");

                entity.Property(e => e.MoudelClassid).HasColumnName("CN_MOUDEL_CLASSID");

                entity.Property(e => e.Name).HasColumnName("CN_NAME");

                entity.Property(e => e.NameAbb).HasColumnName("CN_NAME_ABB");

                entity.Property(e => e.SystemClassid).HasColumnName("CN_SYSTEM_CLASSID");

                entity.Property(e => e.Title).HasColumnName("CN_TITLE");

                entity.Property(e => e.ValueClassid).HasColumnName("CN_VALUE_CLASSID");

                entity.Property(e => e.ValueDigits).HasColumnName("CN_VALUE_DIGITS");

                entity.Property(e => e.ValueType).HasColumnName("CN_VALUE_TYPE");
            });

            modelBuilder.Entity<FunctionRel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TN_BI_FUNCTION_REL");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.CreateBy).HasColumnName("CN_CREATE_BY");

                entity.Property(e => e.CreateLogin).HasColumnName("CN_CREATE_LOGIN");

                entity.Property(e => e.CreateName).HasColumnName("CN_CREATE_NAME");

                entity.Property(e => e.Desc).HasColumnName("CN_DESC");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FuncattrName).HasColumnName("CN_FUNCATTR_NAME");

                entity.Property(e => e.Funcid).HasColumnName("CN_FUNCID");

                entity.Property(e => e.ValueattrName).HasColumnName("CN_VALUEATTR_NAME");
            });

            modelBuilder.Entity<FunctionValue>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TN_BI_FUNCTION_VALUE01");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.CreateBy).HasColumnName("CN_CREATE_BY");

                entity.Property(e => e.CreateLogin).HasColumnName("CN_CREATE_LOGIN");

                entity.Property(e => e.CreateName).HasColumnName("CN_CREATE_NAME");

                entity.Property(e => e.Desc).HasColumnName("CN_DESC");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FuncId).HasColumnName("CN_FUNC_ID");

                entity.Property(e => e.Level1Name).HasColumnName("CN_LEVEL1_NAME");

                entity.Property(e => e.Level1Objectid).HasColumnName("CN_LEVEL1_OBJECTID");

                entity.Property(e => e.Level2Name).HasColumnName("CN_LEVEL2_NAME");

                entity.Property(e => e.Level2Objectid).HasColumnName("CN_LEVEL2_OBJECTID");

                entity.Property(e => e.V0000).HasColumnName("CN_V0000");

                entity.Property(e => e.V0001).HasColumnName("CN_V0001");

                entity.Property(e => e.V0002).HasColumnName("CN_V0002");

                entity.Property(e => e.V0003).HasColumnName("CN_V0003");

                entity.Property(e => e.V0004).HasColumnName("CN_V0004");

                entity.Property(e => e.V0005).HasColumnName("CN_V0005");

                entity.Property(e => e.V0006).HasColumnName("CN_V0006");

                entity.Property(e => e.V0007).HasColumnName("CN_V0007");

                entity.Property(e => e.V0008).HasColumnName("CN_V0008");

                entity.Property(e => e.V0009).HasColumnName("CN_V0009");

                entity.Property(e => e.V0010).HasColumnName("CN_V0010");

                entity.Property(e => e.V0011).HasColumnName("CN_V0011");

                entity.Property(e => e.V0012).HasColumnName("CN_V0012");

                entity.Property(e => e.V0013).HasColumnName("CN_V0013");

                entity.Property(e => e.V0014).HasColumnName("CN_V0014");

                entity.Property(e => e.V0015).HasColumnName("CN_V0015");

                entity.Property(e => e.V0016).HasColumnName("CN_V0016");

                entity.Property(e => e.V0017).HasColumnName("CN_V0017");

                entity.Property(e => e.V0018).HasColumnName("CN_V0018");

                entity.Property(e => e.V0019).HasColumnName("CN_V0019");

                entity.Property(e => e.V0020).HasColumnName("CN_V0020");
            });
        }
    }
}
