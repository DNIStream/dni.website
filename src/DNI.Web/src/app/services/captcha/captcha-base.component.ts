import { ViewChild } from '@angular/core';

import { ReCaptcha2Component } from 'ngx-captcha';
import { CaptchaService } from 'app/services/captcha/captcha.service';

export abstract class CaptchaBaseComponent {
    public captchaErrorMessage: string;

    public captchaValid = false;
    public captchaLoaded = false;

    @ViewChild('ReCaptcha')
    protected captcha: ReCaptcha2Component;

    constructor(
        protected captchaService: CaptchaService,
    ) { }

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
}

