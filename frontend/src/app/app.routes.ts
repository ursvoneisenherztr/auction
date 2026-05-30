import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PropertiesListComponent } from './components/properties-list/properties-list.component';
import { PropertyDetailComponent } from './components/property-detail/property-detail.component';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'properties', component: PropertiesListComponent },
  { path: 'properties/:id', component: PropertyDetailComponent },
];
