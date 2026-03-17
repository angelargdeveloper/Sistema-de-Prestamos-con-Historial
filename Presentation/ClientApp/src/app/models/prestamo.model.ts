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
  fechaPrimerPago?: string;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  saldo: number;
  fechaFinalPago?: string;
  autorPersonaQueAutorizaId: number;
  nombreAutorizador: string;
  notas?: string;
  activo: boolean;
  fechaCreacion: string;
  fechaModificacion: string;
  usuarioCreacion?: string;
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
  fechaPrimerPago?: string;
  fechaFinalPago?: string;
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
  fechaPrimerPago?: string;
  totalAbonadoCapital: number;
  totalAbonadoIntereses: number;
  fechaFinalPago?: string;
  autorPersonaQueAutorizaId: number;
  notas?: string;
  usuarioModificacion: string;
}