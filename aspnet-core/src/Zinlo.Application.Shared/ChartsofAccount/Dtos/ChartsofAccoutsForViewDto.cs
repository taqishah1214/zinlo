using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class ChartsofAccoutsForViewDto
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public int AccountTypeId { get; set; }
        public long AccountSubTypeId { get; set; }
        public string AccountSubType { get; set; }
        public int ReconciliationTypeId { get; set; }
        public string AssigneeName { get; set; }
        public string  ProfilePicture { get; set; }
        public long AssigneeId { get; set; }

        public int StatusId { get; set; }

    }
}
