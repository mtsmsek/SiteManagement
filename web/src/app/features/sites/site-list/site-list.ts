import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

/**
 * Placeholder for the sites list. The mat-table + create/edit dialog land
 * in the next commit; this keeps the route navigable after login.
 */
@Component({
  selector: 'app-site-list',
  imports: [TranslatePipe],
  template: `<h1>{{ 'nav.sites' | translate }}</h1>`,
})
export class SiteList {}
