import { ShowKeyword } from './show-keyword';

export class Show {
    public title: string;
    public summary: string;
    public showNotes: string;
    public showNotesHtml: string;
    public publishedTime: Date;
    public audioUrl: string;
    public version: number; // decimal
    public imageUrl: string;
    public slug: string;
    public podcastPageUrl: string;
    public duration: string;
    public durationInSeconds: number;
    public keywords: string[];

    public getKeywords(): ShowKeyword[] {
        return this.keywords
            .map(s => new ShowKeyword(s, null));
    }
}
