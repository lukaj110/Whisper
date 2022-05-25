using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Models
{
    public class RegisterCredential : BaseCredential
    {
        public string Email { get; set; } = default!;
        public string PubKey { get; set; } = default!;
    }

    public class BaseCredential
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}