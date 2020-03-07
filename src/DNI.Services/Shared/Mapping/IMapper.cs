namespace DNI.Services.Shared.Mapping {
    /// <summary>
    ///     Provides a generic mapping interface for domain model mapping
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    public interface IMapper<in TSource, out TDestination> {
        /// <summary>
        ///     Maps one object to another
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        TDestination Map(TSource source);
    }
}