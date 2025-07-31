export interface CreateOrUpdateUser {
  name: string;
  gender: "hombre" | "mujer";
  age: number;
  identification: string;
  phone: string;
  address: string;
  clientId: string;
  password: string;
}