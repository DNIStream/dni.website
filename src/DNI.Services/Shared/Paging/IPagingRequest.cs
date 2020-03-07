namespace DNI.Services.Shared.Paging {
    public interface IPagingRequest {
        int PageNumber { get; set; }

        int ItemsPerPage { get; set; }
    }
}