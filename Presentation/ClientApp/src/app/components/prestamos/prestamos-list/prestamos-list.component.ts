import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PrestamosService } from '../../../services/prestamos.service';
import { PrestamoDto, PaginatedResponse } from '../../../models/dto.models';
import { HistorialPrestamoComponent } from '../historial-prestamo/historial-prestamo.component';

@Component({
  selector: 'app-prestamos-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, HistorialPrestamoComponent],
  templateUrl: './prestamos-list.component.html',
  styleUrls: ['./prestamos-list.component.scss']
})
export class PrestamosListComponent implements OnInit {
  @ViewChild(HistorialPrestamoComponent) historialComponent!: HistorialPrestamoComponent;
  
  prestamos: PrestamoDto[] = [];
  pagedResult: PaginatedResponse<PrestamoDto> = {
    data: [],
    totalRecords: 0,
    pageNumber: 1,
    pageSize: 20,
    totalPages: 0
  };
  
  // Parámetros de búsqueda
  pageNumber: number = 1;
  pageSize: number = 20;
  busqueda: string = '';
  
  // Estados de la UI
  loading = false;
  error = '';
  
  // Configuración de la tabla
  sortColumn = '';
  sortDirection: 'asc' | 'desc' = 'asc';

  constructor(private prestamosService: PrestamosService) {}

  ngOnInit(): void {
    console.log('PrestamosListComponent initialized');
    this.loadPrestamos();
  }

  loadPrestamos(): void {
    console.log('loadPrestamos() called');
    this.loading = true;
    this.error = '';

    this.prestamosService.getPrestamos(this.pageNumber, this.pageSize, this.busqueda).subscribe({
      next: (result: PaginatedResponse<PrestamoDto>) => {
        console.log('PrestamosListComponent: Datos recibidos:', result);
        this.pagedResult = result;
        this.prestamos = Array.isArray(result.data) ? result.data : [];
        this.loading = false;
      },
      error: (error: any) => {
        console.error('PrestamosListComponent: Error al cargar préstamos:', error);
        this.error = 'Error al cargar los préstamos. Por favor, intente nuevamente.';
        this.loading = false;
        this.prestamos = [];
      }
    });
  }

  onSearch(): void {
    console.log('Búsqueda:', this.busqueda);
    this.pageNumber = 1; // Resetear a la primera página
    this.loadPrestamos();
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.pagedResult.totalPages) {
      this.pageNumber = page;
      this.loadPrestamos();
    }
  }

  onPageSizeChange(): void {
    this.pageNumber = 1; // Resetear a la primera página
    this.loadPrestamos();
  }

  sortBy(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    
    // Aplicar el ordenamiento localmente
    this.prestamos.sort((a, b) => {
      let aVal: any, bVal: any;
      
      switch (column) {
        case 'prestamoId':
          aVal = a.idPrestamo;
          bVal = b.idPrestamo;
          break;
        case 'empleadoNombre':
          aVal = a.nombreEmpleado || '';
          bVal = b.nombreEmpleado || '';
          break;
        case 'fechaPrestamo':
          aVal = new Date(a.fechaCreacion);
          bVal = new Date(b.fechaCreacion);
          break;
        case 'montoPrestamo':
          aVal = a.cantidadTotalPrestada;
          bVal = b.cantidadTotalPrestada;
          break;
        case 'saldoActual':
          aVal = a.saldo;
          bVal = b.saldo;
          break;
        default:
          return 0;
      }
      
      if (aVal < bVal) return this.sortDirection === 'asc' ? -1 : 1;
      if (aVal > bVal) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });
  }

  getSortIcon(column: string): string {
    if (this.sortColumn !== column) return 'fas fa-sort';
    return this.sortDirection === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
  }

  getStatusBadgeClass(activo: boolean): string {
    return activo ? 'badge bg-success' : 'badge bg-secondary';
  }

  getStatusText(activo: boolean): string {
    return activo ? 'Activo' : 'Inactivo';
  }

  deletePrestamo(prestamo: PrestamoDto): void {
    if (confirm(`¿Está seguro de eliminar el préstamo #${prestamo.idPrestamo} de ${prestamo.nombreEmpleado}?`)) {
      this.prestamosService.deletePrestamo(prestamo.idPrestamo).subscribe({
        next: () => {
          alert('Préstamo eliminado exitosamente');
          this.loadPrestamos(); // Recargar la lista
        },
        error: (error: any) => {
          console.error('Error al eliminar el préstamo:', error);
          alert('No se pudo eliminar el préstamo');
        }
      });
    }
  }

  verHistorial(prestamo: PrestamoDto): void {
    if (this.historialComponent) {
      this.historialComponent.mostrarHistorial(prestamo.idPrestamo, prestamo.nombreEmpleado || '');
    }
  }

  refresh(): void {
    this.loadPrestamos();
  }

  // Getters para la paginación
  get hasPreviousPage(): boolean {
    return this.pageNumber > 1;
  }

  get hasNextPage(): boolean {
    return this.pageNumber < this.pagedResult.totalPages;
  }

  get startItem(): number {
    return (this.pageNumber - 1) * this.pageSize + 1;
  }

  get endItem(): number {
    return Math.min(this.pageNumber * this.pageSize, this.pagedResult.totalRecords);
  }
}