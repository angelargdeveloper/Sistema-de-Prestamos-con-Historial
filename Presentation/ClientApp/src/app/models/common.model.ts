export interface PagedResultDto<T> {
  items: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface SearchParametersDto {
  busqueda?: string;
  pageNumber: number;
  pageSize: number;
}

export interface ApiResponseDto<T> {
  success: boolean;
  message: string;
  data?: T;
  errors: string[];
}

export interface ValidationErrorDto {
  field: string;
  message: string;
}