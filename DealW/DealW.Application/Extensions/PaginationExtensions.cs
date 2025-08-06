using System.Linq.Dynamic.Core;
using DealW.Application.Models.Pagination;
using Microsoft.Extensions.Logging;

namespace DealW.Application.Extensions
{
    public static class PaginationExtension
    {
        public static Paginated<TData> AsPaginated<TData>(this IQueryable<TData> data, PaginationParams paginationParams, SortParams? sortParams, ILogger logger)
        {
            try
            {
                data = data.ApplySorting(sortParams);

                var result = data
                    .Skip(paginationParams.PageNumber * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToList()
                    .AsReadOnly();

                var totalItems = data.LongCount();
                var totalPages = (int)Math.Ceiling((double)(totalItems / paginationParams.PageSize));

                return new Paginated<TData>
                {
                    Items = result,
                    PaginationParams = paginationParams,
                    TotalPages = totalPages + 1,
                    HasPreviewPage = paginationParams.PageNumber > PaginationParams.StartPage,
                    HasNextPage = paginationParams.PageNumber < totalPages
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Pagination failed due to the following error: {error}", ex.Message);
                return new Paginated<TData>
                {
                    Items = new List<TData>().AsReadOnly(),
                    PaginationParams = paginationParams,
                    TotalPages = 0,
                    HasPreviewPage = false,
                    HasNextPage = false,
                    Error = ex.Message
                };
            }
        }

        private static IQueryable<TData> ApplySorting<TData>(this IQueryable<TData> data, SortParams? sortParams)
        {
            if (sortParams is not null)
            {
                var expression = $"{sortParams.SortBy} {(sortParams.IsAscending ? "asc" : "desc")}";
                return data.OrderBy(expression);
            }

            return data.Order();
        }
    }
}