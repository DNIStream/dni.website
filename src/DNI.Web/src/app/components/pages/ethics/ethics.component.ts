import { Component, OnInit } from '@angular/core';

import { SEOService } from 'app/services/seo/seo.service';

@Component({
  templateUrl: './ethics.component.html',
  styleUrls: ['./ethics.component.scss']
})
export class EthicsComponent implements OnInit {

  constructor(
    private seoService: SEOService
  ) { }

  ngOnInit() {
    this.seoService.setTitle('Code of Ethics');
  }
}
