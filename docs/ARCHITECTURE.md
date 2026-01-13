
---

# docs/ARCHITECTURE.md

```md
# Arquitetura — CondoAI (v1)

## 1) Decisões principais
- Server: **.NET 8 + SignalR**
- DB: **PostgreSQL**
- Acesso externo: **DDNS + HTTPS (reverse proxy)**
- Push: **FCM**
- E-mail: **Gmail API (OAuth)**
- Android: **Kotlin + Compose + Room + WorkManager**

## 2) Perfis e permissões (RBAC)
- **ADMIN**: configurações globais, políticas, escalas, relatórios, aprovações, IA
- **USER**: ponto, agenda, seus relatórios
- **GATE**: operações de portaria (MVP2)

Todas as ações sensíveis geram **AuditLog**.

## 3) Offline-first + Outbox + Idempotência
### Evento local (Android)
Cada ação gera um evento com:
- `clientEventId: UUID`
- `eventType`
- `occurredAtUtc`
- evidências: selfieMediaId, gps, wifi, qrId/qrVersion
- `state`: PENDING_UPLOAD | ACKED | FAILED

### Sync Engine
- WorkManager executa:
  1) `POST /sync/batch` com lote de eventos PENDING
  2) marca ACK por `clientEventId`
  3) sincroniza `GET /presence/today` e escala da semana

### Idempotência no servidor
- `clientEventId` é UNIQUE no banco
- Se repetido: servidor retorna o mesmo ACK

## 4) Evidências de presença no local
Condição de “ponto válido” (configurável):
- **Selfie obrigatória**
- **Wi-Fi** em allowlist (SSID/BSSID)
- **GPS** dentro do geofence (raio) com accuracy mínimo
- **QR fixo** com `qrVersion` atual (atualizável pelo gestor)

Resultado da validação:
- OK
- WARNING
- PENDING_APPROVAL (ex.: fora da janela, GPS fora, etc.)
- BLOCK (se política exigir bloqueio)

## 5) QR fixo com “update” (sem trocar o adesivo)
- QR impresso contém `qrId` (ex.: PORTARIA_QR_01)
- Painel ADM possui “Atualizar QR”:
  - incrementa `qrVersion`
  - (opcional) muda segredo/assinatura para a versão
- App registra `qrId` + `qrVersionUsed` no evento

## 6) Escalas (V1) e transições
### Tipos suportados
- 6x1 (Manutenção/Limpeza)
- 12x36 (Portaria/Vigias)

### Turnos (templates)
- Limpeza: Manhã / Tarde / Noite2
- Portaria: Diurno / Noturno
- Vigia: 20–08

### Motor de escala (determinístico)
- Gera `WorkScheduleDay` (dia a dia)
- Detecta conflitos:
  - troca de padrão sem período de transição
  - sobreposição de turnos
  - ausência de folga prevista
- Responde “amanhã trabalha?” consultando escala gerada
- IA apenas **solicita** mudanças, o motor **valida e aplica**

## 7) Folha de ponto (fechamento) + e-mail
### Fechamento
- Consolida batidas + aprovações
- Gera PDF “Espelho de Ponto” por período
- Armazena PDF como MediaRef
- Envia por Gmail API e salva `emailMessageId`

## 8) IA como “cérebro do gestor”
### Padrão seguro: IA sugere → servidor valida → executa
O chat do painel ADM chama `/ai/command`.
Servidor:
1) envia contexto mínimo para IA
2) IA retorna **Action JSON**
3) servidor valida RBAC e regras
4) aplica e registra auditoria

Exemplos de ações:
- CreateEmployee
- CreateScheduleBatch
- ChangeAssignmentPattern
- UpdateWorkNetworkAllowlist
- GenerateTimesheetAndEmail
- SendFCMNotification

## 9) Realtime (SignalR)
- Hub: `/realtime`
- Eventos:
  - `presenceDiff`: { employeeId, status, sinceUtc, lastUpdateUtc }
  - `notification`: { severity, title, message, createdAtUtc }
