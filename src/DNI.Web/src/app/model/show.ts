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

    public get showNotesFormatted(): string {
        // Replace links with anchors and line breaks with HTML breaks
        return this.showNotes
            .replace(/(https?:\/\/[\w\-\.~:\/\?#\[\]@!\$&'\(\)*+;=,]{3,})([\b\.\s])/g, '<a href="$1">$1</a>$2')
            .replace(/(?:\r\n|\r|\n)/g, '<br/>');
    }

    public get durationMinutes(): string {
        const mins = this.durationInSeconds / 60;
        const seconds = Math.ceil((mins % 1) * 60);
        return `${Math.floor(mins)}m ${seconds}s`;
    }

    public shown: boolean;
}
