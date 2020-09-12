import { Component, OnInit, Input } from '@angular/core';

import { ShowKeyword } from '../../../model/show-keyword';

@Component({
  selector: 'dni-keyword-tags',
  templateUrl: './keyword-tags.component.html',
  styleUrls: ['./keyword-tags.component.scss']
})
export class KeywordTagsComponent implements OnInit {

  @Input()
  public keywords: ShowKeyword[];

  constructor() { }

  ngOnInit() {
  }
}
