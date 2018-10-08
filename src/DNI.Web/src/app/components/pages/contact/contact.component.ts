import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';

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

  public processing: boolean = false;
  public sent: boolean = false;
  public error: boolean = false;
  public messageHtml: string = null;

  public reCaptchaSiteKey: string;

  constructor(
    protected captchaService: CaptchaService,
    protected contactService: ContactService
  ) {
    super(captchaService);
  }

  ngOnInit() {
    this.reCaptchaSiteKey = environment.recaptchaSiteKey;
  }

  public onSubmit(): void {
    this.processing = true;
    this.error = false;
    this.sent = false;

    this.contactService
      .sendContactEmail(this.model)
      .pipe(
        finalize(() => {
          this.sent = true;
          this.processing = false;
        }))
      .subscribe(x => {
        this.messageHtml = '<p>Thank you for your message, we\'ll get back you as soon as possible.</p>';
      }, e => {
        this.error = true;
        this.messageHtml = 'There was an unexpected error when trying to send your message; please try again or contact us using one of other methods. Please let us know if this form isn\'t working!';
      });

  }
}
