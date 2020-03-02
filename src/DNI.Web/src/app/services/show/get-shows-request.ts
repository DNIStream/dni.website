export class GetShowsRequest {
    constructor(
        public orderByField: string = 'PublishedTime',
        public orderByOrder: string = 'Descending',
        public currentPage: number = 1,
        public itemsPerPage: number = 7,
        public keyword: string = null
    ) { }
}
