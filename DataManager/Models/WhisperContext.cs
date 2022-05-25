﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Whisper.DataManager.Models
{
    public partial class WhisperContext : DbContext
    {
        public WhisperContext()
        {
        }

        public WhisperContext(DbContextOptions<WhisperContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<Channel> Channel { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserMessages> UserMessages { get; set; }

        public User FromUsername(string Username) => User.Include(e=>e.Group).Single(e => e.Username == Username);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=Whisper;User Id=postgres;Password=testing123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum("ContentType", new[] { "Image", "Video", "Document" });

            var converter = new EnumToStringConverter<ContentType>();

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Content).HasConversion(converter);
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.Property(e => e.ChannelId).HasDefaultValueSql("pseudo_encrypt((nextval('channelid_sequence'::regclass))::integer)");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.Group)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_GroupChannel");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.OwnerGroup)
                    .HasForeignKey(d => d.Owner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupOwner");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.Checksum).IsRequired();

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.SentAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.ChannelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MessageChannel");

                entity.HasOne(d => d.SenderNavigation)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.Sender)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MessageUser");

                entity.HasMany(d => d.Attachment)
                    .WithMany(p => p.Message)
                    .UsingEntity<Dictionary<string, object>>(
                        "MessageAttachment",
                        l => l.HasOne<Attachment>().WithMany().HasForeignKey("AttachmentId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Attachments_Attachment"),
                        r => r.HasOne<Message>().WithMany().HasForeignKey("MessageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Attachments_Message"),
                        j =>
                        {
                            j.HasKey("MessageId", "AttachmentId").HasName("Message_Attachment_pkey");

                            j.ToTable("Message_Attachment");
                        });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(320);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PubKey).IsRequired();

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_UserChannel");

                entity.HasMany(d => d.Group)
                    .WithMany(p => p.User)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserGroup",
                        l => l.HasOne<Group>().WithMany().HasForeignKey("GroupId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Groups_User"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Groups_Group"),
                        j =>
                        {
                            j.HasKey("UserId", "GroupId").HasName("User_Group_pkey");

                            j.ToTable("User_Group");
                        });
            });

            modelBuilder.Entity<UserMessages>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("User_Messages");

                entity.Property(e => e.User).HasMaxLength(320);
            });

            modelBuilder.HasSequence("channelid_sequence");

            modelBuilder.HasSequence("serial").StartsAt(101);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}