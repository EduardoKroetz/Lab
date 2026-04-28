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
- Somente na criação de um risco, caso o impacto não seja informado, ele deve ser inferido automaticamente pela criticalidade do Ativo vinculado.
- Qualquer alteração no risco gera um histórico e uma Snapshot
- O Score do risco é influenciado por
	- Probabilidade X Impacto
	- Eficácia de controles preventivos (afeta probabilidade)
	- Eficácia dos controles detectivos e corretivos (afeta impacto)
	- Quantidade de incidentes vinculados ao risco e seu Score
- Tratamento (Treatment)
	- Mitigate
		- Requer ao menos 1 controle vinculado
		- Eficácia influencia score apenas quando definida
	- Accept
		- Requer justificativa
		- Status do risco é movido para Monitoring (Monitoramento)
	- Transfer: requer descrição com detalhes sobre a transferência
	- Eliminate
		- Requer desativar o ativo vinculado
		- Status do risco é movido para Closed (Fechado)
- Status e Transições
	- Identified: status padrão ao criar um risco
	- Identified -> UnderTreatment
		- Existe pelo menos uma Task aberta vinculado ao risco 
		- OU o Tratamento = Mitigado
	- UnderTreatment -> Monitoring
		- Todas as Tasks vinculadas ao risco foram concluídas
	- Monitoring -> Closed
		- Possui justificativa
		- e:
			- Tratamento = Aceito OU Eliminado
			- OU decisão manual explícita
	- Monitoring -> UnderTreatment
		- novo incidente
		- aumento do Score
		- criação de Task do tipo:
		  - RiskTreatment
		  - ControlExecution
		  - IncidentResponse
	- Closed -> UnderTreatment: 
		- Novo incidente
- Revisões e Vencimento de Risco
	- O vencimento pode ser somente: 
		- Fixo: informando uma data fixa OU
		- Periódico: informando um intervalo de tempo
	- Ao vencer, uma task é gerada automaticamente ao responsável do risco

### Controles 
- Forma de defender/mitigar o risco
- Os controles vinculados a riscos afetam diretamente o cálculo de Score do risco usando a eficácia do controle sobre o risco. Se não tiver eficácia, não influencia.
- Um controle pode ser atribuído diretamente a um risco, sem necessidade de uma Task, porém sua eficácia só pode ser definida após a conclusão de uma Task.

### Tasks / Tarefas
- São tarefas designadas a usuários e representam uma ação/execução do usuário
- Ao concluir a última task vinculada ao risco, o status do risco é alterado automaticamente para Monitoring (Monitoramento)
- Ao criar uma Task do tipo RiskTreatment, ControlExecution ou IncidentResponse, o status do risco vinculado é alterado automaticamente para UnderTreatment (Em Tratamento)
- Tasks do tipo RiskReview
	- Não alteram status do risco
- Tasks do tipo ControlExecution (Execução de controle):
	- Vincula automaticamente o risco ao controle ao concluir a Task.
	- Exige que a eficácia seja informada/reavaliada ao concluir a Task.

### Incidentes
- Representam um problema real que o ataque causou
- Impactos
	- Consequência do ataque
	- Influenciam o cálculo de Score do risco
- Um incidente deve estar vinculado à um risco
- O Score do incidente é calculado com base na quantidade e nível dos impactos vinculados
- Caso não tenha nenhum impacto vinculado ao incidente, o nível pode ser alterado livremente, porém se tiver impactos vinculados o Nível passa a ser inferido pelo sistema.
- Ao criar incidente
	- Status do risco de move automaticamente para UnderTreatment (Em Tratamento)
	- Cria uma Task automaticamente


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

