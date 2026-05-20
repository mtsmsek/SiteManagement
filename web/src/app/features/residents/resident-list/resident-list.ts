import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

/**
 * Placeholder for the residents list. The mat-table + form land in the
 * next commit; this keeps the route navigable after login.
 */
@Component({
  selector: 'app-resident-list',
  imports: [TranslatePipe],
  template: `<h1>{{ 'nav.residents' | translate }}</h1>`,
})
export class ResidentList {}
