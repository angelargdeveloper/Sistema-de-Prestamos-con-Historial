import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { PrestamosService } from '../../../services/prestamos.service';
import { PrestamoDto } from '../../..//models/dto.models';

@Component({
  selector: 'app-prestamo-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, DatePipe, CurrencyPipe],
  template: `<div>Prestamo Detail Component</div>` // Template temporal
})
export class PrestamoDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private prestamosService = inject(PrestamosService);

  prestamo: PrestamoDto | null = null;
  prestamoId: number = 0;
  loading = false;
  error: string | null = null;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.prestamoId = +params['id'];
      if (this.prestamoId) {
        this.loadPrestamo();
      } else {
        this.error = 'ID de préstamo inválido';
      }
    });
  }

  private loadPrestamo(): void {
    this.loading = true;
    this.error = null;
    
    this.prestamosService.getPrestamo(this.prestamoId).subscribe({
      next: (data) => {
        this.prestamo = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error al cargar el préstamo:', error);
        this.error = 'No se pudo cargar la información del préstamo';
        this.loading = false;
      }
    });
  }

  getEstadoBadgeClass(activo: boolean): string {
    return activo ? 'badge bg-success' : 'badge bg-secondary';
  }

  getProgressPercentage(): number {
    if (!this.prestamo) return 0;
    return this.prestamo.porcentajePagado;
  }

  getProgressBarClass(): string {
    const progress = this.getProgressPercentage();
    if (progress < 25) return 'bg-danger';
    if (progress < 50) return 'bg-warning';
    if (progress < 75) return 'bg-info';
    return 'bg-success';
  }

  deletePrestamo(): void {
    if (!this.prestamo) return;

    const confirmMessage = `¿Está seguro de eliminar el préstamo #${this.prestamo.idPrestamo} de ${this.prestamo.nombreEmpleado}?`;
    
    if (confirm(confirmMessage)) {
      this.loading = true;
      
      this.prestamosService.deletePrestamo(this.prestamo.idPrestamo).subscribe({
        next: () => {
          alert('Préstamo eliminado exitosamente');
          this.router.navigate(['/prestamos']);
        },
        error: (error) => {
          console.error('Error al eliminar el préstamo:', error);
          alert('No se pudo eliminar el préstamo');
          this.loading = false;
        }
      });
    }
  }
}