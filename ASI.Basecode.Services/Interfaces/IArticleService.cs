﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IArticleService
    {
        IEnumerable<ArticleViewModel> RetrieveAll();
        void Add(ArticleViewModel article);
        void Update(ArticleViewModel article);
        void Delete(ArticleViewModel article);
        IEnumerable<Category> GetCategories();
        IEnumerable<ArticleViewModel> RetrieveFavorites();
        void AddFavorite(string articleId);
        void DeleteFavorite(string articleId);
        int GetUserFavoriteCount();
    }
}
