/* eslint-disable @typescript-eslint/no-explicit-any */
export interface IBaseErrorHttpResponse {
  message: string;
  data: any;
}

export const isBadResponse = (obj: any): obj is IBaseErrorHttpResponse => {
  return obj && typeof obj.message === "string" && !obj.isSuccess;
};

export const isProblemDetail = (obj: any): obj is IProblemDetail => {
  return (
    obj &&
    typeof obj.type === "string" &&
    typeof obj.title === "string" &&
    typeof obj.status === "number" &&
    obj.errors
  );
};

export type IProblemDetail = {
  type: string;
  title: string;
  status: number;
  errors: any;
}
