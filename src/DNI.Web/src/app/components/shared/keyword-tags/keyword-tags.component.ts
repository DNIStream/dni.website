import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'dni-keyword-tags',
  templateUrl: './keyword-tags.component.html',
  styleUrls: ['./keyword-tags.component.scss']
})
export class KeywordTagsComponent implements OnInit {

  @Input()
  public keywords: string[];

  constructor() { }

  ngOnInit() {
  }

}
