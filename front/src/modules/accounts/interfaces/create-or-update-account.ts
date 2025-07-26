export interface CreateOrUpdateAccount {
  accountNumber: string;
  accountType: "ahorros" | "corriente";
  initialBalance: number;
  userId: string;
}