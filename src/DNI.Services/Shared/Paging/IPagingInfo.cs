namespace DNI.Services.Shared.Paging {
    public interface IPagingInfo {
        int PageNumber { get; set; }

        int ItemsPerPage { get; set; }
    }
}