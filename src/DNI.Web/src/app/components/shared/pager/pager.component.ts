import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { ThrowStmt } from '@angular/compiler';
import { SafeHtml } from '@angular/platform-browser';

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
  public currentPage: Observable<number>;

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
  public firstText: string = '&laquo;';

  @Input()
  public lastText: string = '&raquo;';


  @Output()
  public pageChanged = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  public onPageChanged(pageInfo: any): void {
    console.log('child onPageChanged');
    this.pageChanged.emit(pageInfo);
  }
}
