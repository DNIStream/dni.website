import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'dni-pager',
  templateUrl: './pager.component.html',
  styleUrls: ['./pager.component.scss']
})
export class PagerComponent implements OnInit {

  @Input()
  public totalItems: number = 0;

  @Input()
  public itemsPerPage: number = 7;

  @Input()
  public totalPages: number = 0;

  @Input()
  public currentPage: number = 1;

  @Input()
  public directionLinks: boolean = true;

  @Input()
  public boundaryLinks: boolean = true;

  @Input()
  public maxSize: number = 5;

  @Input()
  public rotate: boolean = true;

  @Input()
  public showTotals: boolean = true;

  @Input()
  public itemType: string = 'items';

  @Input()
  public previousText: string = '&lsaquo;';

  @Input()
  public nextText: string = '&rsaquo;';

  @Input()
  public firstText = '&laquo;';

  @Input()
  public lastText = '&raquo;';

  @Input()
  public items: any[] = null;

  @Output()
  public pageChanged = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  public onPageChanged(pageInfo: any) {
    this.pageChanged.emit(pageInfo);
  }

}
