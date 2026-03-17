import { Routes } from '@angular/router';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: '/prestamos', 
    pathMatch: 'full' 
  },
  {
    path: 'prestamos',
    loadComponent: () => import('./components/prestamos/prestamos-list/prestamos-list.component').then(m => m.PrestamosListComponent)
  },
  {
    path: 'prestamos/nuevo',
    loadComponent: () => import('./components/prestamos/prestamo-form/prestamo-form.component').then(m => m.PrestamoFormComponent)
  },
  {
    path: 'prestamos/editar/:id',
    loadComponent: () => import('./components/prestamos/prestamo-form/prestamo-form.component').then(m => m.PrestamoFormComponent)
  },
  {
    path: 'prestamos/ver/:id',
    loadComponent: () => import('./components/prestamos/prestamo-detail/prestamo-detail.component').then(m => m.PrestamoDetailComponent)
  },
  { 
    path: '**', 
    redirectTo: '/prestamos' 
  }
];