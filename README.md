-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

MICRO-ONDAS DIGITAL - Pojeto C# em .NET 5.0.

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Nível 1

1. Interface onde é possível informar um tempo e a potência.
	a. Informar os números > input por teclado.
	b. Interface em Razor Pages

2. Método para iniciar o aquecimento, sendo parametrizável o tempo e a potência.
	a. O tempo máximo é 2 minutos e o mínimo é 1 segundo.
	b. A potência varia de 1 a 10 assumindo 10 como padrão, caso não informada.
	c. Caso o usuário informe um tempo maior que 60 segundos e menor que 100 segundos o programa converte o tempo em minutos

3. Validações de operação:
	a. Caso o tempo seja informado fora dos limites definidos (menor que 1 ou maior que 2 minutos) é exibida uma mensagem solicitando um tempo valido.
	b. Caso a potência for informada com um valor menor que 0 ou maior que 10 o sistema exibe mensagem.	 
	c. Em caso de potência não informada, será inserido em tela o valor 10 como padrão.

4. Possibilita o "início rápido":
        a. Se define por apertar o botão de INICIAR sem informar tempo ou potência. Neste caso o programa iniciará o aquecimento com a potência 10 e o tempo será 30 segundos.

5. Acréscimo de tempo durante o aquecimento: 
	a. Caso o usuário pressione o botão de INICIAR aquecimento com um aquecimento já em execução o programa acrescenta 30 segundos no tempo que estiver faltando.

6. String informativa do processo de aquecimento: 
	a. Durante o processo de aquecimento uma string de label é exibida e atualizada em tela, para dar a noção de processamento.

	b. A string tem o seguinte formato: o caractere usado será o "." e a quantidade de caracteres por segundo vai variar de acordo com a potência definida. 
           Exemplo: tempo de 10 segundos com a potência 1 o resultado final será:
	   ". . . . . . . . . .". tempo 5 com potência 3: "... ... ... ... ...". Assim por diante.

	b. Ao final do processo no final dessa string é concatenado a frase "Aquecimento concluído".

7. Pausa / cancelamento de aquecimento:

	a. O programa deverá ter um botão único para pausa / cancelamento. 
           Se o aquecimento estiver em andamento e este botão for acionado o programa pausará o aquecimento. 
           Caso o botão de iniciar seja acionado nesta situação o aquecimento prosseguirá do ponto onde parou.

	b. Caso o botão de pausa / cancelamento seja acionado com o aquecimento pausado o aquecimento será cancelado, limpando todas as informações em tela e interrompendo o aquecimento.

	c. Caso o botão de pausa / cancelamento seja acionado antes do aquecimento ser iniciado, as informações de tempo e potência serão limpadas.

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Nível 2

1. Programas de aquecimento pré-definidos

	a. O programa disponibiliza 5 programas de aquecimento pré-definidos. 
           Programas de aquecimento tem as seguintes informações: 
		Nome do programa
                Alimento
                Tempo
                Potência
                String de aquecimento 
	        Instruções complementares.

	b. Cada programa deverá utilizar uma string de aquecimento diferenciada.
           Não é permitido repetir entre outros programas e não poderá ser o caractere padrão ".".

	c. Os programas pré-definidos não podem ser alterados ou excluídos.
	
	d. Ao selecionar um programa, os dados referentes a potência e tempo são ser preenchidos de forma automática não permitindo a troca das informações.

	e. Para programas pré-definidos não é permitido o acrescimento de tempo.

	f. As ações de pausa e cancelamento são permitidas.

Programas de aquecimento pré-definidos:

1. Nome: Pipoca
Alimento: Pipoca (de micro-ondas)
tempo: 3 minutos
potência: 7
Instruções: Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.

2. Nome: Leite
Alimento: Leite
tempo: 5 minutos
potência: 5
Instruções: Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.

3. Carnes de boi
Alimento: Carne em pedaço ou fatias
tempo: 14 minutos
potência 4
Instruções: Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.

4. Frango
Alimento: Frango (qualquer corte)
tempo: 8 minutos
potência: 7
Instruções: Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.

5. Feijão
Alimento: Feijão congelado
tempo: 8 minutos
potência 9
Instruções: Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Nível 3

1. Cadastro de programas de aquecimento customizados:
	a. O programa permite o cadastro de programas de aquecimento customizados.
           As informações de nome do programa, alimento, potência, caractere de aquecimento e tempo deverão obrigatoriamente ser preenchidos.

	b. As instruções de uso têm o preenchimento opcional.

	c. O caractere de aquecimento deverá ser validado e não poderá se repetir com qualquer programa de aquecimento ou ao caractere padrão "."

	d. Os programas customizados deverão ser exibidos juntamente aos programas pré-definidos, porém a fonte dele deverá ser em itálico para diferenciar.

	e. A persistência dos programas customizados são feitos em arquivo JSON, localizado na mesma pasta que a solução.

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Nível 4 - não implementado
