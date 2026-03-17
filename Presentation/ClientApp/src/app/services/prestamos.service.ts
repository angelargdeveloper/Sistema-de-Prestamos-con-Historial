import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { PrestamoDto, CreatePrestamoDto, UpdatePrestamoDto, PrestamoHistorialDto, ApiResponse, PaginatedResponse } from '../models/dto.models';

// Environment configuration inline
const environment = {
  apiUrl: 'https://localhost:7178'
};

@Injectable({
  providedIn: 'root'
})
export class PrestamosService {
  private apiUrl = `${environment.apiUrl}/api/prestamos`;

  constructor(private http: HttpClient) { }

  // Obtener todos los préstamos con paginación
  getPrestamos(pageNumber: number = 1, pageSize: number = 10, busqueda?: string): Observable<PaginatedResponse<PrestamoDto>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (busqueda) {
      params = params.set('busqueda', busqueda);
    }

    console.log('PrestamosService: Enviando petición a:', this.apiUrl);
    console.log('PrestamosService: Con parámetros:', params.toString());

    return this.http.get<any>(this.apiUrl, { params }).pipe(
      map(response => ({
        data: Array.isArray(response.items) ? response.items : [],
        totalRecords: response.totalItems || 0,
        pageNumber: response.pageNumber || pageNumber,
        pageSize: response.pageSize || pageSize,
        totalPages: response.totalPages || 0
      })),
      catchError(this.handleError<PaginatedResponse<PrestamoDto>>('getPrestamos', {
        data: [],
        totalRecords: 0,
        pageNumber: pageNumber,
        pageSize: pageSize,
        totalPages: 0
      }))
    );
  }

  // Obtener un préstamo por ID
  getPrestamo(id: number): Observable<PrestamoDto> {
    return this.http.get<PrestamoDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError<PrestamoDto>(`getPrestamo id=${id}`))
    );
  }

  // Crear un nuevo préstamo
  createPrestamo(prestamo: CreatePrestamoDto): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, prestamo).pipe(
      catchError(this.handleError<ApiResponse<number>>('createPrestamo'))
    );
  }

  // Actualizar un préstamo
  updatePrestamo(id: number, prestamo: UpdatePrestamoDto): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/${id}`, prestamo).pipe(
      catchError(this.handleError<ApiResponse<boolean>>(`updatePrestamo id=${id}`))
    );
  }

  // Eliminar un préstamo
  deletePrestamo(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError<ApiResponse<boolean>>(`deletePrestamo id=${id}`))
    );
  }

  // Obtener historial de cambios de un préstamo
  getHistorialPrestamo(id: number): Observable<PrestamoHistorialDto[]> {
    return this.http.get<PrestamoHistorialDto[]>(`${this.apiUrl}/${id}/historial`).pipe(
      catchError(this.handleError<PrestamoHistorialDto[]>('getHistorialPrestamo', []))
    );
  }

  // Manejo de errores
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed:`, error);
      console.error('Error details:', error.error);
      console.error('Status:', error.status);
      console.error('Status Text:', error.statusText);
      
      // Si hay un result por defecto, lo retornamos
      if (result !== undefined) {
        return of(result as T);
      }
      
      // De lo contrario, propagamos el error
      return throwError(() => error);
    };
  }
}