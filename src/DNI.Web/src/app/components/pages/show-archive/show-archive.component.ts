import { Component, OnInit } from '@angular/core';
import { ShowService } from 'app/services/show/show.service';

@Component({
  selector: 'dni-show-archive',
  templateUrl: './show-archive.component.html',
  styleUrls: ['./show-archive.component.scss']
})
export class ShowArchiveComponent implements OnInit {

  constructor(
    private showService: ShowService
  ) { }

  ngOnInit() {
  }

}
