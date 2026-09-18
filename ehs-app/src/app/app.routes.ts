import { Routes } from '@angular/router';
import { SubmitIncidentComponent } from './components/submit-incident/submit-incident.component';
import { IncidentListComponent } from './components/incident-list/incident-list.component';
import { IncidentDetailComponent } from './components/incident-detail/incident-detail.component';
import { LoginComponent } from './components/login/login.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'incidents', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'incidents', component: IncidentListComponent, canActivate: [authGuard] },
  { path: 'incidents/new', component: SubmitIncidentComponent, canActivate: [authGuard] },
  { path: 'incidents/:id', component: IncidentDetailComponent, canActivate: [authGuard] }
];