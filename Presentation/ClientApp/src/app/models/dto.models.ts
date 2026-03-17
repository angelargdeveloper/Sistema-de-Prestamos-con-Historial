// DTOs para la comunicación con la API

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
  tipoPago: string;
  descripcionTipoPago: string;
  fechaPrimerPago: Date;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  saldo: number;
  fechaFinalPago: Date;
  autorPersonaQueAutorizaId: number;
  nombreAutorizador: string;
  notas?: string;
  activo: boolean;
  fechaCreacion: Date;
  fechaModificacion?: Date;
  usuarioCreacion: string;
  usuarioModificacion?: string;
  porcentajePagado: number;
  estaPagado: boolean;
  estadoPago: string;
  fechaPrimerPagoFormateada: string;
  fechaFinalPagoFormateada: string;
}

export interface CreatePrestamoDto {
  empleadoId: number;
  cantidadTotalPrestada: number;
  cantidadTotalAPagar: number;
  interesAprobado: number;
  interesMoratorio: number;
  tipoPagoAbonoId: number;
  fechaPrimerPago?: Date;
  fechaFinalPago?: Date;
  autorPersonaQueAutorizaId: number;
  notas?: string;
  usuarioCreacion: string;
}

export interface UpdatePrestamoDto {
  idPrestamo: number;
  empleadoId: number;
  cantidadTotalPrestada: number;
  cantidadTotalAPagar: number;
  interesAprobado: number;
  interesMoratorio: number;
  tipoPagoAbonoId: number;
  fechaPrimerPago?: Date;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  fechaFinalPago?: Date;
  autorPersonaQueAutorizaId: number;
  notas?: string;
  usuarioModificacion: string;
}

export interface PrestamoHistorialDto {
  id: number;
  prestamoId: number;
  nombreEmpleado: string;
  cantidadTotalPrestada: number;
  cantidadTotalAPagar: number;
  interesAprobado: number;
  interesMoratorio: number;
  tipoPago: string;
  fechaPrimerPago?: Date;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  saldo: number;
  fechaFinalPago?: Date;
  nombreAutorizador: string;
  notas?: string;
  activo: boolean;
  tipoOperacion: string; // CREATE, UPDATE, DELETE
  fechaOperacion: Date;
  usuarioOperacion?: string;
  fechaOperacionFormateada: string;
  fechaPrimerPagoFormateada: string;
  fechaFinalPagoFormateada: string;
}

export interface EmpleadoDto {
  id: number;
  numNomina: string;
  nombres: string;
  apellido1: string;
  apellido2?: string;
  nombreCompleto: string;
  activo: boolean;
}

export interface EmpleadoAutorizadorDto {
  id: number;
  numNomina: string;
  nombreCompleto: string;
  // Fallback properties para compatibilidad
  empleadoAutorizadorId?: number;
  nombre?: string;
  apellidos?: string;
  activo?: boolean;
}

export interface TipoPagoAbonoDto {
  id: number;
  nombreCorto: string;
  descripcion: string;
  // Fallback properties para compatibilidad
  tipoPagoAbonoId?: number;
  activo?: boolean;
}

export interface CatalogosDto {
  empleados: EmpleadoDto[];
  empleadosAutorizadores: EmpleadoAutorizadorDto[];
  tiposPagoAbono: TipoPagoAbonoDto[];
}

// Interfaces para respuestas de API
export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  data: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface PagedResultDto<T> {
  items: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}