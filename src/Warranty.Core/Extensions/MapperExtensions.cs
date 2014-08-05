namespace Warranty.Core.Extensions
{
    using System.Collections.Generic;
    using StructureMap;
    using global::AutoMapper;

    public static class MapperExtensions
    {
        public static IMapExpression<TSource> Map<TSource>(this TSource source)
        {
            return new MapExpression<TSource>(source);
        }

        public static IEnumerableMapExpression<IEnumerable<TSource>> MapAll<TSource>(this IEnumerable<TSource> source)
        {
            return new CollectionMapExpression<IEnumerable<TSource>>(source);
        }

        private class MapExpression<TSource> : IMapExpression<TSource>
        {
            private readonly TSource _source;

            public MapExpression(TSource source)
            {
                _source = source;
            }

            public TDestination To<TDestination>(IContainer container)
            {
                return Mapper.Map<TSource, TDestination>(_source, o => o.ConstructServicesUsing(container.GetInstance));
            }

            public TDestination To<TDestination>()
            {
                return Mapper.Map<TSource, TDestination>(_source);
            }
        }

        private class CollectionMapExpression<TSource> : IEnumerableMapExpression<TSource>
        {
            private readonly TSource _source;

            public CollectionMapExpression(TSource source)
            {
                _source = source;
            }

            public IList<TDestination> To<TDestination>(IContainer container)
            {
                return Mapper.Map<TSource, IList<TDestination>>(_source, o => o.ConstructServicesUsing(container.GetInstance));
            }

            public IList<TDestination> To<TDestination>()
            {
                return Mapper.Map<TSource, IList<TDestination>>(_source);
            }
        }

        public interface IMapExpression<TSource>
        {
            TDestination To<TDestination>();
            TDestination To<TDestination>(IContainer container);
        }

        public interface IEnumerableMapExpression<TSource>
        {
            IList<TDestination> To<TDestination>();
            IList<TDestination> To<TDestination>(IContainer container);
        }
    }
}
