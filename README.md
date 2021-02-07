# TheFool - Описание интерфейса бота.
Бот представляет собой одностраничное веб-приложение, которое должно быть доступно через интернет по URL. Параметры передаются в строке вызова (GET), бот выдает ответ в виде XML структуры.

**command**	- обязательный параметр. Может принимать одно из следующих значений

| command | расшифровка |
| --- | --- |
| ready	| Инициализация бота, подтверждение его готовности к игре | 
| init	| Передача боту его карт в начале игры, информация о козыре и первом ходе  | 
| move	| Предложение боту сделать ход | 
| beat	| Предложение боту отбить карту | 
| dump	| Предложение боту добросить карту после того, как противник взял | 
| info	| Информация о том, как противник отбился | 
| add	| Доброс карты боту после того, как бот не смог отбиться и взял | 
| card	| Сдача карты боту после очередного раунда | 
| done	| Игра завершена | 

Остальные параметры зависят от параметра **command**

| command | параметр | расшифровка |
| --- | --- | --- |
| ready	 | <без параметров>	 | - | 
|  init	 | cards | 6 карт, изначальный расклад |
|  | trump | Карта - козырь |
|   | turn	 | Первый ход (0 – бота, 1 – противника) | 
| move	| <без параметров>	| - | 
| beat	| card	| Карта, которой пошел противник | 
| dump	| <без параметров>	| - | 
| info	| card	| Карта, которой противник отбился |  
| add	| card	| Карта, которую противник подбросил после того, как бот взял | 
| card	| card	| Карта, которую сдали боту после окончания очередного раунда | 
| done	| result	| Результат игры: -1 – победа, 0 – ничья, 1 - поражение | 

**Обозначения карт:**

2-символьная строка вида  <1..9>< s | c | d | h >

Номинал – от 1 до 9, соответственно от шестерки до туза

Масть – s (spades) – пики, c (clubs) – трефы, d (diamonds) – бубны, h (hearts) - червы

**Примеры строк-команд:**

http://witbot.ru/index.aspx?command=init&cards=3c3h2d4h6d5c&trump=1s&turn=0 (передача боту изначального расклада, козыря и информации о первом ходе) 

http://witbot.ru/index.aspx?command=beat&card=8d  (ход на бота картой 8d - королем бубен - с предложением отбиться)

**В ответ бот должен выдать такую XML-структуру:**
```<?xml version=”1.0”>
<response>
    <status>...</status>
    <card>...</card>
    <error>...</error>
</response>
```

| XML-элемент | Значение |
| --- | --- |
| status	| 0 если все ок, другое число – код ошибки | 
| card	| Карта в стандартном формате (для команд move, beat, dump) или пустая строка | 
| error	| Пустая строка если все ок, иначе - описание ошибки |  


**Ответ бота на переданную команду:**

| command | status | расшифровка |
| --- | --- |--- |
| ready | 0 | Готов к игре | 
|    | 51 | Не готов играть | 
|  init | 0 | Бот раздачу получил, готов играть | 
|    | 61 | Не найден параметр **cards** с начальным раскладом | 
|   | 62 | Не найден параметр **trump** c информацией о козыре | 
|   | 63 | Не найден параметр **turn** c информацией о первом ходе | 
|   | 64 | В параметре **cards** ошибки, карты не распознаны | 
|   | 65 | В параметре **trump** ошибки, карта не распознана | 
|   | 66 | В параметре **turn** ошибка, должен быть 0 или 1 | 
|   | 67 | Неверная начальная раздача (не шесть карт, повторяющиеся карты и проч.) | 
| move | 0 | Бот сделал ход, XML-элемент **card** – ход бота | 
| beat | 0 | Бот отбился или принял, XML-элемент **card** – ответ бота или NA - принял | 
|   | 61 | Не найден элемент **card** с картой, которой пошли на нас | 
|   | 64 | В элементе **card** ошибка, карта не распознана | 
|   | 71 | Карта **card** уже встречалась в игре (в отбое, на игровом столе, у бота на руках) | 
|   | 72 | Карту **card** подбросить нельзя (не тот номинал) | 
|   | 83 | Еще одну карту подбросить нельзя, «не лезет» | 
| dump | 0 | Бот подбросил карту, **card** – подброс бота (или NA, если подбросить нечего) | 
| info | 0 | Информация о том, как отбился противник, принята к сведению | 
|   | 61 | Не найден элемент **card** с картой, которой противник отбился | 
|   | 64 | В элементе **card** ошибка, карта не распознана | 
|   | 71 | Карта **card** уже встречалась в игре (в отбое, на игровом столе, у бота на руках) | 
|   | 73 | Картой **card** отбиться нельзя (не тот номинал) | 
| add | 0 | Бот взял подброшенную карту | 
|   | 61 | Не найден элемент **card** с картой, которую противник подбросил | 
|   | 64 | В элементе **card** ошибка, карта не распознана | 
|   | 71 | Карта **card** уже встречалась в игре (в отбое, на игровом столе, у бота на руках) | 
|   | 72 | Карту **card** подбросить (не тот номинал) | 
| card | 0 | Бот взял розданную карту | 
|   | 61 | Не найден параметр **card** с картой, которую игровой движок раздал боту | 
|   | 64 | В параметре **card** ошибка, карта не распознана | 
|   | 71 | Карта **card** уже встречалась в игре (в отбое, на игровом столе, у бота на руках) | 
| done | 0 | Бот согласен с результатом игры | 



Внутренняя структура (поля) объекта game

```     
        List<string> cards; 			Карты на руках у бота
        List<string> cardsgone; 		Карты в отбое
        List<string> cardsgame;			Карты на игровом столе
        List<string> lastcardsgame;		Копия игрового стола после того, как cardsgame ушли в отбой

        string trump;			        Козырь
        bool turn;				        Очередность хода
        bool adoptmode;			        Режим «взял, принимаю доброс»
```
