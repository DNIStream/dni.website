import { Component, OnInit, HostListener } from '@angular/core';

import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { DiscoveryPlatform } from '../../model/discovery-platform';
import { DataService } from '../../services/data/data.service';
import { PlatformService } from '../../services/platform/platform.service';

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

  constructor(
    private dataService: DataService,
    public platform: PlatformService
  ) { }

  @HostListener('click', ['$event.target.id'])
  protected onComponentClicked(id: string): void {
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
        return throwError(err);
      }))
      .subscribe(discoveryPlatforms => {
        this.discoveryPlatforms = discoveryPlatforms.sort((p1, p2) => p1.name.localeCompare(p2.name));
      });
  }
}
