CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Users (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" TEXT NOT NULL,
    "Gender" TEXT,
    "Age" INT,
    "Identification" TEXT,
    "Address" TEXT,
    "Phone" TEXT,
    "ClientId" TEXT,
    "Password" TEXT,
    "Status" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE
);


CREATE TABLE Accounts (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "AccountNumber" TEXT NOT NULL,
    "AccountType" TEXT,
    "InitialBalance" NUMERIC(18, 2),
    "CurrentBalance" NUMERIC(18, 2),
    "Status" BOOLEAN DEFAULT TRUE,
    "UserId" UUID NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
   FOREIGN KEY ("UserId") REFERENCES Users("Id") ON DELETE CASCADE
   );

CREATE TABLE Transactions (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Type" TEXT,
    "CurrentBalance" NUMERIC(18, 2),
    "Amount" NUMERIC(18, 2),
    "Status" BOOLEAN DEFAULT TRUE,
    "Movement" TEXT,
    "AccountId" UUID NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "IsDeleted" BOOLEAN DEFAULT FALSE,
    FOREIGN KEY ("AccountId") REFERENCES Accounts("Id") ON DELETE CASCADE
);
