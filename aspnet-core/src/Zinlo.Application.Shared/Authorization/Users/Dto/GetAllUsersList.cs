using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users.Dto
{
   public class GetAllUsersList
    {
        public long id { get; set; }
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }
        public DateTime CreationTime { get; set; }
        public string UserName { get; set; }

    }
}
