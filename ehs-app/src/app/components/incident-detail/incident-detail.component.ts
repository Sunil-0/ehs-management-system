import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IncidentService } from '../../services/incident.service';
import { AuthService } from '../../services/auth.service';
import { Incident, IncidentStatus, IncidentStatusLabels } from '../../models/Incident.model';

@Component({
  selector: 'app-incident-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './incident-detail.component.html'
})
export class IncidentDetailComponent implements OnInit {
  incident: Incident | null = null;
  statusLabels = IncidentStatusLabels;
  IncidentStatus = IncidentStatus; // exposed so the template can compare against it directly

  assignForm: FormGroup;
  investigationForm: FormGroup;
  actionError: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private incidentService: IncidentService,
    public authService: AuthService, // public: template reads authService.hasRole(...) directly
    private fb: FormBuilder
  ) {
    this.assignForm = this.fb.group({
      investigatorId: [null, Validators.required]
    });

    this.investigationForm = this.fb.group({
      findings: ['', Validators.required],
      rootCause: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadIncident();
  }

  private loadIncident(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.incidentService.getAll().subscribe((incidents) => {
      this.incident = incidents.find((i) => i.id === id) ?? null;
    });
  }

  assignInvestigator(): void {
    if (!this.incident || this.assignForm.invalid) return;

    this.incidentService.assignInvestigator(this.incident.id, this.assignForm.value).subscribe({
      next: (updated) => (this.incident = updated),
      error: (err) => this.handleError(err)
    });
  }

  completeInvestigation(): void {
    if (!this.incident || this.investigationForm.invalid) return;

    this.incidentService.completeInvestigation(this.incident.id, this.investigationForm.value).subscribe({
      next: (updated) => (this.incident = updated),
      error: (err) => this.handleError(err)
    });
  }

  approve(approved: boolean): void {
    if (!this.incident) return;

    this.incidentService.decideApproval(this.incident.id, { approved }).subscribe({
      next: (updated) => (this.incident = updated),
      error: (err) => this.handleError(err)
    });
  }

  private handleError(err: any): void {
    // The backend returns 409 Conflict with a plain-text message for
    // illegal workflow transitions -- surface that directly to the user.
    this.actionError = err.status === 409 ? err.error : 'Something went wrong.';
  }
}