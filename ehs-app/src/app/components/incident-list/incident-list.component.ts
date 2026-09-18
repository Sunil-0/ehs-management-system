import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { IncidentService } from '../../services/incident.service';
import { Incident, IncidentStatusLabels } from '../../models/Incident.model';
 
@Component({
  selector: 'app-incident-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './incident-list.component.html'
})
export class IncidentListComponent implements OnInit {
  // Kept as an Observable (not subscribed manually) so the template's
  // async pipe handles subscribe/unsubscribe automatically -- this avoids
  // a common source of memory leaks (forgetting to unsubscribe).
  incidents$!: Observable<Incident[]>;
  statusLabels = IncidentStatusLabels;
 
  constructor(private incidentService: IncidentService) {}
 
  ngOnInit(): void {
    this.incidents$ = this.incidentService.getAll();
  }
}