import { Show } from 'app/model/show';
import { IPagedAPIResponse } from 'app/components/shared/paging/IPagedAPIResponse';

export class ShowResponse {
    public pageInfo: IPagedAPIResponse;

    public pagedShows: Show[];

    public showKeywords: { [key: string]: number; };

    public latestShow: Show;
}
