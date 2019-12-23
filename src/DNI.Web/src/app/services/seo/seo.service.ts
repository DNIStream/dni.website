import { Injectable } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class SEOService {
  constructor(
    private title: Title,
    private meta: Meta
  ) { }

  public setTitle(title: string): void {
    this.title.setTitle(title + ': DNI Stream Development Podcast');
  }

  public setDescription(description: string): void {
    this.meta.updateTag({ name: 'description', content: description });
  }
}
