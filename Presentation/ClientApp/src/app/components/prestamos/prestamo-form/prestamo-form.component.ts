import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { PrestamosService } from '../../../services/prestamos.service';
import { CatalogosService } from '../../../services/catalogos.service';
import { CreatePrestamoDto, UpdatePrestamoDto, PrestamoDto, EmpleadoDto, EmpleadoAutorizadorDto, TipoPagoAbonoDto } from '../../../models/dto.models';

@Component({
  selector: 'app-prestamo-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './prestamo-form.component.html',
  styleUrls: ['./prestamo-form.component.scss']
})
export class PrestamoFormComponent implements OnInit {
  prestamoForm: FormGroup;
  isEditMode = false;
  prestamoId: number | null = null;
  loading = false;
  error = '';
  
  // Catálogos
  empleados: EmpleadoDto[] = [];
  tiposPagoAbono: TipoPagoAbonoDto[] = [];
  autorizadores: EmpleadoAutorizadorDto[] = [];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private prestamosService: PrestamosService,
    private catalogosService: CatalogosService
  ) {
    this.prestamoForm = this.createForm();
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.prestamoId = +params['id'];
        this.isEditMode = true;
        this.loadPrestamo();
      }
    });
    
    this.loadCatalogos();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      empleadoId: [null, Validators.required],
      fechaPrestamo: [new Date().toISOString().split('T')[0], Validators.required],
      cantidadTotalPrestada: [null, [Validators.required, Validators.min(0.01)]],
      cantidadTotalAPagar: [null, [Validators.required, Validators.min(0)]],
      interesAprobado: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      interesMoratorio: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      empleadoAutorizadorId: [null],
      autorPersonaQueAutorizaId: [null, Validators.required],
      tipoPagoAbonoId: [null, Validators.required],
      fechaPrimerPago: [null],
      fechaFinalPago: [null],
      diaPago: [null, [Validators.min(1), Validators.max(31)]],
      montoAbono: [null, [Validators.min(0)]],
      observaciones: [''],
      notas: [''],
      activo: [true]
    });
  }

  private loadCatalogos(): void {
    // Cargar empleados
    this.catalogosService.getEmpleados().subscribe({
      next: (empleados) => {
        this.empleados = empleados;
      },
      error: (error) => {
        console.error('Error cargando empleados:', error);
      }
    });

    // Cargar tipos de pago
    this.catalogosService.getTiposPagoAbono().subscribe({
      next: (tipos) => {
        this.tiposPagoAbono = tipos;
      },
      error: (error) => console.error('Error cargando tipos de pago:', error)
    });

    // Cargar autorizadores
    this.catalogosService.getAutorizadores().subscribe({
      next: (autorizadores) => {
        this.autorizadores = autorizadores;
      },
      error: (error) => console.error('Error cargando autorizadores:', error)
    });
  }

  private loadPrestamo(): void {
    if (!this.prestamoId) return;

    this.loading = true;
    this.prestamosService.getPrestamo(this.prestamoId).subscribe({
      next: (prestamo) => {
        this.prestamoForm.patchValue({
          empleadoId: prestamo.empleadoId,
          cantidadTotalPrestada: prestamo.cantidadTotalPrestada,
          cantidadTotalAPagar: prestamo.cantidadTotalAPagar,
          interesAprobado: prestamo.interesAprobado,
          interesMoratorio: prestamo.interesMoratorio,
          autorPersonaQueAutorizaId: prestamo.autorPersonaQueAutorizaId,
          tipoPagoAbonoId: prestamo.tipoPagoAbonoId,
          fechaPrimerPago: prestamo.fechaPrimerPago ? new Date(prestamo.fechaPrimerPago).toISOString().split('T')[0] : null,
          fechaFinalPago: prestamo.fechaFinalPago ? new Date(prestamo.fechaFinalPago).toISOString().split('T')[0] : null,
          notas: prestamo.notas,
          activo: prestamo.activo
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error cargando préstamo:', error);
        this.error = 'Error al cargar el préstamo';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.prestamoForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    const formValue = this.prestamoForm.value;

    if (this.isEditMode && this.prestamoId) {
      this.updatePrestamo(formValue);
    } else {
      this.createPrestamo(formValue);
    }
  }

  private createPrestamo(formValue: any): void {
    const createDto: CreatePrestamoDto = {
      empleadoId: formValue.empleadoId,
      cantidadTotalPrestada: formValue.cantidadTotalPrestada,
      cantidadTotalAPagar: formValue.cantidadTotalAPagar,
      interesAprobado: formValue.interesAprobado,
      interesMoratorio: formValue.interesMoratorio,
      tipoPagoAbonoId: formValue.tipoPagoAbonoId,
      fechaPrimerPago: formValue.fechaPrimerPago ? new Date(formValue.fechaPrimerPago) : undefined,
      fechaFinalPago: formValue.fechaFinalPago ? new Date(formValue.fechaFinalPago) : undefined,
      autorPersonaQueAutorizaId: formValue.autorPersonaQueAutorizaId,
      notas: formValue.notas || formValue.observaciones,
      usuarioCreacion: 'Sistema' // Por ahora hardcodeado, después puede venir del usuario logueado
    };

    this.prestamosService.createPrestamo(createDto).subscribe({
      next: (response) => {
        alert('Préstamo creado exitosamente');
        this.router.navigate(['/prestamos']);
      },
      error: (error) => {
        console.error('Error creando préstamo:', error);
        this.error = 'Error al crear el préstamo';
        this.loading = false;
      }
    });
  }

  private updatePrestamo(formValue: any): void {
    const updateDto: UpdatePrestamoDto = {
      idPrestamo: this.prestamoId!,
      empleadoId: formValue.empleadoId,
      cantidadTotalPrestada: formValue.cantidadTotalPrestada,
      cantidadTotalAPagar: formValue.cantidadTotalAPagar,
      interesAprobado: formValue.interesAprobado,
      interesMoratorio: formValue.interesMoratorio,
      tipoPagoAbonoId: formValue.tipoPagoAbonoId,
      fechaPrimerPago: formValue.fechaPrimerPago ? new Date(formValue.fechaPrimerPago) : undefined,
      totalAbonadoCapital: 0, // Estos valores deben venir del préstamo existente si se edita
      totalAbonadoIntereses: 0,
      fechaFinalPago: formValue.fechaFinalPago ? new Date(formValue.fechaFinalPago) : undefined,
      autorPersonaQueAutorizaId: formValue.autorPersonaQueAutorizaId,
      notas: formValue.notas || formValue.observaciones,
      usuarioModificacion: 'Sistema' // Por ahora hardcodeado
    };

    this.prestamosService.updatePrestamo(this.prestamoId!, updateDto).subscribe({
      next: (response) => {
        alert('Préstamo actualizado exitosamente');
        this.router.navigate(['/prestamos']);
      },
      error: (error) => {
        console.error('Error actualizando préstamo:', error);
        this.error = 'Error al actualizar el préstamo';
        this.loading = false;
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.prestamoForm.controls).forEach(key => {
      const control = this.prestamoForm.get(key);
      control?.markAsTouched();
    });
  }

  onCancel(): void {
    this.router.navigate(['/prestamos']);
  }

  // Validaciones y utilidades
  isFieldInvalid(fieldName: string): boolean {
    const field = this.prestamoForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.prestamoForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) return 'Este campo es obligatorio';
      if (field.errors['min']) return `El valor mínimo es ${field.errors['min'].min}`;
      if (field.errors['max']) return `El valor máximo es ${field.errors['max'].max}`;
    }
    return '';
  }

  // Cálculos automáticos
  calcularInteres(): void {
    const montoPrestamo = this.prestamoForm.get('cantidadTotalPrestada')?.value;
    const porcentajeInteres = this.prestamoForm.get('interesAprobado')?.value;
    
    if (montoPrestamo && porcentajeInteres) {
      // Calcular el monto total con intereses
      const interes = montoPrestamo * (porcentajeInteres / 100);
      const montoTotal = montoPrestamo + interes;
      
      // Actualizar el total a pagar
      this.prestamoForm.patchValue({ 
        cantidadTotalAPagar: Number(montoTotal.toFixed(2))
      });
    }
  }

  onMontoPrestamoChange(): void {
    const montoPrestamo = this.prestamoForm.get('montoPrestamo')?.value;
    if (montoPrestamo && !this.isEditMode) {
      // Al crear un nuevo préstamo, el saldo actual es igual al monto prestado inicialmente
      this.prestamoForm.patchValue({ saldoActual: montoPrestamo });
    }
  }

  calculateMontoAbono(): void {
    const saldoActual = this.prestamoForm.get('saldoActual')?.value;
    const diaPago = this.prestamoForm.get('diaPago')?.value;
    
    if (saldoActual && diaPago) {
      // Cálculo simple: dividir el saldo entre los días restantes del mes
      const montoSugerido = Math.ceil(saldoActual / (30 - diaPago + 1));
      this.prestamoForm.patchValue({ montoAbono: montoSugerido });
    }
  }
}