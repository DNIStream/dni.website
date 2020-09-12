import { Component, OnInit } from '@angular/core';

import { SEOService } from '../../../services/seo/seo.service';

@Component({
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.scss']
})
export class PageNotFoundComponent implements OnInit {

  constructor(
    private seoService: SEOService
  ) { }

  ngOnInit() {
    this.seoService.setTitle('404 Page Not Found');
    this.seoService.setDescription('404 Page Not Found');
  }
}
