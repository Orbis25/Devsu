import { HttpClient } from "@angular/common/http";
import { BaseService } from "../../../shared/services/base.service";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root',
})
export class AccountService extends BaseService {
  constructor(private _http: HttpClient) {
    super(_http, 'accounts');
  }
}