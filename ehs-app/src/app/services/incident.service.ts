import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Incident,
  CreateIncidentRequest,
  AssignInvestigatorRequest,
  CompleteInvestigationRequest,
  ApprovalDecisionRequest
} from '../models/Incident.model';
 
@Injectable({ providedIn: 'root' }) // singleton, available app-wide without registering it anywhere
export class IncidentService {
  private readonly baseUrl = `${environment.apiUrl}/incidents`;
 
  constructor(private http: HttpClient) {}
 
  // GET /api/incidents
  getAll(): Observable<Incident[]> {
    return this.http.get<Incident[]>(this.baseUrl);
  }
 
  // POST /api/incidents
  submit(request: CreateIncidentRequest): Observable<Incident> {
    return this.http.post<Incident>(this.baseUrl, request);
  }
 
  // PUT /api/incidents/{id}/assign
  assignInvestigator(id: number, request: AssignInvestigatorRequest): Observable<Incident> {
    return this.http.put<Incident>(`${this.baseUrl}/${id}/assign`, request);
  }
 
  // PUT /api/incidents/{id}/complete-investigation
  completeInvestigation(id: number, request: CompleteInvestigationRequest): Observable<Incident> {
    return this.http.put<Incident>(`${this.baseUrl}/${id}/complete-investigation`, request);
  }
 
  // PUT /api/incidents/{id}/approval
  decideApproval(id: number, request: ApprovalDecisionRequest): Observable<Incident> {
    return this.http.put<Incident>(`${this.baseUrl}/${id}/approval`, request);
  }
}