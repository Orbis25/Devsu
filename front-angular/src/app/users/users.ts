import { Component, OnInit } from '@angular/core';
import { IPaginationResult } from '../../shared/interfaces/pagination.interface';
import { GetUser } from './interfaces/get-user';
import { CreateOrUpdateUser } from './interfaces/create-or-update-user';
import { UserService } from './services/user.service';
import { Column, TableComponent } from '../shared/table/table';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
 

const formEmpty: CreateOrUpdateUser = {
  name: "",
  gender: "hombre",
  age: 0,
  identification: "",
  phone: "",
  address: "",
  clientId: "",
  password: "",
};

@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit {
  showForm: boolean = false;
  data: IPaginationResult<GetUser> | null = null;
  isLoading: boolean = false;
  editId: string | null = null;
  initialValues: CreateOrUpdateUser = formEmpty;
  searchQuery: string = '';
  userForm: FormGroup;

  columns: Column<GetUser>[] = [
    { key: 'name', label: 'Nombre' },
    { key: 'identification', label: 'Identificación' },
    { key: 'gender', label: 'Genero' },
    { key: 'age', label: 'Edad' },
    { key: 'phone', label: 'Teléfono' },
    { key: 'address', label: 'Dirección' },
    { key: 'clientId', label: 'ID Cliente' },
    {
      key: 'actions',
      label: 'Acciones',
      render: (value: unknown, row: GetUser) => `
       <div>
        <button class="btn-edit" data-action="edit" data-id="${row.id}">Editar</button>
        <button class="btn-delete" data-action="delete" data-id="${row.id}">Eliminar</button>
       </div>
      `,
    },
  ];

  constructor(private userService: UserService, private fb: FormBuilder) {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      gender: ['hombre', Validators.required],
      age: ['', [Validators.required, Validators.min(18), Validators.pattern(/^[0-9]*$/)]],
      identification: ['', [Validators.required, Validators.pattern(/^[0-9]*$/)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9]*$/)]],
      address: ['', Validators.required],
      clientId: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.initialize();
  }

  initialize(): void {
    this.getAll();
  }

  getAll(q: string = this.searchQuery, page: number = 1): void {
    this.isLoading = true;
    try {
      this.userService
        .getAllPaginated<GetUser>({
          page: page,
          query: q
        })
        .subscribe({
          next: (result) => {
            this.data = result.data;
          },
          error: (error) => {
            console.error('Error:' + error);
          },
        });
    } catch (error) {
      console.error('Error:' + error);
    } finally {
      this.isLoading = false;
    }
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    let serviceCall;
    let msg = 'Creando con éxito';

    if (this.editId) {
      serviceCall = this.userService.edit(this.editId, this.userForm.value);
      msg = 'Actualizado con éxito';
    } else {
      serviceCall = this.userService.post<CreateOrUpdateUser, any>(
        this.userForm.value
      );
    }

    serviceCall.subscribe({
      next: () => {
        alert(msg);
        this.getAll();
        this.editId = null;
        this.initialValues = { ...formEmpty };
        this.onCloseModal();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  handleChangeSearch(event: Event): void {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.getAll(this.searchQuery);
  }

  handleActionEdit(row: GetUser): void {
    this.editId = row.id;
    this.initialValues = { ...row };
    this.userForm.patchValue(row);
    this.showForm = true;
  }

  handleClickDelete(id: string): void {
    if (!id) return;

    const result = confirm('¿Está seguro de que desea eliminar este cliente?');
    if (!result) return;

    this.userService.remove(id).subscribe({
      next: (_) => {
        alert('Cliente eliminado con éxito');
        this.getAll();
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  handleChangePage(page: number): void {
    this.getAll(this.searchQuery, page);
  }

  handleTableAction(event: { action: string; row: GetUser }): void {
    if (event.action === 'edit') {
      this.handleActionEdit(event.row);
    } else if (event.action === 'delete') {
      this.handleClickDelete(event.row.id);
    }
  }

  openModal(): void {
    this.editId = null;
    this.initialValues = { ...formEmpty };
    this.userForm.reset(formEmpty);
    this.showForm = true;
  }

  onCloseModal(): void {
    this.showForm = false;
    this.userForm.reset(formEmpty);
  }
}
