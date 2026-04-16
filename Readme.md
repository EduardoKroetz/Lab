# Escopo

## 1. Multi-Tenant

- Cada entidade do sistema deve possuir `TenantId`
- Um tenant representa um negócio (ex: barbearia, clínica)
- Isolamento lógico de dados por tenant
- Todas as operações devem respeitar o contexto do tenant

