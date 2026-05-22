import { Component, inject, signal } from '@angular/core';
import { UpperCasePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth/auth.service';
import { ThemeService } from '../../../core/theme/theme.service';
import { LanguageService, SUPPORTED_LANGUAGES, Language } from '../../../core/i18n/language.service';

/**
 * Admin sign-in page. Reactive form, posts to AuthService.login, then
 * routes to the returnUrl (or /admin) on success. Wrong credentials show an
 * inline error fed by the API's 401.
 */
@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatProgressBarModule,
    TranslatePipe,
    UpperCasePipe,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly theme = inject(ThemeService);
  private readonly language = inject(LanguageService);

  /** Whether the dark scheme is currently active (drives the toggle icon). */
  readonly isDark = this.theme.isDark;

  /** Languages offered by the switcher. */
  readonly languages = SUPPORTED_LANGUAGES;

  /** True while the login request is in flight. */
  readonly submitting = signal(false);

  /** Set when the API rejects the credentials (401). */
  readonly invalidCredentials = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  async submit(): Promise<void> {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.invalidCredentials.set(false);

    try {
      await this.auth.login(this.form.getRawValue());
      const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/admin';
      await this.router.navigateByUrl(returnUrl);
    } catch (error) {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        this.invalidCredentials.set(true);
      }
    } finally {
      this.submitting.set(false);
    }
  }

  /** Flips between light and dark mode. */
  toggleTheme(): void {
    this.theme.toggle();
  }

  /** Switches the active UI language. */
  setLanguage(lang: Language): void {
    void this.language.use(lang);
  }
}
