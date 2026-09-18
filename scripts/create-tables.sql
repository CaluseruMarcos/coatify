-- Run from the repository root:
-- docker compose exec -T postgres psql -U coatify -d coatify < scripts/create-tables.sql
-- Matches the current Domain models and CoatifyContext DbSet names.
-- Quoted identifiers preserve the casing expected by EF Core/Npgsql.
-- Existing tables are left unchanged; this is not a migration script.
-- Supply IDs and UTC timestamps from the application (no SQL defaults).

\set ON_ERROR_STOP on

BEGIN;

CREATE TABLE IF NOT EXISTS public."Users"
(
    "Id" uuid NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."DeviceTypes"
(
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL,
    CONSTRAINT "PK_DeviceTypes" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."Devices"
(
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "SerialNumber" text NOT NULL,
    -- EF infers this shadow FK from the required DeviceType navigation.
    "DeviceTypeId" uuid NOT NULL,
    "Status" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Devices" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Devices_DeviceTypes_DeviceTypeId"
        FOREIGN KEY ("DeviceTypeId") REFERENCES public."DeviceTypes" ("Id")
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_Devices_DeviceTypeId"
    ON public."Devices" ("DeviceTypeId");

CREATE TABLE IF NOT EXISTS public."Measurements"
(
    "Id" uuid NOT NULL,
    -- No navigation or explicit relationship is currently configured in C#.
    -- DeviceId alone does not make EF discover a foreign-key relationship.
    "DeviceId" uuid NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL,
    "Temperature" real NOT NULL,
    "Pressure" real NOT NULL,
    "PowerConsumption" real NOT NULL,
    CONSTRAINT "PK_Measurements" PRIMARY KEY ("Id")
);

COMMIT;
