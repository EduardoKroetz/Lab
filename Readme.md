
# Entidades e Domínio

## Auditoria

Auditoria não deve ser posta em entidades pois não faz parte de dominio, e sim de Infra.

Campos como CreatedAt, UpdatedAt, CreatedBy, UpdatedBy são campos de auditoria, que apesar de poderem ser informações de negócio, poluem o dominio e ainda permite que regras de negócio sejam aplicadas sobre ele.

Esses campos de auditoria podem e devem ser mapeados na camada de Infra.

## Entidade Base
