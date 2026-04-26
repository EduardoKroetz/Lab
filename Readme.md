# Escopo

## 1. Multi-Tenant

- Cada entidade do sistema deve possuir `TenantId`
- Um tenant representa um negócio (ex: barbearia, clínica)
- Isolamento lógico de dados por tenant
- Todas as operações devem respeitar o contexto do tenant

## Risk Management

### Architecture Decision Record

#### ADR — Propriedades pré-calculadas em Risk

Decisão: Persistir EffectivenessOnProbability e EffectivenessOnImpact em vez de calcular dinamicamente a partir de RiskControls.
Motivação: Evitar necessidade de carregar coleções e dependência de navegação (RiskControls + Control.Type) para calcular Score.

Consequências:
- ✔ Leitura mais simples e performática
- ✔ Domínio independente de carregamento de dados
- ❗ Necessário garantir recalculo ao adicionar/remover/alterar RiskControl
- ❗ Possível inconsistência se regras não forem respeitadas

Regra: Toda alteração em RiskControl deve acionar o recálculo da effectiveness no Risk.

