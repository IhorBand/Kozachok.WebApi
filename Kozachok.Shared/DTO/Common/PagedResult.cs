using System.Collections.Generic;

namespace Kozachok.Shared.DTO.Common
{
    public class PagedResult<TEntity>
    {
        public PagedResult(int currentPage, int totalPages, int totalItems, int pageSize, List<TEntity> items)
        {
            this.CurrentPage = currentPage;
            this.TotalPages = totalPages;
            this.Items = items;
            this.PageSize = pageSize;
            this.TotalItems = totalItems;
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public List<TEntity> Items { get; set; }
    }
}
