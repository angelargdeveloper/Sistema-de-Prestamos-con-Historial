import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { EmpleadoDto, EmpleadoAutorizadorDto, TipoPagoAbonoDto, CatalogosDto, PagedResultDto } from '../models/dto.models';

// Environment configuration inline
const environment = {
  apiUrl: 'https://localhost:7178'
};

@Injectable({
  providedIn: 'root'
})
export class CatalogosService {
  private apiUrl = `${environment.apiUrl}/api/catalogos`;

  constructor(private http: HttpClient) { }

  getEmpleados(): Observable<EmpleadoDto[]> {
    const url = `${this.apiUrl}/empleados?pageSize=1000`;
    return this.http.get<PagedResultDto<EmpleadoDto>>(url)
      .pipe(
        map(response => response.items || [])
      );
  }

  getAutorizadores(): Observable<EmpleadoAutorizadorDto[]> {
    return this.http.get<EmpleadoAutorizadorDto[]>(`${this.apiUrl}/empleados-autorizadores`);
  }

  getTiposPagoAbono(): Observable<TipoPagoAbonoDto[]> {
    return this.http.get<TipoPagoAbonoDto[]>(`${this.apiUrl}/tipos-pago`);
  }

  getCatalogos(): Observable<CatalogosDto> {
    return this.http.get<CatalogosDto>(`${this.apiUrl}/todos`);
  }
}