import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, of, map } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PrestamoDto {
  idPrestamo: number;
  empleadoId: number;
  nombreEmpleado: string;
  numNomina: string;
  cantidadTotalPrestada: number;
  cantidadTotalAPagar: number;
  interesAprobado: number;
  interesMoratorio: number;
  tipoPagoAbonoId: number;
  tipoPagoAbono: string;
  fechaCreacion: string;
  fechaVencimiento: string;
  fechaUltimaActualizacion: string;
  activo: boolean;
  estado: string;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  saldoPendiente: number;
  porcentajePagado: number;
  empleadoAutorizadorId?: number;
  empleadoAutorizador?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({
  providedIn: 'root'
})
export class PrestamoService {
  private apiUrl = `${environment.apiUrl}/prestamos`;

  constructor(private http: HttpClient) {}

  getPrestamos(pageNumber: number = 1, pageSize: number = 20, searchTerm: string = ''): Observable<PagedResult<PrestamoDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<PagedResult<PrestamoDto>>(this.apiUrl, { params })
      .pipe(
        catchError(error => {
          console.error('Error al obtener préstamos:', error);
          // Retornar datos de demostración en caso de error
          return of(this.getMockData(pageNumber, pageSize));
        })
      );
  }

  getPrestamoById(id: number): Observable<PrestamoDto | null> {
    return this.http.get<PrestamoDto>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(error => {
          console.error('Error al obtener préstamo:', error);
          return of(null);
        })
      );
  }

  createPrestamo(prestamo: Partial<PrestamoDto>): Observable<PrestamoDto | null> {
    return this.http.post<PrestamoDto>(this.apiUrl, prestamo)
      .pipe(
        catchError(error => {
          console.error('Error al crear préstamo:', error);
          return of(null);
        })
      );
  }

  updatePrestamo(id: number, prestamo: Partial<PrestamoDto>): Observable<PrestamoDto | null> {
    return this.http.put<PrestamoDto>(`${this.apiUrl}/${id}`, prestamo)
      .pipe(
        catchError(error => {
          console.error('Error al actualizar préstamo:', error);
          return of(null);
        })
      );
  }

  deletePrestamo(id: number): Observable<boolean> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        map(() => true),
        catchError(error => {
          console.error('Error al eliminar préstamo:', error);
          return of(false);
        })
      );
  }

  private getMockData(pageNumber: number, pageSize: number): PagedResult<PrestamoDto> {
    const mockPrestamos: PrestamoDto[] = [
      {
        idPrestamo: 1,
        empleadoId: 1,
        nombreEmpleado: 'Diana Elizabeth Cruz Morales',
        numNomina: 'EMP006',
        cantidadTotalPrestada: 50000.00,
        cantidadTotalAPagar: 55000.00,
        interesAprobado: 10.0,
        interesMoratorio: 2.0,
        tipoPagoAbonoId: 1,
        tipoPagoAbono: 'MENSUAL',
        fechaCreacion: '2026-03-16T00:00:00',
        fechaVencimiento: '2027-03-16T00:00:00',
        fechaUltimaActualizacion: '2026-03-16T00:00:00',
        activo: true,
        estado: 'Pendiente',
        totalAbonadoCapital: 0,
        totalAbonadoIntereses: 0,
        saldoPendiente: 55000.00,
        porcentajePagado: 0,
        empleadoAutorizadorId: 2,
        empleadoAutorizador: 'Supervisor Admin'
      },
      {
        idPrestamo: 2,
        empleadoId: 2,
        nombreEmpleado: 'Roberto Miguel Vargas Castillo',
        numNomina: 'EMP007',
        cantidadTotalPrestada: 25000.00,
        cantidadTotalAPagar: 26875.00,
        interesAprobado: 7.5,
        interesMoratorio: 1.5,
        tipoPagoAbonoId: 2,
        tipoPagoAbono: 'QUINCENAL',
        fechaCreacion: '2026-03-16T00:00:00',
        fechaVencimiento: '2027-03-16T00:00:00',
        fechaUltimaActualizacion: '2026-03-16T00:00:00',
        activo: true,
        estado: 'En progreso',
        totalAbonadoCapital: 5000,
        totalAbonadoIntereses: 1250,
        saldoPendiente: 20625.00,
        porcentajePagado: 25,
        empleadoAutorizadorId: 2,
        empleadoAutorizador: 'Supervisor Admin'
      }
    ];

    return {
      items: mockPrestamos,
      totalItems: mockPrestamos.length,
      pageNumber: pageNumber,
      pageSize: pageSize,
      totalPages: Math.ceil(mockPrestamos.length / pageSize)
    };
  }
}