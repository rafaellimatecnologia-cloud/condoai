# CondoAI — Gestão Condominial com IA (Ponto, Escalas, Portaria)

CondoAI é uma plataforma **offline-first** para gestão operacional de condomínios: **folha de ponto com evidências**, **escala de trabalho**, **registro de ocorrências por departamento**, **painel ADM em tempo real** e **IA como cérebro do gestor** para automatizar configurações e rotinas (escala, lembretes, relatórios, alertas).

## Objetivo
- Registrar ponto de funcionários com **selfie obrigatória**, **geolocalização**, **Wi-Fi cadastrado** e **QR fixo atualizável**
- Suportar escalas iniciais:
  - **Manutenção 6x1** (folgas revezadas sábado/domingo)
  - **Limpeza 6x1** (Manhã 08:00–16:20, Tarde 13:40–22:00, Noite2 16:40–00:00)
  - **Portaria 12x36** (06–18 e 18–06)
  - **Vigias 12x36** (20–08)
- Operar fora da rede via **DDNS + HTTPS**, com perfis **Admin** e **Usuário**
- Enviar “espelho de ponto” por **e-mail (Gmail API)** em PDF
- Notificações **Push (FCM)** para lembretes de expediente/escala

## Módulos
- **CondoAI Admin (Web)**: configuração total do sistema, cadastros, escalas, auditoria, relatórios, IA chat
- **CondoAI Server (Windows)**: API + Realtime (SignalR) + DB (Postgres) + jobs + integrações (Gmail/FCM)
- **CondoAI Mobile (Android Funcionário)**: ponto offline, agenda, presença “Em expediente”
- **CondoAI Gate (Android Portaria Tablet)**: chaves/encomendas/visitantes (MVP2)

## Conceito de Home (Android)
Home simples (branco/azul), com ícones:
- **Registro de Ponto**
- **Agenda**
- **Vigia**
- **ADM** (acesso restrito por perfil)

## Princípios
- **Offline-first**: o app registra tudo sem internet e sincroniza quando possível
- **Outbox + Idempotência**: cada evento tem `clientEventId` (UUID) e nunca duplica
- **Fonte de verdade no servidor**: políticas, escalas, auditoria e relatórios
- **IA sugere, servidor valida e executa** (RBAC + motor de regras)
- **Segurança**: HTTPS, tokens, auditoria, retenção de selfies

## Arquitetura (alto nível)
```mermaid
flowchart TB
  subgraph ANDROID[Android (Funcionário/Portaria)]
    A1[Login (Senha/PIN + BiometricPrompt)]
    A2[Ponto (Selfie + GPS + Wi-Fi + QR)]
    A3[Room DB + Outbox]
    A4[Agenda (Calendar Provider)]
    A5[Push (FCM)]
  end

  subgraph SERVER[Server Windows]
    S1[API REST (.NET 8)]
    S2[Realtime (SignalR)]
    S3[(PostgreSQL)]
    S4[Rules Engine (Escalas/Políticas)]
    S5[IA Orchestrator (ChatGPT API)]
    S6[Jobs (Escala, Lembretes, Fechamento)]
    S7[Gmail API (envio PDF)]
    S8[Portal Admin Web]
  end

  A2 --> A3
  A3 -->|sync/batch| S1
  S1 --> S3
  S1 --> S4
  S5 -->|comandos| S1
  S6 -->|notificações| A5
  S7 -->|emails| SERVER
  S1 --> S2
  S2 --> A1
  S2 --> S8
  S8 --> S1
