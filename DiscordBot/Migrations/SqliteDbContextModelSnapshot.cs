﻿// <auto-generated />
using System;
using DiscordBot.Resources.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBot.Migrations
{
    [DbContext(typeof(SqliteDbContext))]
    partial class SqliteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity("DiscordBot.Resources.Database.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProjectId");

                    b.Property<ulong>("UserId");

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("DiscordBot.Resources.Database.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("ProjectName");

                    b.Property<ulong>("UserId");

                    b.Property<bool>("isComplete");

                    b.HasKey("ProjectId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("DiscordBot.Resources.Database.ProjectFile", b =>
                {
                    b.Property<int>("FileId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("FileName");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int>("ProjectId");

                    b.Property<bool>("isActive");

                    b.HasKey("FileId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("DiscordBot.Resources.Database.Stone", b =>
                {
                    b.Property<ulong>("UserID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.HasKey("UserID");

                    b.ToTable("Stones");
                });

            modelBuilder.Entity("DiscordBot.Resources.Database.User", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserName");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}