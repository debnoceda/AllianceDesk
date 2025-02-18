﻿using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Article
    {
        public Article()
        {
            Favorites = new HashSet<Favorite>();
        }

        public Guid ArticleId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public byte? CategoryId { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
    }
}
