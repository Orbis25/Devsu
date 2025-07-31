import { Injectable } from '@angular/core';
import { IPaginationResult } from '../../../shared/interfaces/pagination.interface';
import {
  BaseResponse,
  BaseService,
} from '../../../shared/services/base.service';
import { GetTransaction } from '../interfaces/get-transaction';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TransactionService extends BaseService {
  constructor(private _http: HttpClient) {
    super(_http, 'transactions');
  }

  search<T>(search: {
    page?: number;
    qyt?: number;
    noPaginate?: boolean;
    from?: string;
    to?: string;
    query?: string;
  }): Observable<BaseResponse<IPaginationResult<T>>> {
    let params = new HttpParams();
    if (search.page) params = params.append('page', search.page.toString());
    if (search.qyt) params = params.append('qyt', search.qyt.toString());
    if (search.noPaginate) params = params.append('noPaginate', search.noPaginate.toString());
    if (search.from) params = params.append('from', search.from);
    if (search.to) params = params.append('to', search.to);
    if (search.query) params = params.append('query', search.query);

    return this._http.get<BaseResponse<IPaginationResult<T>>>(`${this.baseEndpoint}/report`, {params});
  }

  exportFile(params: { page: number; from: string; to: string; type: 'pdf' | 'json' }): Observable<BaseResponse<string>> {
    let queryParams = new HttpParams();
    queryParams = queryParams.append('page', params.page.toString());
    queryParams = queryParams.append('from', params.from);
    queryParams = queryParams.append('to', params.to);
    queryParams = queryParams.append('type', params.type);

    return this._http.get<BaseResponse<string>>(`${this.baseEndpoint}/report/${params.type}`, {params: queryParams});
  
  }
}
