import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';

import { environment } from 'environments/environment';

import { CaptchaService } from 'app/services/captcha/captcha.service';
import { ContactService } from 'app/services/contact/contact.service';
import { CaptchaBaseComponent } from 'app/services/captcha/captcha-base.component';
import { ContactModel } from 'app/services/contact/contact-model';
import { SEOService } from 'app/services/seo/seo.service';
import { PlatformService } from 'app/services/platform/platform.service';

@Component({
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent extends CaptchaBaseComponent implements OnInit {

  public model: ContactModel = new ContactModel();

  public messageHtml: string = null;
  public state: 'init' | 'processing' | 'error' | 'sent';
  public reCaptchaSiteKey: string;

  constructor(
    protected captchaService: CaptchaService,
    protected contactService: ContactService,
    public platform: PlatformService,
    protected formBuilder: FormBuilder,
    private seoService: SEOService
  ) {
    super(captchaService, formBuilder, platform);

    this.seoService.setTitle('Contact Us');
    this.seoService.setDescription('Contact us to enquire about being a guest on the show or if you have any comments or suggestions.');
  }

  ngOnInit() {
    this.reCaptchaSiteKey = environment.recaptchaSiteKey;
    this.state = 'init';
  }

  public onSubmit(): void {
    this.state = 'processing';

    this.contactService
      .sendContactEmail(this.model)
      .subscribe(() => {
        this.state = 'sent';
        this.messageHtml = 'Thank you for your message, we\'ll get back you as soon as possible.';
      }, () => {
        this.state = 'error';
        this.messageHtml = 'There was an unexpected error when trying to send your message; please try again or contact us using one of other methods. Please let us know if this form isn\'t working!';
      });

  }
}
