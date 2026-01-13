# CondoAI Server Scaffold

Scaffold for a .NET 8 Web API + SignalR server with EF Core (PostgreSQL) and JWT authentication.

## Quick start

```powershell
cd server
./db_up.ps1
./run_dev.ps1
```

### Requirements

- .NET SDK 8
- PostgreSQL running locally

## Configuration

Update `appsettings.json` with your Postgres connection string and JWT settings.

## Auth

- `POST /auth/login`
- Use username: `admin`, `gate`, or any other value for `USER`
- Password: `password` (scaffold default)

The API emits JWTs with roles `ADMIN`, `USER`, and `GATE`.

## Endpoints

- `GET /health`
- `POST /auth/login`
- `POST /sync/batch` (idempotent with `clientEventId`)
- `GET /presence/today`
- `GET /admin/employees`
- `POST /admin/employees`
- `PUT /admin/employees/{id}`
- `DELETE /admin/employees/{id}`
- `GET /admin/shift_templates`
- `POST /admin/shift_templates`
- `PUT /admin/shift_templates/{id}`
- `DELETE /admin/shift_templates/{id}`
- `GET /admin/config`
- `PUT /admin/config`

## Realtime

SignalR hub: `/realtime`

## Notes

Audit logs are written for employee, shift template, and config mutations.
