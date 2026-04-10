using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Hotel.Domain.Interface.Shared
{
   public class PaginationFilter
    {
        public int PageNumber { get; set ; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null; // Busca por um termo específico (nome, descrição, etc.)
        public string SortBy { get; set; } // Ordenação por campo específico
        public string SortDirection { get; set; } = "asc"; // 'asc' ou 'desc' para ordenar
        public string FieldFilter { get; set; } = null; // Filtro para um campo específico, ex: 'name'
        public int EmpresaId {get; set;} = 0;
         public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;  //itemperpag
        }

       /*  public PaginationFilter(int pageNumber, int pageSize, string searchTerm, string sortBy, string sortDirection, string fieldFilter)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 100 ? 100 : pageSize;
            this.SearchTerm = searchTerm;
            this.SortBy = sortBy;
            this.SortDirection = sortDirection.ToLower() == "desc" ? "desc" : "asc";
            this.FieldFilter = fieldFilter;
        } */
        public PaginationFilter(int? pageNumber, int? pageSize, string searchTerm = null, string sortBy = null, string sortDirection = "asc", string fieldFilter = null, int? empresaId = 0)
    {
        this.PageNumber = pageNumber.HasValue && pageNumber > 0 ? pageNumber.Value : 1;
        this.PageSize = pageSize.HasValue && pageSize <= 100 ? pageSize.Value : 10;
        this.SearchTerm = searchTerm;
        this.SortBy = sortBy;
        this.SortDirection = sortDirection?.ToLower() == "desc" ? "desc" : "asc";
        this.FieldFilter = fieldFilter;
        this.EmpresaId = (int)empresaId;
    }
    }
}