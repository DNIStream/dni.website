import { ViewChild, Inject, PLATFORM_ID } from '@angular/core';

import { ReCaptcha2Component } from 'ngx-captcha';

import { CaptchaService } from 'app/services/captcha/captcha.service';
import { isPlatformBrowser } from '@angular/common';

export abstract class CaptchaBaseComponent {
    public captchaErrorMessage: string;

    public captchaValid = false;
    public captchaLoaded = false;
    public captchaReady = false;

    @ViewChild('ReCaptcha')
    protected captcha: ReCaptcha2Component;

    public get isBrowser(): boolean {
        return isPlatformBrowser(this.platformId);
    }

    constructor(
        protected captchaService: CaptchaService,
        @Inject(PLATFORM_ID) protected platformId: Object
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

    public handleCaptchaReady() {
        // Short delay to make sure the CAPTCHA is fully loaded and rendered
        setTimeout(() => {
            this.captchaReady = true;
        }, 1000);
    }
}

