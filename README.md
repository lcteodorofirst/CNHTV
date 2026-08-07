# CNH TV

Versão 1.1.2.

Plataforma para gerenciamento e monitoramento de páginas exibidas em TVs LG webOS.

## Estrutura

- `Api/Cnhtv`: API ASP.NET Core em .NET 8 LTS seguindo as camadas Domain, Application, Infrastructure e API.
- `Ui/cnhtv`: painel administrativo Angular 21 com componentes Core locais e somente pacotes públicos.
- `WebOs/CnhtvPlayer`: player empacotado para LG webOS.
- `Database/Cnhtv.Database.sql`: script SQL Server independente.

## Comportamento

- Uma TV nova cria uma identificação persistente e se cadastra automaticamente.
- A configuração inicial usa `https://itm.curitiba.cnh.com/cnhtv/presentation`.
- O refresh começa desativado; quando habilitado, o intervalo inicial é 5 minutos.
- O player envia heartbeat a cada 30 segundos.
- Após 90 segundos sem heartbeat, o servidor marca o equipamento como offline.
- Cada período online é registrado no histórico de conexão.
- Alterações de URL ou refresh chegam ao player no heartbeat seguinte.

## Endereços de produção

- Painel web: `https://itm.curitiba.cnh.com/cnhtv`
- API: `https://itm.curitiba.cnh.com/cnhtv/api`
- Página inicial do player: `https://itm.curitiba.cnh.com/cnhtv/presentation`

O `UsePathBase("/api")` da API pressupõe que o IIS publique a aplicação com esse caminho-base dentro de `/cnhtv`.

## Preparação

1. Execute `Database/Cnhtv.Database.sql`.
2. Preencha `AppConfiguration:AppDbConnection` no `appsettings.Production.json` da API.
3. Configure em `AllowedOrigins` o endereço do painel Angular.
4. Publique a API no caminho `/cnhtv/api`.
5. Gere o Angular e publique-o em `/cnhtv`.
6. Empacote `WebOs/CnhtvPlayer` como IPK usando o webOS Studio.

## Dependências públicas

O frontend usa somente `registry.npmjs.org`; não depende do feed privado nem do pacote `@cnh/core-ui`. O shell, menu lateral, tabela, feedback e diálogos foram incorporados ao projeto preservando o padrão visual preto, branco e vermelho.

O backend usa .NET 8 LTS com Entity Framework Core 8.0.29 e ASP.NET Core OData 9.5.0. Mantenha o runtime atualizado no patch 8.0.x mais recente disponível no ambiente corporativo.

## Segurança

Os endpoints `display/connect` e `display/{deviceKey}/heartbeat` são públicos para permitir o primeiro cadastro. Em produção, recomenda-se restringi-los na rede corporativa e, se necessário, acrescentar um segredo de provisionamento por instalação.

## Compatibilidade da página apresentada

O player utiliza `iframe` para manter heartbeat e configuração ativos enquanto exibe a página. A página configurada deve permitir incorporação por meio de `Content-Security-Policy frame-ancestors` e não pode responder com um `X-Frame-Options` incompatível.
