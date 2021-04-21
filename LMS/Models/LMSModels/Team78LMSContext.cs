using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team78LMSContext : DbContext
    {
        public Team78LMSContext()
        {
        }

        public Team78LMSContext(DbContextOptions<Team78LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Assignment> Assignment { get; set; }
        public virtual DbSet<AssignmentCategory> AssignmentCategory { get; set; }
        public virtual DbSet<Class> Class { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Enrolled> Enrolled { get; set; }
        public virtual DbSet<Professor> Professor { get; set; }
        public virtual DbSet<Semester> Semester { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Submission> Submission { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=atr.eng.utah.edu;User Id=u1097340;Password=Senior2021;Database=Team78LMS");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.AssnId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.AssnCategoryId, e.Name })
                    .HasName("AssnCategoryID")
                    .IsUnique();

                entity.Property(e => e.AssnId).HasColumnName("AssnID");

                entity.Property(e => e.AssnCategoryId).HasColumnName("AssnCategoryID");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.AssnCategory)
                    .WithMany(p => p.Assignment)
                    .HasForeignKey(d => d.AssnCategoryId)
                    .HasConstraintName("Assignment_ibfk_1");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.AssnCategoryId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.ClassId, e.Name })
                    .HasName("ClassID")
                    .IsUnique();

                entity.Property(e => e.AssnCategoryId).HasColumnName("AssnCategoryID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategory)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategory_ibfk_1");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.CourseId)
                    .HasName("CourseID");

                entity.HasIndex(e => new { e.SemesterId, e.CourseId })
                    .HasName("SemesterID")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Professor)
                    .IsRequired()
                    .HasColumnType("char(8)");

                entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

                entity.Property(e => e.Start).HasColumnType("time");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Class_ibfk_2");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Class)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Class_ibfk_1");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => new { e.DeptAbbr, e.Number })
                    .HasName("DeptAbbr")
                    .IsUnique();

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.DeptAbbr)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DeptAbbrNavigation)
                    .WithMany(p => p.Course)
                    .HasForeignKey(d => d.DeptAbbr)
                    .HasConstraintName("Course_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Abbr)
                    .HasName("PRIMARY");

                entity.Property(e => e.Abbr).HasColumnType("varchar(4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.SId, e.ClassId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.Property(e => e.SId)
                    .HasColumnName("sID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_2");

                entity.HasOne(d => d.S)
                    .WithMany(p => p.Enrolled)
                    .HasForeignKey(d => d.SId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Enrolled_ibfk_1");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.DeptAbbr)
                    .HasName("DeptAbbr");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.DeptAbbr)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.HasOne(d => d.DeptAbbrNavigation)
                    .WithMany(p => p.Professor)
                    .HasForeignKey(d => d.DeptAbbr)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professor_ibfk_1");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.Season })
                    .HasName("Year")
                    .IsUnique();

                entity.Property(e => e.SemesterId).HasColumnName("SemesterID");

                entity.Property(e => e.Season)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.SId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Major)
                    .HasName("Major");

                entity.Property(e => e.SId)
                    .HasColumnName("sID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FName)
                    .IsRequired()
                    .HasColumnName("fName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LName)
                    .IsRequired()
                    .HasColumnName("lName")
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Major)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.MajorNavigation)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.Major)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Student_ibfk_1");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.SId, e.AssnId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AssnId)
                    .HasName("AssnID");

                entity.Property(e => e.SId)
                    .HasColumnName("sID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.AssnId).HasColumnName("AssnID");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Assn)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.AssnId)
                    .HasConstraintName("Submission_ibfk_2");

                entity.HasOne(d => d.S)
                    .WithMany(p => p.Submission)
                    .HasForeignKey(d => d.SId)
                    .HasConstraintName("Submission_ibfk_1");
            });
        }
    }
}
