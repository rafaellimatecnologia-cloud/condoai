# Regras para IA (Codex/Gemini) — CondoAI

1) Tudo deve **compilar** (server e android).
2) **Nunca** commitar secrets (API keys, tokens, credenciais).
3) Offline-first obrigatório: **Room + Outbox + idempotência** (clientEventId).
4) Toda ação crítica gera **AuditLog** (escala, políticas, aprovações, envio email).
5) Aplicar **RBAC** (ADMIN/USER/GATE) em endpoints e telas.
6) Sempre incluir scripts de build e instruções de execução.
