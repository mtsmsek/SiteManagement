import { Component, computed, DestroyRef, inject, signal, ViewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpperCasePipe } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/theme/theme.service';
import { LanguageService, SUPPORTED_LANGUAGES, Language } from '../../core/i18n/language.service';

/**
 * Admin shell: a persistent sidenav with the section links plus a toolbar
 * carrying the language switcher and logout. Child routes render in the
 * <router-outlet>.
 */
@Component({
  selector: 'app-admin-layout',
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
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.scss',
})
export class AdminLayout {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);
  private readonly theme = inject(ThemeService);
  private readonly breakpoints = inject(BreakpointObserver);
  private readonly destroyRef = inject(DestroyRef);

  /** Reference to the sidenav so the hamburger button can toggle it on mobile. */
  @ViewChild(MatSidenav) sidenav?: MatSidenav;

  /** True when the viewport is narrower than the tablet breakpoint (phone). */
  private readonly isMobileSignal = signal(false);

  /** Languages offered by the topbar switcher. */
  readonly languages = SUPPORTED_LANGUAGES;

  /** Whether the dark scheme is currently active (drives the toggle icon). */
  readonly isDark = this.theme.isDark;

  /** SideNav mode: "side" on tablet+ (always visible), "over" on phone (drawer). */
  readonly sidenavMode = computed(() => (this.isMobileSignal() ? 'over' : 'side'));

  /** SideNav initial opened state: open on tablet+, closed on phone. */
  readonly sidenavOpened = computed(() => !this.isMobileSignal());

  /** True only on phone; toolbar shows the hamburger button. */
  readonly isMobile = this.isMobileSignal.asReadonly();

  constructor() {
    this.breakpoints
      .observe([Breakpoints.Handset, Breakpoints.Small])
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((state) => this.isMobileSignal.set(state.matches));
  }

  /** Hamburger handler: opens/closes the drawer when sidenav is in "over" mode. */
  toggleSidenav(): void {
    void this.sidenav?.toggle();
  }

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
