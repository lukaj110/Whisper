﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Whisper.DataManager.Models
{
    public partial class Channel
    {
        public Channel()
        {
            Group = new HashSet<Group>();
            Message = new HashSet<Message>();
            User = new HashSet<User>();
        }

        public long ChannelId { get; set; }

        public virtual ICollection<Group> Group { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}