# Escopo

## Multi-Tenant

- Cada entidade do sistema deve possuir `TenantId`
- Um tenant representa um negócio (ex: barbearia, clínica)
- Isolamento lógico de dados por tenant
- Todas as operações devem respeitar o contexto do tenant

## Risk Management

### Ativos
- Representa algo de valor (ex: equipamento físico, software, etc)

### Ameaças
- Causa potencial de dano (ex: ransomware, erro humano, falha de servidor)

### Vulnerabilidades
- Fraqueza (ex: API sem validação, senha sem hash no banco)

### Riscos
- Possibilidade de uma ameaça explorar uma vulnerabilidade e causar dano no ativo
- Qualquer alteração no risco gera um histórico e uma Snapshot
- O Score do risco é influenciado por
	- Probabilidade X Impacto
	- Eficácia de controles preventivos (afeta probabilidade)
	- Eficácia dos controles detectivos e corretivos (afeta impacto)
	- Quantidade de incidentes vinculados ao risco e seu Score
- Tratamento (Treatment)
	- Mitigate: requer ao menos 1 controle vinculado
	- Accept: requer justificativa
	- Transfer: requer descrição com detalhes sobre a transferência
	- Eliminate: requer descrição de como o risco foi eliminado
- Status
	- Open
	- Closed
- Revisões
	- A revisão pode ser somente: 
		- Fixa: informando uma data fixa OU
		- Periódica: informando um intervalo de tempo

### Controles 
- Forma de mitigar o risco
- Os controles vinculados a riscos afetam diretamente o cálculo de Score do risco usando a eficácia do controle sobre o risco. Se não tiver eficácia, não influencia.

### Incidentes
- Representam um problema real que o ataque causou
- Impactos
	- Consequência do ataque
	- Influenciam o cálculo de Score do risco
- Um incidente deve estar vinculado à um risco
- O Score do incidente é calculado com base na quantidade e nível dos impactos vinculados

## Architecture Decision Record

### ADR — Propriedades pré-calculadas em Risk

Decisão: Persistir EffectivenessOnProbability e EffectivenessOnImpact em vez de calcular dinamicamente a partir de RiskControls. 

Motivação: Evitar necessidade de carregar coleções e dependência de navegação (RiskControls + Control.Type) para calcular Score.

Consequências:
- ✔ Leitura mais simples e performática
- ✔ Domínio independente de carregamento de dados
- ❗ Necessário garantir recalculo ao adicionar/remover/alterar RiskControl
- ❗ Possível inconsistência se regras não forem respeitadas

Regra: Toda alteração em RiskControl deve acionar o recálculo da effectiveness no Risk.

