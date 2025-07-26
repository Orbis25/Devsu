export interface IPaginate {
  page?: number;
  qyt?: number;
  orderByDesc?: number;
  noPaginate?: boolean;
  query?: string;
}

export interface IPaginationResult<T> {
  actualPage: number;
  qyt: number;
  pageTotal: number;
  results: T[];
  total: number;
}
