export interface CreateOrUpdateTransaction {
    type:"debito" | "credito";
    amount:number;
    accountId:string;
}