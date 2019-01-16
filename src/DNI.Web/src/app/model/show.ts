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
        return this.showNotes.replace(/(?:\r\n|\r|\n)/g, '<br/>');
    }

    public get durationMinutes(): string {
        const mins = this.durationSeconds / 60;
        const seconds = Math.floor((mins % 1) * 60);
        return `${Math.floor(mins)}m ${seconds}s`;
    }

    public shown: boolean;
}
