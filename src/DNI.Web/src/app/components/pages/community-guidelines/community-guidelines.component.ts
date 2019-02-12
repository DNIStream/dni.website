import { Component, OnInit } from '@angular/core';

import { SEOService } from 'app/services/seo/seo.service';

@Component({
  templateUrl: './community-guidelines.component.html',
  styleUrls: ['./community-guidelines.component.scss']
})
export class CommunityGuidelinesComponent implements OnInit {

  constructor(
    private seoService: SEOService
  ) { }

  ngOnInit() {
    this.seoService.setTitle('Community Guidelines');
  }
}
