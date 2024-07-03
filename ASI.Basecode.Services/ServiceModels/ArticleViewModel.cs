﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ArticleViewModel
    {
        public string ArticleId { get; set; }

        [Required(ErrorMessage = "Article Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Article Content is required")]
        public string Body { get; set; }
        public byte? Category { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
