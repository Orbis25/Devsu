import { Component, Input, OnInit } from '@angular/core';
import { IPaginationResult } from '../../shared/interfaces/pagination.interface';
import { GetTransaction } from './interfaces/get-transaction';
import { CreateOrUpdateTransaction } from './interfaces/create-or-update-transaction';
import { TransactionService } from './services/transaction.service';
import { Column } from '../shared/table/table';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { GetAccount } from '../accounts/interfaces/get-account';
import { AccountService } from '../accounts/services/accounts.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

const formEmpty: CreateOrUpdateTransaction = {
  type: 'credito',
  amount: 0,
  accountId: '',
};

@Component({
  selector: 'app-transactions-component',
  standalone: false,
  templateUrl: './transactions.html',
  styleUrl: './transactions.css',
})
export class TransactionsComponent implements OnInit {
  @Input() report: boolean = false;
  @Input() title: string = 'Movimientos';

  showForm: boolean = false;
  data: IPaginationResult<GetTransaction> | null = null;
  isLoading: boolean = false;
  editId: string | null = null;
  initialValues: CreateOrUpdateTransaction = formEmpty;
  searchQuery: string = '';
  transactionForm: FormGroup;
  accounts: GetAccount[] = [];
  searchParams: URLSearchParams = new URLSearchParams();

  columns: Column<GetTransaction>[] = [
    { key: 'createdAtStr', label: 'Fecha' },
    {
      key: 'account',
      label: 'Cliente',
      render: (value: unknown, row: GetTransaction) =>
        row.account?.user?.name ?? 'N/A',
    },
    {
      key: 'account',
      label: 'Numero de cuenta',
      render: (value: unknown, row: GetTransaction) =>
        row.account?.accountNumber ?? 'N/A',
    },
    {
      key: 'account',
      label: 'Tipo de cuenta',
      render: (value: unknown, row: GetTransaction) =>
        row.account?.accountType ?? 'N/A',
    },
    {
      key: 'account',
      label: 'Saldo inicial',
      render: (value: unknown, row: GetTransaction) =>
        `$${row.account?.initialBalance}`,
    },
    { key: 'status', label: 'Estado' },
    { key: 'movement', label: 'Movimiento' },
    {
      key: 'currentBalance',
      label: 'Saldo disponible',
      render: (value: unknown, row: GetTransaction) => `$${row.currentBalance}`,
    },
    {
      key: 'actions',
      label: 'Acciones',
      isButtons: true,
      render: (value: unknown, row: GetTransaction) => `
       <div>
        <button class="btn-edit" data-action="edit" data-id="${row.id}">Editar</button>
        <button class="btn-delete" data-action="delete" data-id="${row.id}">Eliminar</button>
       </div>
      `,
    },
  ];

  constructor(
    private transactionService: TransactionService,
    private accountService: AccountService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.transactionForm = this.fb.group({
      type: ['credito', Validators.required],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      accountId: ['', Validators.required],
    });

    this.route.queryParams.subscribe((params) => {
      this.searchParams = new URLSearchParams(params as any);
      this.report = this.router.url.includes('reports');
      this.title = this.report ? 'Estado de Cuenta' : 'Movimientos';
      if (this.report) {
        this.columns = this.columns.filter((col) => col.key !== 'actions');
      }
    });
  }

  ngOnInit(): void {
    this.initialize();
  }

  initialize(): void {
    this.getAll();
  }

  getAll(q: string = this.searchQuery, clear: boolean = false): void {
    const page = clear ? 1 : Number(this.searchParams.get('page') ?? 1);
    const query = clear ? '' : q ?? this.searchParams.get('query') ?? '';
    const from = clear ? '' : this.searchParams.get('from') ?? '';
    const to = clear ? '' : this.searchParams.get('to') ?? '';

    this.isLoading = true;
    this.transactionService
      .search<GetTransaction>({
        page: page,
        query: query,
        from: from,
        to: to,
        qyt: 10,
        noPaginate: false,
      })
      .subscribe({
        next: (result) => {
          this.data = result.data;
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('Error:' + error);
          this.isLoading = false;
        },
      });
  }

