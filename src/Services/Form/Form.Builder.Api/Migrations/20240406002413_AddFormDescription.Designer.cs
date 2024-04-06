﻿// <auto-generated />
using System;
using Form.Builder.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Form.Builder.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240406002413_AddFormDescription")]
    partial class AddFormDescription
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Form.Builder.Api.Entities.Form", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("Form.Builder.Api.Entities.FormField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FormId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("FormFields");
                });

            modelBuilder.Entity("Form.Builder.Api.Entities.FormFieldValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Guid>("FieldId")
                        .HasColumnType("uuid");

                    b.Property<int>("FormSubmissionId")
                        .HasColumnType("integer");

                    b.Property<string>("StringValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.HasIndex("FormSubmissionId");

                    b.ToTable("FormFieldValues");
                });

            modelBuilder.Entity("Form.Builder.Api.Models.FormSubmission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FormId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("FormSubmissions");
                });

            modelBuilder.Entity("Form.Builder.Api.Entities.FormField", b =>
                {
                    b.HasOne("Form.Builder.Api.Entities.Form", "Form")
                        .WithMany("Fields")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Form.Builder.Api.Entities.FormFieldDetails", "Details", b1 =>
                        {
                            b1.Property<Guid>("FormFieldId")
                                .HasColumnType("uuid");

                            b1.HasKey("FormFieldId");

                            b1.ToTable("FormFields");

                            b1.ToJson("Details");

                            b1.WithOwner()
                                .HasForeignKey("FormFieldId");

                            b1.OwnsOne("Form.Builder.Api.Entities.NumberInputDetails", "NumberInput", b2 =>
                                {
                                    b2.Property<Guid>("FormFieldDetailsFormFieldId")
                                        .HasColumnType("uuid");

                                    b2.Property<int?>("Max")
                                        .HasColumnType("integer");

                                    b2.Property<int?>("Min")
                                        .HasColumnType("integer");

                                    b2.HasKey("FormFieldDetailsFormFieldId");

                                    b2.ToTable("FormFields");

                                    b2.WithOwner()
                                        .HasForeignKey("FormFieldDetailsFormFieldId");
                                });

                            b1.OwnsOne("Form.Builder.Api.Entities.TextInputDetails", "TextInput", b2 =>
                                {
                                    b2.Property<Guid>("FormFieldDetailsFormFieldId")
                                        .HasColumnType("uuid");

                                    b2.Property<int?>("MaxLength")
                                        .HasColumnType("integer");

                                    b2.Property<int?>("MinLength")
                                        .HasColumnType("integer");

                                    b2.HasKey("FormFieldDetailsFormFieldId");

                                    b2.ToTable("FormFields");

                                    b2.WithOwner()
                                        .HasForeignKey("FormFieldDetailsFormFieldId");
                                });

                            b1.Navigation("NumberInput");

                            b1.Navigation("TextInput");
                        });

                    b.Navigation("Details")
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("Form.Builder.Api.Entities.FormFieldValue", b =>
                {
                    b.HasOne("Form.Builder.Api.Entities.FormField", "Field")
                        .WithMany()
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Form.Builder.Api.Models.FormSubmission", "FormSubmission")
                        .WithMany("Values")
                        .HasForeignKey("FormSubmissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Field");

                    b.Navigation("FormSubmission");
                });

            modelBuilder.Entity("Form.Builder.Api.Models.FormSubmission", b =>
                {
                    b.HasOne("Form.Builder.Api.Entities.Form", "Form")
                        .WithMany()
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Form");
                });

            modelBuilder.Entity("Form.Builder.Api.Entities.Form", b =>
                {
                    b.Navigation("Fields");
                });

            modelBuilder.Entity("Form.Builder.Api.Models.FormSubmission", b =>
                {
                    b.Navigation("Values");
                });
#pragma warning restore 612, 618
        }
    }
}
