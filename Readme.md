
# Entidades e Domínio

## Auditoria

Auditoria não deve ser posta em entidades pois não faz parte de dominio, e sim de Infra.

Campos como CreatedAt, UpdatedAt, CreatedBy, UpdatedBy são campos de auditoria, que apesar de poderem ser informações de negócio, poluem o dominio e ainda permite que regras de negócio sejam aplicadas sobre ele.

Esses campos de auditoria podem e devem ser mapeados na camada de Infra.

# Segurança 

## JWT 
JWT é um token auto-contido, assinado, que representa um conjunto de claims.

Auto-contido = tudo que o servidor precisa pra confiar naquele token já está dentro dele.
Isso significa que o servidor não preciso consultar banco, cache, nem sessão, só precisa da chave da assinatura e das regras de validação.

Estrutura: header.payload.signature
- Header: algoritmo e tipo
- Payload: claims (dados do usuário)
- Signature: prova criptográfica de integridade e autoria

Nada é secreto no token. Tudo é legível. A confiança vem só da assinatura.

A assinatura é gerada assim:
HMAC(chave, conteúdo) → assinatura

### Por que alterar o payload invalida o token
- qualquer alteração muda o hash
- a assinatura original não corresponde mais
- sem a chave, não dá pra gerar outra válida
O servidor:
- não conhece o payload original
- só verifica coerência matemática entre conteúdo recebido e assinatura

# ASP.NET Core

## Autenticação e Autorização

O ASP.NET Core não autentica, ele é mais um orquestrador de esquemas de autenticação. 

Através de IAuthenticationHandler é possível injetar diferentes formas de se autenticar e diferentes implementações/handlers de autenticação. 

E assim, através do esquema (ex: JWT, OAuth) é possível definir, por ex:
- Authenticate (como o usuário vai ser autenticado)
- Challange (como o ASP.NET Core vai lidar com usuário não autenticado. Se vai retornar 401 (JWT), 302 (Cookies), etc)
- Forbid (como lidar com usuários não autorizados)
Cada esquema tem exatamente um handler.

O AuthenticationMiddleware por sua vez é quem tenta autenticar (chama HttpContext.AuthenticateAsync).
Se a autenticação der sucesso, define HttpContext.User.
Através de UseAuthentication que esse middleware é ativado.

E o Authorization verifica a permissão do usuário, se possui claim X, passa em certa policy e caso não, retorna Challange ou Forbid

### Fluxo

Request
 ↓
UseAuthentication
 ↓
AuthenticationHandler (por scheme)
 ↓
HttpContext.User
 ↓
UseAuthorization
 ↓
[Authorize] → Challenge / Forbid

## Outras dúvidas

Como funciona container DI por baixo?
Se eu não usar async em um controller, qual o impacto?





