namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     Provides an interface for ordering API results
    /// </summary>
    public interface ISortingInfo {
        /// <summary>
        ///     The field name to sort by
        /// </summary>
        
        string Field { get; set; }

        /// <summary>
        ///     The sort order
        /// </summary>
        FieldOrder Order { get; set; }
    }
}