import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IPaginationResult } from '../../shared/interfaces/pagination.interface';
import { Column } from '../shared/table/table';
import { GetAccount } from './interfaces/get-account';
import { CreateOrUpdateAccount } from './interfaces/create-or-update-account';
import { AccountService } from './services/accounts.service';
import { UserService } from '../users/services/user.service';
import { GetUser } from '../users/interfaces/get-user';
import { firstValueFrom, Observable } from 'rxjs';

const formEmpty: CreateOrUpdateAccount = {
  accountNumber: '',
  accountType: 'ahorros',
  initialBalance: 0,
  userId: '',
};

@Component({
  selector: 'app-accounts',
  standalone: false,
  templateUrl: './accounts.html',
  styleUrl: './accounts.css',
})
export class Accounts implements OnInit {
  showForm: boolean = false;
  data: IPaginationResult<GetAccount> | null = null;
  isLoading: boolean = false;
  editId: string | null = null;
  initialValues: CreateOrUpdateAccount = formEmpty;
  searchQuery: string = '';
  accountForm: FormGroup;
  users: GetUser[] = [];

  columns: Column<GetAccount>[] = [
    { key: 'accountNumber', label: 'No. Cuenta' },
    { key: 'accountType', label: 'Tipo de cuenta' },
    {
      key: 'initialBalance',
      label: 'Balance inicial',
      render: (value: unknown, row: GetAccount) => `$${row.initialBalance}`,
    },
    {
      key: 'currentBalance',
      label: 'Balance Actual',
      render: (value: unknown, row: GetAccount) => `$${row.currentBalance}`,
    },
    {
      key: 'dailyDebitLimit',
      label: 'Límite diario de débito',
      render: (value: unknown, row: GetAccount) => `$${row.dailyDebitLimit}`,
    },
    {
      key: 'user',
      label: 'Cliente',
      render: (value: unknown, row: GetAccount) => row.user?.name ?? 'N/A',
    },
    {
      key: 'actions',
      label: 'Acciones',
      isButtons:true,
      render: (value: unknown, row: GetAccount) => `
        <div>
          <button class="btn-edit" data-action="edit" data-id="${row.id}">Editar</button>
          <button class="btn-delete" data-action="delete" data-id="${row.id}">Eliminar</button>
        </div>
      `,
    },
  ];

  constructor(
    private accountService: AccountService,
    private userService: UserService,
    private fb: FormBuilder
  ) {
    this.accountForm = this.fb.group({
      accountNumber: ['', Validators.required],
      accountType: ['ahorros', Validators.required],
      initialBalance: [0, [Validators.required, Validators.min(0)]],
      userId: ['', Validators.required],
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
    this.accountService
      .getAllPaginated<GetAccount>({
        page: page,
        query: q,
      })
      .subscribe({
        next: (result: { data: IPaginationResult<GetAccount> }) => {
          this.data = result.data;
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('Error:' + error);
        },
      });
  }

  async onSubmit(): Promise<void> {
    if (this.accountForm.invalid) {
      this.accountForm.markAllAsTouched();
      return;
    }

    let serviceCall: Observable<any>;
    let msg = 'Creando con éxito';

    if (this.editId) {
      serviceCall = this.accountService.edit(
        this.editId,
        this.accountForm.value as CreateOrUpdateAccount
      );
      msg = 'Actualizado con éxito';
    } else {
      serviceCall = this.accountService.post<
        CreateOrUpdateAccount,
        CreateOrUpdateAccount
      >(this.accountForm.value as CreateOrUpdateAccount);
    }

    serviceCall.subscribe({
      next: () => {
        alert(msg);
        this.getAll();
        this.editId = null;
        this.initialValues = { ...formEmpty };
        this.onCloseModal();
      },
      error: (error: any) => {
        console.error(error);
      },
    });
  }

  handleChangeSearch(event: Event): void {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.getAll(this.searchQuery);
  }

  async handleActionEdit(row: GetAccount): Promise<void> {
    this.editId = row.id;
    await this.getUsers();
    this.initialValues = { ...row };
    this.accountForm.patchValue(row);
    this.showForm = true;
  }

  handleClickDelete(id: string): void {
    if (!id) return;

    const result = confirm('¿Está seguro de que desea eliminar esta cuenta?');
    if (!result) return;

    this.accountService.remove(id).subscribe({
      next: (_: any) => {
        alert('Cuenta eliminada con éxito');
        this.getAll();
      },
      error: (error: any) => {
        console.error(error);
      },
    });
  }

  handleChangePage(page: number): void {
    this.getAll(this.searchQuery, page);
  }

  handleTableAction(event: { action: string; row: GetAccount }): void {
    if (event.action === 'edit') {
      this.handleActionEdit(event.row);
    } else if (event.action === 'delete') {
      this.handleClickDelete(event.row.id);
    }
  }

  async handleClickNew(): Promise<void> {
    await this.getUsers();
    this.editId = null;
    this.initialValues = { ...formEmpty };
    this.accountForm.reset(formEmpty);
    this.showForm = true;
  }

  onCloseModal(): void {
    this.showForm = false;
    this.accountForm.reset(formEmpty);
  }

  async getUsers(): Promise<void> {
    try {

      const response = await firstValueFrom(
        this.userService.getAllPaginated<GetUser>({
          noPaginate: true,
        })
      );
      const data = response?.data;
      this.users = data.results;
    } catch (error) {
      console.error('Error al obtener usuarios:', error);
    }
  }
}
