namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     Provides an interface for ordering API results
    /// </summary>
    public interface ISortingInfo {
        /// <summary>
        ///     A sorted dictionary of the fields to sort by, along with their corresponding order
        /// </summary>
        FieldSort SortByField { get; set; }
    }
}