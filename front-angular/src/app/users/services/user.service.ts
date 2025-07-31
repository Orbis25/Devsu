import { Injectable } from '@angular/core';
import { BaseService } from '../../../shared/services/base.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseService {

  constructor(private _http: HttpClient) {
    super(_http, 'users');
  }
  }
