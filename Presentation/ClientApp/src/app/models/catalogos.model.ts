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
}

export interface TipoPagoAbonoDto {
  id: number;
  nombreCorto: string;
  descripcion: string;
}