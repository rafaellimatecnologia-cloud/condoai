# Modelo de Dados (v1) — PostgreSQL

## Tabelas principais

### employees
- id (uuid)
- full_name (text)
- department (text) — MANUTENCAO | LIMPEZA | PORTARIA | VIGIA | ADM
- role (text) — ADMIN | USER | GATE
- is_active (bool)
- photo_url (text, null)

### shift_templates
- id (uuid)
- department (text)
- name (text)
- start_local_time (time)
- end_local_time (time)
- break_minutes (int)

### schedule_assignments
- id (uuid)
- employee_id (uuid)
- pattern_type (text) — SIX_BY_ONE | TWELVE_BY_THIRTY_SIX
- shift_template_id (uuid)
- start_date (date)
- rotation_group (int, null)
- notes (text, null)

### work_schedule_days
- id (uuid)
- employee_id (uuid)
- date (date)
- status (text) — WORK | OFF | VACATION | SICK_LEAVE
- shift_template_id (uuid, null)
- planned_start_utc (timestamptz, null)
- planned_end_utc (timestamptz, null)
- planned_break_minutes (int, null)
- source (text) — GENERATED | MANUAL_OVERRIDE
- version (int)

### attendance_events
- server_event_id (uuid)
- client_event_id (uuid UNIQUE)
- employee_id (uuid)
- event_type (text) — CLOCK_IN | CLOCK_OUT | BREAK_START | BREAK_END
- occurred_at_utc (timestamptz)
- device_id (text)
- lat (double precision)
- lon (double precision)
- accuracy_meters (double precision)
- wifi_ssid (text, null)
- wifi_bssid (text, null)
- qr_id (text, null)
- qr_version (int, null)
- selfie_media_id (uuid, null)
- justification (text, null)
- validation_status (text) — OK | WARNING | PENDING_APPROVAL | BLOCK
- validation_reason (text, null)

### media_refs
- id (uuid)
- kind (text) — SELFIE | PDF | VIDEO | AUDIO
- sha256 (text)
- storage_path (text)
- created_at_utc (timestamptz)
- retention_until_utc (timestamptz, null)

### audit_logs
- id (uuid)
- actor_employee_id (uuid, null)
- action (text)
- resource (text)
- result (text)
- ip (text, null)
- device_id (text, null)
- at_utc (timestamptz)
- details_json (jsonb, null)

### timesheet_periods
- id (uuid)
- employee_id (uuid)
- period_start (date)
- period_end (date)
- status (text) — OPEN | CLOSED | EMAILED
- pdf_media_id (uuid, null)
- email_message_id (text, null)
- closed_at_utc (timestamptz, null)

### system_config
- key (text PRIMARY KEY)
- value_json (jsonb)

## Configurações (system_config) sugeridas
- work_network_allowlist: { ssids:[], bssids:[] }
- geofence: { lat, lon, radiusMeters, minAccuracyMeters }
- qr: { qrId, qrVersion }
- policy: { tolerances, breakMinutesMin, etc. }
- fcm: { enabled, senderId }
