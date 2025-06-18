using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;

namespace Application.Extensions.Specification
{
    public static class SpecificationExtension
    {
        public static ISpecificationBuilder<T> Paginate<T>(this ISpecificationBuilder<T> query, int pageSize, int pageNumber)
        {
            query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return query;
        }

        public static ISpecificationBuilder<T> DateTimeRangeWhere<T>(this ISpecificationBuilder<T> query, string dateTimePropertyName, DateTime startDate, DateTime endDate)
        {
            return query.Where(x => EF.Property<DateTime>(x!, dateTimePropertyName) >= startDate
            && EF.Property<DateTime>(x!, dateTimePropertyName) <= endDate);

        }

        public static ISpecificationBuilder<T> DateRangeWhere<T>(this ISpecificationBuilder<T> query, string dateTimePropertyName, DateTime startDate, DateTime endDate)
        {
            return query.Where(x => EF.Property<DateTime>(x!, dateTimePropertyName).Date >= startDate
            && EF.Property<DateTime>(x!, dateTimePropertyName).Date <= endDate);
        }
    }
}
