using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class GetUserWithPicture
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }
}
