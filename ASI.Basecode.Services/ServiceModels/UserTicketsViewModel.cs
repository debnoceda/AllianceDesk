﻿using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserTicketsViewModel
    {
        public IEnumerable<TicketViewModel> Tickets { get; set; }
        public TicketViewModel Ticket { get; set; }
        public FeedbackViewModel Feedback { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public byte CurrentStatus { get; set; }
        public string CurrentSearchTerm { get; set; }
        public List<KeyValuePair<string, string>> Statuses { get; set; }
        public List<KeyValuePair<string, string>> Categories { get; set; }
        public List<KeyValuePair<string, string>> Priorities { get; set; }
        public List<string> Emails { get; set; }
        public IEnumerable<ArticleViewModel> Favorites { get; set; }
        public IEnumerable<ArticleViewModel> Articles { get; set; }
    }
}
