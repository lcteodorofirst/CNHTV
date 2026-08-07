# CNH TV Player para LG webOS

Aplicativo empacotado que apresenta conteúdo web em tela cheia. Por padrão, registra automaticamente a TV, envia heartbeat e utiliza a URL definida pelo servidor CNH TV.

## Funcionalidades da versão 1.0.0

- Tela de configurações acessível pelo ícone recolhido na lateral direita ou pelo botão vermelho do controle remoto.
- O ícone se expande somente ao receber o ponteiro ou o foco, preservando a visualização da apresentação.
- Integração com o servidor habilitada por padrão.
- Endereço da API configurável e persistido na TV.
- Modo local disponível quando a integração estiver desabilitada.
- No modo local, URL da apresentação e refresh automático configuráveis.
- Opção para restaurar todas as configurações padrão.

## Configurações padrão

- Integração com o servidor: habilitada.
- Serviço: `https://itm.curitiba.cnh.com:7001/cnhtv/api`.
- Refresh local: desabilitado.
- Intervalo local sugerido: 5 minutos.
- Cor do App Tile: `#010000`, igual ao fundo do ícone enviado ao LG Seller Lounge.

As preferências são armazenadas no `localStorage` do aplicativo e permanecem após a TV ou o aplicativo serem reiniciados.

## Desenvolvimento e instalação

1. Instale a extensão **webOS Studio** no Visual Studio Code.
2. Abra a pasta `WebOs/CnhtvPlayer` como projeto.
3. Cadastre a TV em modo desenvolvedor ou utilize o simulador webOS.
4. Gere o pacote IPK e instale no equipamento.

O endereço padrão do servidor está na constante `DEFAULT_API_BASE_URL` de `app.js` e pode ser substituído na tela de configurações.

## Páginas externas

A URL é apresentada em `iframe` e precisa permitir incorporação, sem bloqueio por `X-Frame-Options` ou pela diretiva `frame-ancestors` de Content Security Policy.
