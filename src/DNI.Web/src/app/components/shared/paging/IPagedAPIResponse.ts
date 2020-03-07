export interface IPagedAPIResponse {
    currentPage: number;
    totalRecords: number;
    totalPages: number;
    startIndex: number;
    endIndex: number;
    itemsPerPage: number;
}
