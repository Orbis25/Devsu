import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IPaginate, IPaginationResult } from "../interfaces/pagination.interface";
import { environment } from '../../environment';

export type Endpoint =
  | "users"
  | "transactions"
  | "accounts";

export interface BaseResponseCore {
  message: string;
  success: boolean;
}

export interface BaseResponse<T> extends BaseResponseCore {
  data: T;
}

@Injectable({
  providedIn: 'root',
})
export class BaseService {
  baseEndpoint: string;
  constructor(private http: HttpClient, @Inject('action') action: Endpoint) {
    this.baseEndpoint = `${environment.apiUrl}/api/${action}`;
  }

  post<TInput, TResponse>(
    model: TInput,
    newEndpoint: string = '',
    headers?: HttpHeaders
  ): Observable<BaseResponse<TResponse>> {
    const url = `${this.baseEndpoint}/${newEndpoint}`;
    return this.http.post<BaseResponse<TResponse>>(
      newEndpoint ? url : this.baseEndpoint,
      model,
      { headers }
    );
  }

  getAllPaginated<TData>(
    model: IPaginate = { page: 1, qyt: 10 },
    newEndpoint: string = '',
    headers?: HttpHeaders
  ): Observable<BaseResponse<IPaginationResult<TData>>> {
    const params = new HttpParams({ fromObject: { ...model } });
    return this.http.get<BaseResponse<IPaginationResult<TData>>>(
      newEndpoint ? `${this.baseEndpoint}/${newEndpoint}` : this.baseEndpoint,
      { params, headers }
    );
  }

  getOne<TData>(
    id: string,
    newEndpoint: string = '',
    headers?: HttpHeaders
  ): Observable<BaseResponse<TData>> {
    return this.http.get<BaseResponse<TData>>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      { headers }
    );
  }

  remove(
    id: string,
    newEndpoint: string = '',
    headers?: HttpHeaders
  ): Observable<BaseResponseCore> {
    return this.http.delete<BaseResponseCore>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      { headers }
    );
  }

  edit(
    id: string,
    model: any,
    newEndpoint: string = '',
    headers?: HttpHeaders
  ): Observable<BaseResponseCore> {
    return this.http.put<BaseResponseCore>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      model,
      { headers }
    );
  }
}
