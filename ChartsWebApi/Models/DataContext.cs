using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChartsWebApi.models
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Build> Build { get; set; }
        public virtual DbSet<BuildItem> BuildItem { get; set; }
        public virtual DbSet<BuildSurvey> BuildSurvey { get; set; }
        public virtual DbSet<BuildSurveyGroup> BuildSurveyGroup { get; set; }
        public virtual DbSet<BuildSurveyGroupTool> BuildSurveyGroupTool { get; set; }
        public virtual DbSet<BuildTool> BuildTool { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<Exam01Bill> Exam01Bill { get; set; }
        public virtual DbSet<Exam01Bom> Exam01Bom { get; set; }
        public virtual DbSet<Exam01BomImg> Exam01BomImg { get; set; }
        public virtual DbSet<Exam01Img> Exam01Img { get; set; }
        public virtual DbSet<Exam02Bill> Exam02Bill { get; set; }
        public virtual DbSet<Exam02Tool> Exam02Tool { get; set; }
        public virtual DbSet<ExamGroup> ExamGroup { get; set; }
        public virtual DbSet<Remark> Remark { get; set; }
        public virtual DbSet<OrgUser> OrgUser { get; set; }
        public virtual DbSet<Dict> Dict { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Dict>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("TA_DIC_VALUE");
                entity.Property(e => e.Id).HasColumnName("CN_ID");
                entity.Property(e => e.Key).HasColumnName("CN_NAME");
                entity.Property(e => e.Value).HasColumnName("CN_VALUE");
            });

            modelBuilder.Entity<OrgUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("TN_ORG_USER");
                entity.Property(e => e.Id).HasColumnName("CN_ID");
                entity.Property(e => e.Guid).HasColumnName("CN_GUID");
                entity.Property(e => e.Login).HasColumnName("CN_LOGIN");
                entity.Property(e => e.Status).HasColumnName("CN_STATUS");
                entity.Property(e => e.Type).HasColumnName("CN_TYPE");
                entity.Property(e => e.Name).HasColumnName("CN_NAME");
                entity.Property(e => e.Username).HasColumnName("CN_USER_NAME");
                entity.Property(e => e.ParGuid).HasColumnName("CN_PAR_GUID");
            });

            modelBuilder.Entity<Build>(entity =>
            {
                entity.HasKey(e => e.CnId);

                entity.ToTable("TA_BULID");

                entity.Property(e => e.CnId).HasColumnName("CN_ID");

                entity.Property(e => e.Cn3dFileDir)
                    .IsRequired()
                    .HasColumnName("CN_3D_FILE_DIR")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Cn3dFileName)
                    .IsRequired()
                    .HasColumnName("CN_3D_FILE_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnCode)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnDesc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnDtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CnName)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnStatus)
                    .IsRequired()
                    .HasColumnName("CN_STATUS")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnSysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CnType)
                    .IsRequired()
                    .HasColumnName("CN_TYPE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<BuildItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_BULID_ITEM");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BuildId).HasColumnName("CN_BULID_ID");
                entity.Property(e => e.PartId).HasColumnName("CN_PART_ID");
                entity.Property(e => e.PartName).HasColumnName("CN_PAR_NAME");
                entity.Property(e => e.PartNameAlias1).HasColumnName("CN_PAR_NAME_Alias1");
                entity.Property(e => e.PartNameAlias2).HasColumnName("CN_PAR_NAME_Alias2");


                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Features)
                    .IsRequired()
                    .HasColumnName("CN_FEATURES")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("CN_FULL_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IsAssemble)
                    .IsRequired()
                    .HasColumnName("CN_ISASEMBLE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IsPart)
                    .IsRequired()
                    .HasColumnName("CN_ISPART")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NameAlias1)
                    .IsRequired()
                    .HasColumnName("CN_NAME_Alias1")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.NameAlias2)
                    .IsRequired()
                    .HasColumnName("CN_NAME_Alias2")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");


                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("CN_STATUS")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("CN_TYPE")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<BuildSurvey>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_BULID_SURVEY");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BuildId).HasColumnName("CN_BULID_ID");

                entity.Property(e => e.Code)
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_D_CREA")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Desc)
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasColumnName("CN_FULL_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasColumnName("CN_VALUE")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<BuildSurveyGroup>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_BULID_SURVEY_GROUP");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BuildId).HasColumnName("CN_BULID_ID");
                
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_D_CREA")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("CN_FULL_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SurveyId).HasColumnName("CN_SURVEY_ID");
            });

            modelBuilder.Entity<BuildSurveyGroupTool>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_BULID_SURVEY_GROUP_TOOL");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Count).HasColumnName("CN_COUNT");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_D_CREA")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GroupId).HasColumnName("CN_GROUP_ID");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasColumnName("CN_GROUP_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ToolId).HasColumnName("CN_TOOL_ID");

                entity.Property(e => e.ToolName)
                    .IsRequired()
                    .HasColumnName("CN_TOOL_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.HasOne(e => e.Group)
                      .WithMany(g => g.GroupTools)
                      .HasForeignKey(e => e.GroupId);
            });

            modelBuilder.Entity<BuildTool>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_BULID_TOOL");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("CN_FULL_NAME")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("CN_STATUS")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('有效')");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BuildCode)
                    .IsRequired()
                    .HasColumnName("CN_Build_Code")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.BuildName)
                    .IsRequired()
                    .HasColumnName("CN_Build_Name")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");


                entity.Property(e => e.BuildPartId)
                    .IsRequired()
                    .HasColumnName("CN_Build_Part_Id");

                entity.Property(e => e.BuildPartCode)
                    .IsRequired()
                    .HasColumnName("CN_Build_Part_Code")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.BuildPartName)
                    .IsRequired()
                    .HasColumnName("CN_Build_Part_Name")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtRemark)
                    .HasColumnName("CN_DT_Remark")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExaminerType)
                    .IsRequired()
                    .HasColumnName("CN_Examiner_type")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasColumnName("CN_GROUP_NAME")
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnName("CN_MODEL")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasColumnName("CN_Remark")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Score).HasColumnName("CN_Score");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("CN_STATUS")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.TimeEnd)
                    .HasColumnName("CN_Time_End")
                    .HasColumnType("datetime");

                entity.Property(e => e.TimeStart)
                    .IsRequired()
                    .HasColumnName("CN_Time_Start")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("CN_TYPE")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.UserId).HasColumnName("CN_USER_ID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("CN_USER_NAME")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam01Bill>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM01_BILL");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamId)
                    .IsRequired()
                    .HasColumnName("CN_Exam_ID")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasColumnName("CN_Remark")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Score).HasColumnName("CN_SCORE");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam01Bom>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM01_BOM");

                entity.Property(e => e.Id).HasColumnName("CN_ID");
                entity.Property(e => e.Score).HasColumnName("CN_SCORE");
                entity.Property(e => e.Order)
                      .IsRequired()
                      .HasColumnName("CN_ORDER");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamId)
                    .IsRequired()
                    .HasColumnName("CN_EXAM_ID")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc1)
                    .IsRequired()
                    .HasColumnName("CN_DESC1")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc2)
                    .IsRequired()
                    .HasColumnName("CN_DESC2")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Item)
                    .IsRequired()
                    .HasColumnName("CN_ITEM")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Part)
                    .IsRequired()
                    .HasColumnName("CN_PART")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam01BomImg>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM01_BOM_IMG");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BomId).HasColumnName("CN_BOM_ID");

                entity.Property(e => e.DCrea)
                    .HasColumnName("CN_D_CREA")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImgId).HasColumnName("CN_IMG_ID");
            });

            modelBuilder.Entity<Exam01Img>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM01_IMG");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamId).HasColumnName("CN_EXAM_ID");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("CN_FULL_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("CN_TYPE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam02Bill>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM02_BILL");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.SurveyId)
                      .IsRequired()
                      .HasColumnName("CN_SURVEY_ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamId)
                    .IsRequired()
                    .HasColumnName("CN_Exam_ID")
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasColumnName("CN_Remark")
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Score).HasColumnName("CN_SCORE");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("CN_VALUE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Exam02Tool>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM02_TOOLS");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.BillId).HasColumnName("CN_BILL_ID");

                entity.Property(e => e.Count).HasColumnName("CN_COUNT");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_D_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ExamId).HasColumnName("CN_Exam_ID");

                entity.Property(e => e.ToolId).HasColumnName("CN_TOOL_ID");
            });

            modelBuilder.Entity<ExamGroup>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_EXAM_GROUP");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExamId).HasColumnName("CN_EXAM_ID");

                entity.Property(e => e.UserId).HasColumnName("CN_USER_ID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("CN_USER_NAME")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Remark>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("TA_REMARK");

                entity.Property(e => e.Id).HasColumnName("CN_ID");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("CN_CODE")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Desc)
                    .IsRequired()
                    .HasColumnName("CN_DESC")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.DtCreate)
                    .HasColumnName("CN_DT_CREATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("CN_NAME")
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ScoreMan).HasColumnName("CN_Score_Man");

                entity.Property(e => e.ScoreMin).HasColumnName("CN_Score_Min");

                entity.Property(e => e.SysNote)
                    .IsRequired()
                    .HasColumnName("CN_SYS_NOTE")
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });
        }
    }
}
