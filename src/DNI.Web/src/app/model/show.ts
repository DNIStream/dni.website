export class Show {
    public title: string;
    public summary: string;
    public showNotes: string;
    public publishedTime: Date;
    public audioUrl: string;
    public videoUrl: string;
    public version: number; // decimal
    public imageUrl: string;
    public vodPageUrl: string;
    public podcastPageUrl: string;
    public durationSeconds: number;

    public get showNotesFormatted(): string {
        // Replace links with anchors and line breaks with HTML breaks
        return this.showNotes
            .replace(/(https?:\/\/[\w\-\.~:\/\?#\[\]@!\$&'\(\)*+;=,]{3,})([\b\.\s])/g, '<a href="$1">$1</a>$2')
            .replace(/(?:\r\n|\r|\n)/g, '<br/>');
    }

    public get durationMinutes(): string {
        const mins = this.durationSeconds / 60;
        const seconds = Math.floor((mins % 1) * 60);
        return `${Math.floor(mins)}m ${seconds}s`;
    }

    public shown: boolean;
}
