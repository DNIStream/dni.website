import { Component, OnInit } from '@angular/core';
import { ShowService } from 'app/services/show/show.service';
import { Show } from 'app/model/show';

@Component({
  selector: 'dni-show-archive',
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit {

  public model: Show[];

  constructor(
    private showService: ShowService
  ) { }

  ngOnInit() {
    this.showService
      .getShows()
      .subscribe(shows => this.model = shows);
  }

}
