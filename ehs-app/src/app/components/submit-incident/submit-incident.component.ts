import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IncidentService } from '../../services/incident.service'
 
@Component({
  selector: 'app-submit-incident',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './submit-incident.component.html'
})
export class SubmitIncidentComponent {
  form: FormGroup;
  submitting = false;
  errorMessage: string | null = null;
 
  constructor(
    private fb: FormBuilder,
    private incidentService: IncidentService,
    private router: Router
  ) {
    // Reactive Forms: the form's shape and validation rules are defined
    // here in the component class, not scattered across the template.
    // This mirrors the backend's [Required]/[StringLength]/[Range] attributes
    // on CreateIncidentDto -- same validation, enforced on both ends.
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      location: ['', [Validators.required, Validators.maxLength(200)]],
      severityLevel: [1, [Validators.required, Validators.min(1), Validators.max(5)]]
    });
  }
 
  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched(); // forces validation messages to show
      return;
    }
 
    this.submitting = true;
    this.errorMessage = null;
 
    this.incidentService.submit(this.form.value).subscribe({
      next: (incident) => {
        this.submitting = false;
        this.router.navigate(['/incidents', incident.id]);
      },
      error: (err) => {
        this.submitting = false;
        this.errorMessage = 'Failed to submit incident. Please try again.';
        console.error(err);
      }
    });
  }
}
 