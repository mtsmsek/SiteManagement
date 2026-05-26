import { TestBed } from '@angular/core/testing';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { TranslateModule } from '@ngx-translate/core';
import { ErrorSnackbar } from './error-snackbar';

describe('ErrorSnackbar', () => {
  const ref = { dismiss: vi.fn() };

  beforeEach(async () => {
    // arrange — the component opened with a declined-payment message
    await TestBed.configureTestingModule({
      imports: [ErrorSnackbar, TranslateModule.forRoot()],
      providers: [
        { provide: MAT_SNACK_BAR_DATA, useValue: { message: 'Ödeme reddedildi.' } },
        { provide: MatSnackBarRef, useValue: ref },
      ],
    }).compileComponents();
  });

  it('renders the error message with a warning icon', () => {
    // act
    const fixture = TestBed.createComponent(ErrorSnackbar);
    fixture.detectChanges();

    // assert
    const host = fixture.nativeElement as HTMLElement;
    expect(host.textContent).toContain('Ödeme reddedildi.');
    expect(host.querySelector('mat-icon')).toBeTruthy();
  });

  it('dismisses itself when the close action is clicked', () => {
    // arrange
    const fixture = TestBed.createComponent(ErrorSnackbar);
    fixture.detectChanges();

    // act
    fixture.componentInstance.dismiss();

    // assert
    expect(ref.dismiss).toHaveBeenCalledOnce();
  });
});
