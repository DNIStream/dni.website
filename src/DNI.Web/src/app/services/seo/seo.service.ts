import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root'
})
export class SEOService {
  constructor(
    private title: Title
  ) { }

  public setTitle(title: string): void {
    this.title.setTitle(title + ': Documentation Not Included Development Podcast');
  }
}
