﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Whisper.DataManager.Models
{
    public partial class User
    {
        public User()
        {
            OwnerGroup = new HashSet<Group>();
            Message = new HashSet<Message>();
            Group = new HashSet<Group>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string PubKey { get; set; }
        public long? ChannelId { get; set; }

        public virtual Channel Channel { get; set; }
        public virtual ICollection<Group> OwnerGroup { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Group> Group { get; set; }
    }
}