using System;
using System.Linq;

namespace Warranty.Core.Extensions
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NPoco;

    public static class DatabaseExtensions
    {
        private static TResult GetResult<TModel, TResult>(IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression, Func<IDatabase, string, object[], TResult> result)
        {
            var ev = db.DatabaseType.ExpressionVisitor<TModel>(db, true);
            var query = ev.Where(expression).Select(columns);

            return result(db, query.Context.ToSelectStatement(), query.Context.Params.ToArray());
        }

        private static TModel GetResult<TModel>(IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression, Func<IDatabase, string, object[], TModel> result)
        {
            return GetResult<TModel, TModel>(db, columns, expression, result);
        }

        public static TResult Single<TModel, TResult>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.Single<TResult>(select, args));
        }

        public static TModel Single<TModel>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult<TModel>(db, columns, expression, (d, select, args) => d.Single<TModel>(select, args));
        }

        public static TResult SingleOrDefault<TModel, TResult>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.SingleOrDefault<TResult>(select, args));
        }

        public static TModel SingleOrDefault<TModel>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult<TModel>(db, columns, expression, (d, select, args) => d.SingleOrDefault<TModel>(select, args));
        }

        public static IEnumerable<TResult> Query<TModel, TResult>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.Query<TResult>(select, args));
        }

        public static IEnumerable<TModel> Query<TModel>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.Query<TModel>(select, args));
        }

        public static IEnumerable<TResult> Fetch<TModel, TResult>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.Fetch<TResult>(select, args));
        }

        public static IEnumerable<TModel> Fetch<TModel>(this IDatabase db, Expression<Func<TModel, object>> columns, Expression<Func<TModel, bool>> expression)
        {
            return GetResult(db, columns, expression, (d, select, args) => d.Fetch<TModel>(select, args));
        }
    }
}
