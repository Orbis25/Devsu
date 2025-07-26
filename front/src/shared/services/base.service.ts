/* eslint-disable @typescript-eslint/no-explicit-any */
import axiosInstance from "../../config/axios.config";
import {
  IPaginate,
  IPaginationResult,
} from "@/shared/interfaces/pagination.interface";
import { AxiosRequestConfig, AxiosResponse } from "axios";

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
    message: string;
    success: boolean;
  }

export class BaseService {
  baseEndpoint;
  axiosInstance;
  constructor(baseEndpoint: Endpoint) {
    this.baseEndpoint = `${import.meta.env.VITE_API_URL}/api/${baseEndpoint}`;
    this.axiosInstance = axiosInstance;
  }

  async post<TInput, TResponse>(
    model: TInput,
    newEndpoint: string = "",
    config?: AxiosRequestConfig<TInput>
  ): Promise<AxiosResponse<BaseResponse<TResponse>>> {
    const url = `${this.baseEndpoint}/${newEndpoint}`;
    const response = await axiosInstance.post<TResponse>(
      newEndpoint ? url : this.baseEndpoint,
      model,
      config
    );

    return response as AxiosResponse<BaseResponse<TResponse>>;
  }

  async getAllPaginated<TData>(
    model: IPaginate = { page: 1, qyt: 10 },
    newEndpoint: string = "",
    config?: AxiosRequestConfig
  ) {
    const response = await axiosInstance.get<BaseResponse<IPaginationResult<TData>>>(
      newEndpoint ? `${this.baseEndpoint}/${newEndpoint}` : this.baseEndpoint,
      {
        params: model,
        ...config,
      }
    );

    return response;
  }

  async getOne<TData>(
    id: string,
    newEndpoint: string = "",
    config?: AxiosRequestConfig
  ) {
    const response = await axiosInstance.get<BaseResponse<TData>>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      config
    );

    return response;
  }

  async remove(
    id: string,
    newEndpoint: string = "",
    config?: AxiosRequestConfig
  ) {
    const response = await axiosInstance.delete<BaseResponseCore>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      config
    );

    return response;
  }

  async edit(
    id: string,
    model: any,
    newEndpoint: string = "",
    config?: AxiosRequestConfig
  ) {
    return await axiosInstance.put<BaseResponseCore>(
      newEndpoint
        ? `${this.baseEndpoint}/${newEndpoint}/${id}`
        : `${this.baseEndpoint}/${id}`,
      model,
      config
    );
  }
}
