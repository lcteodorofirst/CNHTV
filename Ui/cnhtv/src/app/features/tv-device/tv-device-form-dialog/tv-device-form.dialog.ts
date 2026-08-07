import { Component, Inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TvDeviceEndpoint } from '../../../domain/endpoints/tv-device.endpoint';
import { TvDevice } from '../../../domain/models/tv-device.model';

@Component({
  selector: 'app-tv-device-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
  ],
  templateUrl: './tv-device-form.dialog.html',
  styleUrl: './tv-device-form.dialog.scss',
})
export class TvDeviceFormDialog {
  public readonly form;
  public saving = false;

  constructor(
    fb: FormBuilder,
    private readonly endpoint: TvDeviceEndpoint,
    private readonly snackBar: MatSnackBar,
    private readonly dialogRef: MatDialogRef<TvDeviceFormDialog>,
    @Inject(MAT_DIALOG_DATA) public readonly device: TvDevice,
  ) {
    this.form = fb.nonNullable.group({
      Name: [device.Name, [Validators.required, Validators.maxLength(100)]],
      PresentationUrl: [device.PresentationUrl, [Validators.required, Validators.pattern(/^https?:\/\/.+/i)]],
      RefreshEnabled: [device.RefreshEnabled],
      RefreshIntervalMinutes: [
        device.RefreshIntervalSeconds / 60,
        [Validators.required, Validators.min(0.5), Validators.max(1440)],
      ],
    });
  }

  public async submit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving = true;
    const value = this.form.getRawValue();
    try {
      await this.endpoint.update(this.device.Id, {
        Name: value.Name.trim(),
        PresentationUrl: value.PresentationUrl.trim(),
        RefreshEnabled: value.RefreshEnabled,
        RefreshIntervalSeconds: Math.round(value.RefreshIntervalMinutes * 60),
      });
      this.snackBar.open('Configuração salva. A TV receberá a alteração no próximo heartbeat.', 'Fechar', {
        duration: 5000,
        panelClass: ['success-snackbar'],
      });
      this.dialogRef.close(true);
    } catch {
      this.snackBar.open('Não foi possível salvar a configuração.', 'Fechar', {
        duration: 5000,
        panelClass: ['error-snackbar'],
      });
    } finally {
      this.saving = false;
    }
  }
}
