using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Infrastruture.Persistence.Shared
{
 public class PaginationFilter
    {
        public int PageNumber { get; set ; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = null; // Busca por um termo específico (nome, descrição, etc.)
        public string SortBy { get; set; } // Ordenação por campo específico
        public string SortDirection { get; set; } = "asc"; // 'asc' ou 'desc' para ordenar
        public string FieldFilter { get; set; } = null; // Filtro para um campo específico, ex: 'name'
         public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;  //itemperpag
        }

        public PaginationFilter(int pageNumber, int pageSize, string searchTerm, string sortBy, string sortDirection, string fieldFilter)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 100 ? 100 : pageSize;
            this.SearchTerm = searchTerm;
            this.SortBy = sortBy;
            this.SortDirection = sortDirection.ToLower() == "desc" ? "desc" : "asc";
            this.FieldFilter = fieldFilter;
        }
    }
}