  exportFile(type: 'pdf' | 'json' = 'pdf'): void {
    const page = Number(this.searchParams.get('page') ?? 1);
    const from = this.searchParams.get('from') ?? '';
    const to = this.searchParams.get('to') ?? '';

    this.isLoading = true;
    this.transactionService
      .exportFile({
        page: page,
        from: from,
        to: to,
        type: type,
      })
      .subscribe({
        next: (result) => {
          const link = document.createElement('a');
          link.href = `data:application/${type};base64,${result.data}`;
          link.download = `movimientos.${type}`;
          document.body.appendChild(link);
          link.click();
          link.remove();
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('Error al exportar el archivo:', error);
          alert('Error al exportar el archivo. Detalles en la consola.');
          console.error('Full error object:', error);
          this.isLoading = false;
        },
      });
  }

  getAccounts(): void {
    this.accountService
      .getAllPaginated<GetAccount>({
        noPaginate: true,
      })
      .subscribe({
        next: (result) => {
          this.accounts = result.data.results;
        },
        error: (error) => {
          console.error('Error al obtener cuentas:', error);
        },
      });
  }

  onSubmit(): void {
    if (this.transactionForm.invalid) {
      this.transactionForm.markAllAsTouched();
      return;
    }

    let serviceCall: Observable<any>;
    let msg = 'Creando con éxito';

    if (this.editId) {
      serviceCall = this.transactionService.edit(
        this.editId,
        this.transactionForm.value as CreateOrUpdateTransaction
      );
      msg = 'Actualizado con éxito';
    } else {
      serviceCall = this.transactionService.post(
        this.transactionForm.value as CreateOrUpdateTransaction
      );
    }

    serviceCall.subscribe({
      next: () => {
        alert(msg);
        this.onCloseModal();
        this.getAll();
      },
      error: (error: any) => {
        console.error('Error al guardar:', error);
        alert('Error al guardar');
      },
    });
  }

  handleOnClickFilter(): void {
    const from = this.searchParams.get('from') || '';
    const to = this.searchParams.get('to') || '';

    if (!from || !to) {
      alert('Debe seleccionar ambas fechas');
      return;
    }

    this.getAll();
  }

  handleChangeSearch(event: Event): void {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.setSearchParam('query', this.searchQuery);
    this.getAll(this.searchQuery);
  }

  handleActionEdit(row: GetTransaction): void {
    this.getAccounts();
    this.transactionService.getOne<GetTransaction>(row.id).subscribe({
      next: (result) => {
        this.initialValues = result.data;
        this.editId = row.id;
        this.transactionForm.patchValue(this.initialValues);
        this.showForm = true;
      },
      error: (error: any) => {
        console.error('Error al obtener transacción:', error);
      },
    });
  }

  handleClickDelete(id: string): void {
    if (!id) return;

    if (confirm('¿Estás seguro de que quieres eliminar este movimiento?')) {
      this.transactionService.remove(id).subscribe({
        next: () => {
          alert('Eliminado con éxito');
          this.getAll();
        },
        error: (error: any) => {
          console.error('Error al eliminar:', error);
          alert('Error al eliminar');
        },
      });
    }
  }

  handleChangePage(page: number): void {
    this.searchParams.set('page', page.toString());
    this.updateUrlAndGetAll();
  }

  private updateUrlAndGetAll(clear: boolean = false): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: Object.fromEntries(this.searchParams.entries()),
      queryParamsHandling: 'merge',
    });
    this.getAll(this.searchParams.get('query') ?? '', clear);
  }

  handleTableAction(event: { action: string; row: GetTransaction }): void {
    if (event.action === 'edit') {
      this.handleActionEdit(event.row);
    } else if (event.action === 'delete') {
      this.handleClickDelete(event.row.id);
    }
  }

  openModal(): void {
    this.editId = null;
    this.initialValues = { ...formEmpty };
    this.transactionForm.reset(formEmpty);
    this.showForm = true;
    this.getAccounts();
  }

  onCloseModal(): void {
    this.showForm = false;
    this.transactionForm.reset(formEmpty);
  }

  handleOnClickClear(): void {
    this.setSearchParam('page', '1');
    this.setSearchParam('from', '');
    this.setSearchParam('to', '');
    this.getAll('', true);
  }

  handleClickExport(type: 'pdf' | 'json'): void {
    this.exportFile(type);
  }

  setSearchParam(key: string, value: string): void {
    if (value) {
      this.searchParams.set(key, value);
    } else {
      this.searchParams.delete(key);
    }
    this.router.navigate([], {
      queryParams: Object.fromEntries(this.searchParams),
      queryParamsHandling: 'merge',
    });
  }
}
