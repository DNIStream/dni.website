import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';

import { CaptchaService } from 'app/services/captcha/captcha.service';
import { ContactService } from 'app/services/contact/contact.service';
import { CaptchaBaseComponent } from 'app/services/captcha/captcha-base.component';
import { ContactModel } from 'app/services/contact/contact-model';
import { environment } from 'environments/environment';

@Component({
  selector: 'dni-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss'],
  providers: [
    CaptchaService,
    ContactService
  ]
})
export class ContactComponent extends CaptchaBaseComponent implements OnInit {

  public model: ContactModel = new ContactModel();

  public messageHtml: string = null;
  public state: 'init' | 'processing' | 'error' | 'sent';
  public reCaptchaSiteKey: string;

  constructor(
    protected captchaService: CaptchaService,
    protected contactService: ContactService,
    @Inject(PLATFORM_ID) protected platformId: Object
  ) {
    super(captchaService, platformId);
  }

  ngOnInit() {
    this.reCaptchaSiteKey = environment.recaptchaSiteKey;
    this.state = 'init';
  }

  public onSubmit(): void {
    this.state = 'processing';

    this.contactService
      .sendContactEmail(this.model)
      .subscribe(x => {
        this.state = 'sent';
        this.messageHtml = 'Thank you for your message, we\'ll get back you as soon as possible.';
      }, e => {
        this.state = 'error';
        this.messageHtml = 'There was an unexpected error when trying to send your message; please try again or contact us using one of other methods. Please let us know if this form isn\'t working!';
      });

  }
}
