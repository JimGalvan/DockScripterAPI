﻿// <auto-generated />
using System;
using DockScripter.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DockScripter.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241110034301_update_script_entity")]
    partial class update_script_entity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("DockScripter.Domain.Entities.EnvironmentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContainerId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("EnvironmentName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdatedDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EnvironmentEntities");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.ExecutionResultEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("ErrorOutput")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExecutedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdatedDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Output")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScriptId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ScriptId");

                    b.ToTable("ExecutionResultEntities");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.ScriptEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Language")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastExecutedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdatedDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ScriptEntities");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdatedDateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("UserEntities");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.EnvironmentEntity", b =>
                {
                    b.HasOne("DockScripter.Domain.Entities.UserEntity", "User")
                        .WithMany("Environments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.ExecutionResultEntity", b =>
                {
                    b.HasOne("DockScripter.Domain.Entities.ScriptEntity", "Script")
                        .WithMany()
                        .HasForeignKey("ScriptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Script");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.ScriptEntity", b =>
                {
                    b.HasOne("DockScripter.Domain.Entities.UserEntity", "User")
                        .WithMany("Scripts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DockScripter.Domain.Entities.UserEntity", b =>
                {
                    b.Navigation("Environments");

                    b.Navigation("Scripts");
                });
#pragma warning restore 612, 618
        }
    }
}