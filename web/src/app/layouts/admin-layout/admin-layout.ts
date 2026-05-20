import { Component, inject } from '@angular/core';
import { UpperCasePipe } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet, Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../core/auth/auth.service';

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
  private readonly translate = inject(TranslateService);

  /** Languages offered by the topbar switcher. */
  readonly languages = ['tr', 'en'] as const;

  /** Switches the active UI language at runtime. */
  setLanguage(lang: string): void {
    this.translate.use(lang);
  }

  /** Clears the session and returns to the login page. */
  async logout(): Promise<void> {
    this.auth.logout();
    await this.router.navigateByUrl('/login');
  }
}
