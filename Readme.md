# Escopo

## 1. Multi-Tenant

- Cada entidade do sistema deve possuir `TenantId`
- Um tenant representa um negócio (ex: barbearia, clínica)
- Isolamento lógico de dados por tenant
- Todas as operações devem respeitar o contexto do tenant

---

## 2. Entidades principais

### Cliente

- Nome
- Telefone / Email
- Pertence a um tenant
- Pode criar agendamentos (sem necessidade de autenticação inicialmente)

---

### Profissional

- Nome
- Pertence a um tenant
- Possui:
    - horários de trabalho
    - exceções de agenda
    - tempo de intervalo entre atendimentos (opcional)

---

### Serviço

- Nome
- Duração padrão (em minutos)
- Pertence a um tenant

---

### ServiceProfessional (relação)

- Relaciona Serviço ↔ Profissional
- Permite:
    - associar múltiplos profissionais a um serviço
    - definir duração específica por profissional (override)

---

### Agendamento

- Cliente
- Profissional
- Serviço
- Data/Hora de início
- Duração
- Estado
- Pertence a um tenant

---

## 3. Regras de Duração

Ordem de precedência da duração:

1. Duração definida no agendamento (manual)
2. Duração específica do profissional (`ServiceProfessional`)
3. Duração padrão do serviço

---

## 4. Disponibilidade do Profissional

### Horários de trabalho

- Definidos por dia da semana
- Contém:
    - horário de início
    - horário de fim

---

### Exceções de agenda

- Datas específicas
- Podem:
    - bloquear completamente o dia
    - sobrescrever horário padrão

---

### Intervalo entre atendimentos

- Tempo adicional após cada agendamento
- Opcional por profissional

---

## 5. Regras de Agendamento

- Um agendamento deve respeitar:
    - horário de trabalho do profissional
    - exceções
    - duração do serviço
    - intervalo entre atendimentos
- Não permitir conflito de horários por padrão
- (Futuro) Permitir exceções controladas (overbooking)

---

## 6. Estados do Agendamento

Estados possíveis:

- Criado
- Confirmado
- Cancelado
- Concluído

---

### Regras de transição (inicial)

- Criado → Confirmado
- Criado → Cancelado
- Confirmado → Cancelado
- Confirmado → Concluído
- Não permitir transições inválidas

---

### Conclusão automática

- Agendamentos devem ser marcados como "Concluído" após o horário de término