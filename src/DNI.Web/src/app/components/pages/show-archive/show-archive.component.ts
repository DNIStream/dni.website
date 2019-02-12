import { Component, OnInit } from '@angular/core';

import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';
import { SEOService } from 'app/services/seo/seo.service';

@Component({
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit {

  public model: Show[];

  constructor(
    private showService: ShowService,
    private seoService: SEOService
  ) { }

  ngOnInit() {
    this.seoService.setTitle('Podcast Archive');
    this.seoService.setDescription('The top-level list of our podcast and vodcast shows');

    this.showService
      .getShows()
      .subscribe(shows => this.model = shows);
  }

}
