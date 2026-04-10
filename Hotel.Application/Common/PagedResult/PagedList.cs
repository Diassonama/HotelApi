using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.Common.PagedResult
{
 public class PagedList<T> // : BaseCommandResponse
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public List<T> Data { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
         public bool Success { get;  set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            if (items is null)
            {
                Success = false;
                Message = "Dado(s) não encontrado";

            }

            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Data = items;
            Success = true;
            Message = "Dados carregados com sucesso";
        }
        public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {

            var count = source.Count();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        
    }
}