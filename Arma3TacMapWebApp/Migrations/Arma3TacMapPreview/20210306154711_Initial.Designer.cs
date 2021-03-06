﻿// <auto-generated />
using System;
using Arma3TacMapWebApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Arma3TacMapWebApp.Migrations.Arma3TacMapPreview
{
    [DbContext(typeof(Arma3TacMapPreviewContext))]
    [Migration("20210306154711_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMapPreview", b =>
                {
                    b.Property<int>("TacMapID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Data")
                        .HasColumnType("BLOB");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.HasKey("TacMapID", "Size");

                    b.ToTable("TacMapPreview");
                });
#pragma warning restore 612, 618
        }
    }
}
