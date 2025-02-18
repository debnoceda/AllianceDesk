﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// A model for service
    /// </summary>
    public class NotificationServiceModel
    {
        public string NotificationId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string DateCreated { get; set; }
        public string RecipientId { get; set; }
        public List<NotificationServiceModel> UserNotifications { get; set; }
        public string TicketStatus { get; set; }
        public string AgentName { get; set; }
        public byte RoleId { get; set; }
    }
}
