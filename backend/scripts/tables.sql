CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" text NOT NULL,
    "Gender" text,
    "Age" int4,
    "Identification" text,
    "Address" text,
    "Phone" text,
    "ClientId" text,
    "Password" text,
    "Status" bool DEFAULT true,
    "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" bool DEFAULT false,
    PRIMARY KEY ("Id")
);


CREATE TABLE "Accounts" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "DailyDebitLimit" numeric,
    "DailyDebit" numeric,
    "LastDailyDebitUpdated" timestamp DEFAULT CURRENT_TIMESTAMP,
    "AccountNumber" text NOT NULL,
    "AccountType" text,
    "InitialBalance" numeric,
    "CurrentBalance" numeric,
    "Status" bool DEFAULT true,
    "UserId" uuid NOT NULL,
    "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" bool DEFAULT false,
    CONSTRAINT "Accounts_UserId_fkey" FOREIGN KEY ("UserId") REFERENCES "public"."Users"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("Id"));

CREATE TABLE "Transactions" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Type" text,
    "CurrentBalance" numeric,
    "Amount" numeric,
    "Status" bool DEFAULT true,
    "Movement" text,
    "AccountId" uuid NOT NULL,
    "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" bool DEFAULT false,
    CONSTRAINT "Transactions_AccountId_fkey" FOREIGN KEY ("AccountId") REFERENCES "public"."Accounts"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("Id")
);

