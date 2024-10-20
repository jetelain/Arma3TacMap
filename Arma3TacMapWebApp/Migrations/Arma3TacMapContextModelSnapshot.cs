﻿// <auto-generated />
using System;
using Arma3TacMapWebApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Arma3TacMapWebApp.Migrations
{
    [DbContext(typeof(Arma3TacMapContext))]
    partial class Arma3TacMapContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.Orbat", b =>
                {
                    b.Property<int>("OrbatID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerUserID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.Property<int>("Visibility")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrbatID");

                    b.HasIndex("OwnerUserID");

                    b.ToTable("Orbat", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.OrbatUnit", b =>
                {
                    b.Property<int>("OrbatUnitID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolFriendlyImageBase64")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolHQ")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolHostileAssumedImageBase64")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolHostileImageBase64")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolIcon")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolMod1")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolMod2")
                        .HasColumnType("TEXT");

                    b.Property<string>("NatoSymbolSize")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrbatID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentOrbatUnitID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Trigram")
                        .HasMaxLength(3)
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueDesignation")
                        .HasColumnType("TEXT");

                    b.HasKey("OrbatUnitID");

                    b.HasIndex("OrbatID");

                    b.HasIndex("ParentOrbatUnitID");

                    b.ToTable("OrbatUnit", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMap", b =>
                {
                    b.Property<int>("TacMapID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventHref")
                        .HasColumnType("TEXT");

                    b.Property<int?>("FriendlyOrbatID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("arma3");

                    b.Property<int?>("HostileOrbatID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OwnerUserID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentTacMapID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Phase")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReadOnlyToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReadWriteToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("WorldName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TacMapID");

                    b.HasIndex("FriendlyOrbatID");

                    b.HasIndex("HostileOrbatID");

                    b.HasIndex("OwnerUserID");

                    b.HasIndex("ParentTacMapID");

                    b.ToTable("TacMap", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMapAccess", b =>
                {
                    b.Property<int>("TacMapAccessID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CanWrite")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TacMapID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("TacMapAccessID");

                    b.HasIndex("TacMapID");

                    b.HasIndex("UserID");

                    b.ToTable("TacMapAccess", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMapMarker", b =>
                {
                    b.Property<int>("TacMapMarkerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("TEXT");

                    b.Property<string>("MarkerData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TacMapID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("TacMapMarkerID");

                    b.HasIndex("TacMapID");

                    b.HasIndex("UserID");

                    b.ToTable("TacMapMarker", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsService")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SteamId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserLabel")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserID");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.UserApiKey", b =>
                {
                    b.Property<int>("UserApiKeyID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("HashedKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.HasKey("UserApiKeyID");

                    b.HasIndex("UserID");

                    b.ToTable("UserApiKey", (string)null);
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.Orbat", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.OrbatUnit", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.Orbat", "Orbat")
                        .WithMany("Units")
                        .HasForeignKey("OrbatID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arma3TacMapWebApp.Entities.OrbatUnit", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentOrbatUnitID");

                    b.Navigation("Orbat");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMap", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.Orbat", "FriendlyOrbat")
                        .WithMany()
                        .HasForeignKey("FriendlyOrbatID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Arma3TacMapWebApp.Entities.Orbat", "HostileOrbat")
                        .WithMany()
                        .HasForeignKey("HostileOrbatID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Arma3TacMapWebApp.Entities.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arma3TacMapWebApp.Entities.TacMap", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentTacMapID");

                    b.Navigation("FriendlyOrbat");

                    b.Navigation("HostileOrbat");

                    b.Navigation("Owner");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMapAccess", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.TacMap", "TacMap")
                        .WithMany()
                        .HasForeignKey("TacMapID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arma3TacMapWebApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TacMap");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.TacMapMarker", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.TacMap", "TacMap")
                        .WithMany()
                        .HasForeignKey("TacMapID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arma3TacMapWebApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");

                    b.Navigation("TacMap");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.UserApiKey", b =>
                {
                    b.HasOne("Arma3TacMapWebApp.Entities.User", "User")
                        .WithMany("ApiKeys")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.Orbat", b =>
                {
                    b.Navigation("Units");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.OrbatUnit", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("Arma3TacMapWebApp.Entities.User", b =>
                {
                    b.Navigation("ApiKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
