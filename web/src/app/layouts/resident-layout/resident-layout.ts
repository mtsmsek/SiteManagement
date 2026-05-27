import { Component, inject } from '@angular/core';
import { UpperCasePipe } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';
import { LanguageService, SUPPORTED_LANGUAGES, Language } from '../../core/i18n/language.service';

/**
 * Resident shell: a sidenav with the resident's own sections (bills, messages)
 * plus the shared theme/language/logout toolbar. Mirrors the admin layout but
 * scoped to the self-service area; child routes render in the outlet.
 */
@Component({
  selector: 'app-resident-layout',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    TranslatePipe,
    UpperCasePipe,
  ],
  templateUrl: './resident-layout.html',
  styleUrl: './resident-layout.scss',
})
export class ResidentLayout {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);
  private readonly theme = inject(ThemeService);

  /** Languages offered by the topbar switcher. */
  readonly languages = SUPPORTED_LANGUAGES;

  /** Whether the dark scheme is currently active (drives the toggle icon). */
  readonly isDark = this.theme.isDark;

  /** Switches the active UI language at runtime. */
  setLanguage(lang: Language): void {
    void this.language.use(lang);
  }

  /** Flips between light and dark mode. */
  toggleTheme(): void {
    this.theme.toggle();
  }

  /** Clears the session and returns to the login page. */
  async logout(): Promise<void> {
    this.auth.logout();
    await this.router.navigateByUrl('/login');
  }
}
