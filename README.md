# TheFool
Описание интерфейса бота.
Бот представляет собой одностраничное веб-приложение, которое должно быть доступно через сеть по URL. Параметры передаются в строке вызова (GET), бот выдает ответ в виде XML структуры.
Get-параметры  при вызове бота
command	Обязательный параметр. Может принимать одно из следующих значений

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

Остальные параметры зависят от параметра command
ready	<без параметров>	
 init	cards
trump
turn	6 карт, изначальный расклад
Козырь
Первый ход (0 – бота, 1 – противника)
move	<без параметров>	
beat	card	Карта, которой пошел противник
dump	<без параметров>	
info	card	Карта, которой противник отбился
add	card	Карта, которую противник подбросил после того, как бот взял
card	card	Карта, которую сдали боту после окончания очередного раунда
done	result	Результат игры -1 – победа, 0 – ничья, 1 - поражение

В ответ бот должен выдать такую XML-структуру
<?xml version=”1.0”>
<response>
<status> </status>
<card> </card>
<error> </error>
</response>
status	0 если все ок, другое число – код ошибки
card	Карта в стандартном формате (для команд move, beat, dump) или пустая строка
error	Пустая строка если все ок, иначе - описание ошибки 

Ответ бота на переданную команду. 
ready	status = 0
status = 51	Готов к игре
Не готов играть
 init	status=0
status=61
status=62
status=63
status=64
status=65
status=66
status=67	Бот раздачу получил, готов играть
Не найден параметр cards с начальным раскладом
Не найден параметр trump c информацией о козыре
Не найден параметр turn c информацией о первом ходе
В параметре cards ошибки, карты не распознаны
В параметре trump ошибки, карта не распознана
В параметре turn ошибка, должен быть 0 или 1
Неверная начальная раздача (не шесть карт, повторяющиеся карты и проч.)
move	status=0	Бот сделал ход, XML-параметр <card> – ход бота
beat	status=0
status=61
status=64
status=71
status=72
status=83	Бот отбился или принял, XML-параметр <card> – ответ бота или NA - принял
Не найден параметр <card> с картой, которой пошли на нас
В параметре <card> ошибка, карта не распознана
Карта <card> уже встречалась в игре (в отбое, на игровом столе, у бота на руках)
Карту <card> подбросить нельзя (не тот номинал)
Еще одну карту подбросить нельзя, «не лезет»
dump	status=0	Бот подбросил карту, <card> – подброс бота (или NA, если подбросить нечего)
info	status=0
status=61
status=64
status=71
status=73	Информация о том, как отбился противник, принята к сведению
Не найден параметр <card> с картой, которой противник отбился 
В параметре <card> ошибка, карта не распознана
Карта <card> уже встречалась в игре (в отбое, на игровом столе, у бота на руках)
Картой <card> отбиться нельзя (не тот номинал)
add	status=0
status=61
status=64
status=71
status=72	Бот взял подброшенную карту
Не найден параметр <card> с картой, которую противник подбросил
В параметре <card> ошибка, карта не распознана
Карта <card> уже встречалась в игре (в отбое, на игровом столе, у бота на руках)
Карту <card> подбросить (не тот номинал)
card	status=0
status=61
status=64
status=71	Бот взял розданную карту
Не найден параметр <card> с картой, которую игровой движок раздал боту
В параметре <card> ошибка, карта не распознана
Карта <card> уже встречалась в игре (в отбое, на игровом столе, у бота на руках)
done	status=0	Бот согласен с результатом игры

Обозначения карт  - 2-символьная строка вида  <1..9><s|c|d|h>
Номинал – от 1 до 9 соответственно от шестерки до туза
Масть – s (spades) – пики, c (clubs) – трефы, d (diamonds) – бубны, h (hearts) - червы
Внутренняя структура объекта game
Поля
        List<string> cards;				Карты на руках у бота
        List<string> cardsgone;			Карты в отбое
        List<string> cardsgame;			Карты на игровом столе
        List<string> lastcardsgame;		Копия игрового стола после того, как cardsgame ушли в отбой

        string trump;				Козырь
        bool turn;					Очередность хода
        bool adoptmode;				Режим «взял, принимаю доброс»
