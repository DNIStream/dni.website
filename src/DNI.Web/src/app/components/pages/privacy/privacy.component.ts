import { Component, OnInit } from '@angular/core';

import { SEOService } from 'app/services/seo/seo.service';

@Component({
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.scss']
})
export class PrivacyComponent implements OnInit {

  constructor(
    private seoService: SEOService
  ) { }

  ngOnInit() {
    this.seoService.setTitle('Cookie and Privacy Policy');
    this.seoService.setDescription('The lega stuff - our cookie and privacy policy covering how we gather and process your data');
  }
}
