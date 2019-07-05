import { Component, OnInit, HostListener } from '@angular/core';

import { environment } from 'environments/environment';
import { DiscoveryPlatform } from 'app/model/discovery-platform';
import { DataService } from 'app/services/data/data.service';
import { catchError } from 'rxjs/internal/operators/catchError';
import { throwError } from 'rxjs/internal/observable/throwError';

@Component({
  selector: 'dni-root',
  templateUrl: './app.component.html',
  styleUrls: [
    './app.component.scss'
  ]
})
export class AppComponent implements OnInit {
  public version: string;
  public versionText: string;

  public showListenMenu: boolean = false;

  public discoveryPlatforms: DiscoveryPlatform[];

  constructor(private dataService: DataService) { }

  @HostListener('click', ['$event.target.id'])
  private onComponentClicked(id: string): void {
    if (id !== 'listenDropdown') {
      this.showListenMenu = false;
    }
  }

  ngOnInit(): void {
    this.version = environment.version;
    this.versionText = environment.versionText;

    this.loadDiscoveryPlatforms();
  }

  private loadDiscoveryPlatforms(): void {
    this.dataService
      .discoveryPlatforms()
      .pipe(catchError(err => {
        console.log(err);
        return throwError(err);
      }))
      .subscribe(discoveryPlatforms => {
        this.discoveryPlatforms = discoveryPlatforms.sort((p1, p2) => p1.name.localeCompare(p2.name));
      });
  }
}
