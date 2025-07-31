import { GetAccount } from "../../accounts/interfaces/get-account";

export interface GetTransaction {
  id: string;
  type: "debito" | "credito";
  amount: number;
  currentBalance: number;
  movement: string;
  accountId: string;
  account?: GetAccount;
  createdAtStr: string;
  status: boolean;
}