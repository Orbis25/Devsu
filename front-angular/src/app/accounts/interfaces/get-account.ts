import { GetUser } from "../../users/interfaces/get-user";

export interface GetAccount {
  id: string;
  accountNumber: string;
  accountType: "ahorros" | "corriente";
  initialBalance: number;
  currentBalance: number;
  dailyDebitLimit: number;
  user?: GetUser;
  userId: string;
}
