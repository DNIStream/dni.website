namespace DNI.Services.Shared.Sorting {
    /// <summary>
    ///     Specifies a field name to sort by and the sort order
    /// </summary>
    public class FieldSort {
        /// <summary>
        ///     The field name to sort by
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        ///     The sort order
        /// </summary>
        public FieldOrder Order { get; set; }
    }
}