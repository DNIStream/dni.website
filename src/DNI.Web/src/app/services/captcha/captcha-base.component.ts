import { ViewChild, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ReCaptcha2Component } from 'ngx-captcha';

import { CaptchaService } from 'app/services/captcha/captcha.service';
import { PlatformService } from 'app/services/platform/platform.service';

@Component({
    template: ''
})
export abstract class CaptchaBaseComponent {
    public captchaErrorMessage: string;

    public captchaValid = false;
    public captchaLoaded = false;
    public captchaReady = false;

    public reCaptchaFormGroup: FormGroup;

    @ViewChild('ReCaptcha')
    protected captcha: ReCaptcha2Component;

    constructor(
        protected captchaService: CaptchaService,
        protected formBuilder: FormBuilder,
        public platform: PlatformService
    ) {
        this.reCaptchaFormGroup = this.formBuilder.group({
            recaptcha: ['', Validators.required]
        });
    }

    public handleCaptchaResponse(userResponse: string) {
        this.captchaErrorMessage = null;
        this.captchaService
            .verify(userResponse)
            .subscribe(() => {
                this.captchaValid = true;
            }, () => {
                this.captchaErrorMessage = 'An error occurred when validating the CAPTCHA. If this error occurs again, please contact support.';
                this.captchaValid = false;
                this.captcha.resetCaptcha();
            });
    }

    public handleCaptchaExpired() {
        this.captchaValid = false;
    }

    public handleCaptchaLoaded() {
        this.captchaLoaded = true;
    }

    public handleCaptchaReady() {
        this.captchaReady = true;
    }

    public handleReset() {
        this.captchaValid = false;
    }
}

