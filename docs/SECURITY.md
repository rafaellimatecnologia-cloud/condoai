# Segurança — CondoAI (v1)

## 1) Acesso externo (DDNS)
- Usar **HTTPS obrigatório** via reverse proxy (Caddy/Nginx).
- Bloquear HTTP.
- Considerar IP allowlist para Admin quando possível.

## 2) Autenticação (Login B)
- Usuário entra com **senha/PIN** e desbloqueia ações críticas com **BiometricPrompt** no Android.
- No servidor: sessão via **JWT + refresh token**.
- Rotação de tokens e expiração curta.

## 3) RBAC
- ADMIN: configurações, políticas, escalas, relatórios, aprovações, IA
- USER: seus dados e ponto
- GATE: portaria (MVP2)

## 4) Auditoria
Qualquer ação sensível gera AuditLog:
- mudanças de escala/política
- aprovações de ponto
- envio de e-mail
- alterações de rede/QR

## 5) Selfies e privacidade
- Selfie é evidência e deve ter:
  - retenção configurável (ex.: 90/180 dias)
  - hash (sha256)
  - controle de acesso estrito (apenas admin/gestor quando necessário)

## 6) IA (ChatGPT API)
- Minimização de dados: enviar somente contexto necessário.
- IA gera “proposta de ação”; servidor valida RBAC e regras antes de aplicar.
- Logs de comando/resultado (sem expor dados sensíveis em texto puro).

## 7) Gmail API
- OAuth tokens armazenados **criptografados** no servidor.
- Registro do messageId em timesheet_periods/audit.
