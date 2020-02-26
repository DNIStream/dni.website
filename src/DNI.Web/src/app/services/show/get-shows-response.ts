import { Show } from 'app/model/show';
import { IPagedAPIResponse } from 'app/components/shared/paging/IPagedAPIResponse';
import { ShowKeyword } from 'app/model/show-keyword';

export class GetShowsResponse {
    public pageInfo: IPagedAPIResponse;

    public pagedShows: Show[];

    public showKeywords: { [key: string]: number; };

    public latestShow: Show;

    public getKeywords(): ShowKeyword[] {
        return Object.keys(this.showKeywords)
            .map(s => new ShowKeyword(s, this.showKeywords[s]));
    }
}
