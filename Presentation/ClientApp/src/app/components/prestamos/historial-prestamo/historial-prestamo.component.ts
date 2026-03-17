import { Component, OnInit, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrestamosService } from '../../../services/prestamos.service';
import { PrestamoHistorialDto } from '../../../models/dto.models';

@Component({
  selector: 'app-historial-prestamo',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './historial-prestamo.component.html',
  styleUrls: ['./historial-prestamo.component.scss']
})
export class HistorialPrestamoComponent implements OnInit {
  @Input() prestamoId!: number;
  @Input() nombreEmpleado: string = '';
  
  historial: PrestamoHistorialDto[] = [];
  loading = false;
  error = '';
  modalVisible = false;

  constructor(private prestamosService: PrestamosService) { }

  ngOnInit(): void {
    if (this.prestamoId) {
      this.loadHistorial();
    }
  }

  private loadHistorial(): void {
    this.loading = true;
    this.error = '';
    
    this.prestamosService.getHistorialPrestamo(this.prestamoId).subscribe({
      next: (historial) => {
        this.historial = historial;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error cargando historial:', error);
        this.error = 'Error al cargar el historial del préstamo';
        this.loading = false;
      }
    });
  }

  // Método público para mostrar el modal y cargar el historial
  mostrarHistorial(prestamoId: number, nombreEmpleado: string): void {
    this.prestamoId = prestamoId;
    this.nombreEmpleado = nombreEmpleado;
    this.modalVisible = true;
    this.loadHistorial();
  }

  // Método para cerrar el modal
  cerrarModal(): void {
    this.modalVisible = false;
    this.historial = [];
    this.error = '';
  }

  getTipoOperacionBadge(tipoOperacion: string): string {
    switch (tipoOperacion) {
      case 'CREATE':
        return 'success';
      case 'UPDATE':
        return 'warning';
      case 'DELETE':
        return 'danger';
      default:
        return 'secondary';
    }
  }

  getTipoOperacionTexto(tipoOperacion: string): string {
    switch (tipoOperacion) {
      case 'CREATE':
        return 'Creado';
      case 'UPDATE':
        return 'Actualizado';
      case 'DELETE':
        return 'Eliminado';
      default:
        return tipoOperacion;
    }
  }
}