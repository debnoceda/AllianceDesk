﻿using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class UserPreference
    {
        public Guid PreferenceId { get; set; }
        public Guid UserId { get; set; }
        public bool InAppNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public string DefaultTicketView { get; set; }
        public byte DefaultTicketPerPage { get; set; }

        public virtual User User { get; set; }
    }
}
