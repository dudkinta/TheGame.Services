﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StatisticDbContext;

#nullable disable

namespace StatisticDbContext.Migrations
{
    [DbContext(typeof(StatisticContext))]
    partial class StatisticContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("StatisticDbContext.Models.ArmyModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int>("barrack_id")
                        .HasColumnType("integer");

                    b.Property<int>("useType")
                        .HasColumnType("integer");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("barrack_id")
                        .IsUnique();

                    b.ToTable("armies", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.BarrackModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int>("hero_id")
                        .HasColumnType("integer");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("hero_id");

                    b.ToTable("barracks", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.HeroModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("asset")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int>("level")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("type")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("id");

                    b.ToTable("heroes", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.InventoryModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<int?>("army_id")
                        .HasColumnType("integer");

                    b.Property<int>("item_id")
                        .HasColumnType("integer");

                    b.Property<int>("user_id")
                        .HasColumnType("integer");

                    b.HasKey("id");

                    b.HasIndex("army_id");

                    b.HasIndex("item_id");

                    b.ToTable("inventory", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.ItemModel", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("id"));

                    b.Property<string>("asset")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int>("level")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("type")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("id");

                    b.ToTable("items", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.StorageModel", b =>
                {
                    b.Property<int>("user_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("user_id"));

                    b.Property<long>("aim")
                        .HasColumnType("bigint");

                    b.Property<long>("bonus_coin")
                        .HasColumnType("bigint");

                    b.Property<int>("energy")
                        .HasColumnType("integer");

                    b.Property<long>("hunts")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("last_check_energy")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("main_coin")
                        .HasColumnType("bigint");

                    b.Property<long>("refer_coin")
                        .HasColumnType("bigint");

                    b.Property<long>("shots")
                        .HasColumnType("bigint");

                    b.Property<long>("task_coin")
                        .HasColumnType("bigint");

                    b.HasKey("user_id");

                    b.ToTable("storage", (string)null);
                });

            modelBuilder.Entity("StatisticDbContext.Models.ArmyModel", b =>
                {
                    b.HasOne("StatisticDbContext.Models.BarrackModel", "barrack")
                        .WithOne("army")
                        .HasForeignKey("StatisticDbContext.Models.ArmyModel", "barrack_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("barrack");
                });

            modelBuilder.Entity("StatisticDbContext.Models.BarrackModel", b =>
                {
                    b.HasOne("StatisticDbContext.Models.HeroModel", "hero")
                        .WithMany()
                        .HasForeignKey("hero_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("hero");
                });

            modelBuilder.Entity("StatisticDbContext.Models.InventoryModel", b =>
                {
                    b.HasOne("StatisticDbContext.Models.ArmyModel", "army")
                        .WithMany("equip")
                        .HasForeignKey("army_id");

                    b.HasOne("StatisticDbContext.Models.ItemModel", "item")
                        .WithMany()
                        .HasForeignKey("item_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("army");

                    b.Navigation("item");
                });

            modelBuilder.Entity("StatisticDbContext.Models.ArmyModel", b =>
                {
                    b.Navigation("equip");
                });

            modelBuilder.Entity("StatisticDbContext.Models.BarrackModel", b =>
                {
                    b.Navigation("army");
                });
#pragma warning restore 612, 618
        }
    }
}
