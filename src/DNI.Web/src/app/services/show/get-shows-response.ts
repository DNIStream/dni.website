import { IPagedAPIResponse } from '../../components/shared/paging/IPagedAPIResponse';
import { Show } from '../../model/show';
import { ShowKeyword } from '../../model/show-keyword';

export class GetShowsResponse {
    public pageInfo: IPagedAPIResponse;

    public pagedShows: Show[];

    public showKeywords: { [key: string]: number; };

    public latestShow: Show;

    public getKeywords(): ShowKeyword[] {
        return Object.keys(this.showKeywords)
            .map(s => new ShowKeyword(s, this.showKeywords[s]));
    }

    public getTopKeywords(top: number = 10): ShowKeyword[] {
        return this.getKeywords()
            .sort((a, b) => {
                const countSort = (a.count * -1) - (b.count * -1);
                let keywordSort: number;
                if (a.keyword < b.keyword) {
                    keywordSort = -1;
                } else if (a.keyword > b.keyword) {
                    keywordSort = 1;
                } else {
                    keywordSort = 0;
                }

                return countSort || keywordSort;
            })
            .slice(0, top);
    }
}
