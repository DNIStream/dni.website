/**
 * angular2-cookie-law
 *
 * Copyright 2016-2018, @andreasonny83, All rights reserved.
 *
 * @author: @andreasonny83 <andreasonny83@gmail.com>
 */

import {
  Component,
  OnInit,
  HostBinding,
  ViewEncapsulation,
  Input,
  Output,
  EventEmitter,
} from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { AnimationEvent } from '@angular/animations';
import { closeIcon } from './icons';
import { translateInOut } from './animations';
import { CookieLawAnimation, CookieLawTarget, CookieLawPosition } from './definitions';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'cookie-law-component',
  templateUrl: './angular2-cookie-law.component.html',
  styleUrls: ['./angular2-cookie-law.component.css'],
  animations: [translateInOut],
  encapsulation: ViewEncapsulation.None,
})
export class CookieLawComponent implements OnInit {

  public closeSvg: SafeHtml;
  public currentStyles: any;
  public transition: CookieLawAnimation;

  @HostBinding('class.cookie-law')
  public cookieLawClass: boolean;

  @Input()
  get learnMore() { return this._learnMore; }
  set learnMore(value: string) {
    this._learnMore = (value !== null && `${value}` !== 'false') ? value : null;
  }

  @Input()
  get awsomeCloseIcon() { return this._awsomeCloseIcon; }
  set awsomeCloseIcon(value: string) {
    this._awsomeCloseIcon = (value !== null && `${value}` !== 'false') ? value : null;
  }

  @Input()
  get target() { return this._target; }
  set target(value: CookieLawTarget) {
    this._target = (value !== null && `${value}` !== 'false' &&
      (`${value}` === '_blank' || `${value}` === '_self')
    ) ? value : '_blank';
  }

  @Input()
  get position() { return this._position; }
  set position(value: CookieLawPosition) {
    this._position = (value !== null && `${value}` !== 'false' &&
      (`${value}` === 'top' || `${value}` === 'bottom')
    ) ? value : 'bottom';
  }

  @Output()
  public isSeen = new EventEmitter<boolean>();

  public noopener: boolean;

  private _learnMore: string;
  private _awsomeCloseIcon: string;
  private _target: CookieLawTarget;
  private _position: CookieLawPosition;

  constructor(
    private domSanitizer: DomSanitizer,
  ) {
    this.transition = 'bottomIn';
    this._position = 'bottom';
    this.cookieLawClass = true;
  }

  public ngOnInit(): void {
    this.noopener = this._target === '_blank';
    this.transition = this.position === 'bottom' ? 'bottomIn' : 'topIn';

    if (this._awsomeCloseIcon) {
      this.closeSvg = this.domSanitizer.bypassSecurityTrustHtml(
        `<i class="fab ${this._awsomeCloseIcon}"></i>`);
    } else {
      this.closeSvg = this.domSanitizer.bypassSecurityTrustHtml(closeIcon);
    }


    this.currentStyles = {
      'top': this.position === 'top' ? '0' : null,
      'bottom': this.position === 'top' ? 'initial' : null,
    };
  }

  public afterDismissAnimation(evt: AnimationEvent): void {
    if (evt.toState === 'topOut' ||
      evt.toState === 'bottomOut') {
      this.isSeen.emit(true);
    }
  }

  public dismiss(evt?: MouseEvent): void {
    if (evt) {
      evt.preventDefault();
    }

    this.transition = this.position === 'top' ? 'topOut' : 'bottomOut';
  }

}
