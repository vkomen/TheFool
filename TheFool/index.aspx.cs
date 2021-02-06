using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fool {

    public class game {

        List<string> cards;
        List<string> cardsgone;
        List<string> cardsgame;
        List<string> lastcardsgame;

        string trump;
        bool turn;
        bool adoptmode;

        //      КОНСТРУКТОР! Если узнаем из глобальной перменной, что игра началась - наполняем объект из переменных сессии, иначе создаем пустой новый
        public game() {
            if (HttpContext.Current.Session["start"] == null) {
                ClearGame();
            }
            else {
                PopGame();
            }
        }

        //      ОЧистка содержимого (старт новой игры)
        public void ClearGame() {
            cards = new List<string>();
            cardsgone = new List<string>();
            cardsgame = new List<string>();
            lastcardsgame = new List<string>();

            trump = "";
            turn = false;
            adoptmode = false;
            HttpContext.Current.Session["start"] = true;
        }

        //      Добавление карты на руки нам
        public void AddCard(string card) {
            cards.Add(card);
        }

        //      СБрос режима "принимаю до кучи"
        public void ResetAdoptMode() {
            adoptmode = false;
        }

        //      Запрос статуса режима "принимаю до кучи"
        public bool GetAdoptMode() {
            return adoptmode;
        }

        //      Добавление карты на игровое поле (по информации от противника, свою дообавляем по факту хода)
        public void AddCardToGame(string card) {
            cardsgame.Add(card);
        }

        //      Отправка игрового поля в отбой и очистка
        public void DumpCards() {
            for (int i = 0; i < cardsgame.Count; i++) {
                cardsgone.Add(cardsgame[i]);
            }
        cardsgame.Clear();
        }

        //      Выбор карты для доброса, когда противник уже взял
        public string MakeDump() {
            string card;

            for (int i = 0; i < lastcardsgame.Count; i++) {
                for (int j = 0; j < cards.Count; j++) {
                    if (cards[j][0] == lastcardsgame[i][0]) {
                    card = cards[j];
                    cards.RemoveAt(j);
                    return card;
                    }
                }
            }
            return "NA";
        }

        //      Выброр карты для нашего хода. 
        public string MakeMove() {
            string card;

            cards.Sort();
            if (cardsgame.Count == 0) {
                for (int i = 0; i < cards.Count; i++) { // Если первый ход в текущей сдаче - ищем минимальный не козырь
                if (trump[1] != cards[i][1]) {
                    card = cards[i];
                    cardsgame.Add(card);
                    cards.RemoveAt(i);
                    return card;
                    }
                }

                card = cards[0];                        // Если ничего не нашли - значит, у нас все козыри, ходим младшим
                cardsgame.Add(card);
                cards.RemoveAt(0);
                return card;
            }                                 
            else {                                      // Ищем, что можно подкинуть с тем же номиналом, что уже есть на поле
                for (int i = 0; i < cardsgame.Count; i++) {
                    for (int j = 0; j < cards.Count; j++) {
                        if (cards[j][0] == cardsgame[i][0]) {
                        card = cards[j];
                        cardsgame.Add(card);
                        cards.RemoveAt(j);
                        return card;
                        }
                    }
                }
            }

            DumpCards();                                // Если кидать больше нечего - отбой
            ToggleTurn();
            return "NA";
        }

        //      Выброр карты чтобы отбиться.
        public string MakeBeat(string card) {

            string beatcard;

            cards.Sort();
            cardsgame.Add(card);

            for (int i = 0; i < cards.Count; i++) {
                if ((card[1] == cards[i][1]) && (card[0] < cards[i][0])) {      // Если масть совпадает, а карта старше - это нам подходит
                    beatcard = cards[i];
                    cardsgame.Add(beatcard);
                    cards.RemoveAt(i);
                    return beatcard;
                }
            }

            if (card[1] == trump[1]) {                                          // Если ходили с козыря, и мы не смогли побить, то взяли
                lastcardsgame = new List<string>(cardsgame);                    // Запомнили, что было на поле - чтобы проверить потом, в кассу ли нам добрасывают !!!!!!!!!!!!!!!!!!!!!
                AcceptCards();
                adoptmode = true;
                return "NA";

        }

        for (int i = 0; i < cards.Count; i++) {                             // Иначе ищем младший козырь
                if (trump[1] == cards[i][1]) {
                    beatcard = cards[i];
                    cardsgame.Add(beatcard);
                    cards.RemoveAt(i);
                    return beatcard;
                }
            }

            lastcardsgame = new List<string>(cardsgame);                    // Запомнили, что было на поле - чтобы проверить потом, в кассу ли нам добрасывают !!!!!!!!!!!!!!!!!!!!!
            AcceptCards();                                                      // Бить нечем - взяли
            adoptmode = true;
            return "NA";
            }

        //      Количество карт у нас на руках
        public int Length() {
            return cards.Count;
        }

        //      Количество карт в игре
        public int GameLength() {
            return cardsgame.Count;
        }

        //      Установка козыря
        public void SetTrump(string tr) {
            trump = tr;
        }

        //      Проверяем, не было ли карты на руках, на поле, в отбое, у противника (if known)
        public bool CheckAbsence(string s) {
        return ((cards.IndexOf(s) == -1) && (cardsgone.IndexOf(s) == -1) && (cardsgame.IndexOf(s) == -1));// && (s != trump));
        }

        //      Проверяем, был ли на игровом поле такой номинал
        public bool CheckPresence(string s) {
            if (cardsgame.Count == 0) { return true; }
            for (int i = 0; i < cardsgame.Count; i++) {
                if (s[0] == cardsgame[i][0]) {
                    return true;
                }
            }
            return false;
            }

        //      Проверяем, был ли в тольуо что взятых картах такой номинал
        public bool CheckAdoptPresence(string s) {
            for (int i = 0; i < lastcardsgame.Count; i++) {
                if (s[0] == lastcardsgame[i][0]) {
                    return true;
                }
            }
            return false;
            }

        //  Проверка, что так можно бить  последнюю подброшенную
        public bool CheckBeatence(string beat) {
            if (cardsgame[cardsgame.Count - 1][1] == beat[1]) {             // Если совпадает масть
                if (cardsgame[cardsgame.Count - 1][0] < beat[0]) {          // И карта, которую бьют, младше - все ок
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return (beat[1] == trump[1]);                               // Масть не совпадает - тогда бить должен только козырь
            }
        }
            
        //      Мы приняли карты
        public void AcceptCards() {
            for (int i = 0; i < cardsgame.Count; i++) {
                cards.Add(cardsgame[i]);
            }
            cardsgame.Clear();
        }

        //      Противник принял карты - ТУТ МОЖНО ИХ ЗАПОМНИТЬ!!!
        public void AcceptEnemyCards() {
            lastcardsgame = new List<string>(cardsgame); // Переносим во внутреннюю переменную для добрасывания потом, когда вызовут
            cardsgame.Clear();
        }

        //      Установка параметра, чья очередь ходить
        public void SetTurn(bool t) {
            turn = t;
        }

        //      Опрос параметра, чья очередь ходить
        public bool OurTurn() {
            return turn;
        }

        //      Обращение параметра, чья очередь ходить
        public void ToggleTurn() {
            turn = !turn;
        }

        //      Выгрузка объекта в переменные сессии
        public void PushGame() {
            HttpContext.Current.Session["trump"] = trump;
            HttpContext.Current.Session["turn"] = turn;
            HttpContext.Current.Session["adoptmode"] = adoptmode;

            HttpContext.Current.Session["cards"] = string.Join(",", cards.ToArray());
            HttpContext.Current.Session["cardsgone"] = string.Join(",", cardsgone.ToArray());
            HttpContext.Current.Session["cardsgame"] = string.Join(",", cardsgame.ToArray());
            HttpContext.Current.Session["lastcardsgame"] = string.Join(",", lastcardsgame.ToArray());
        }

        //      Загрузка объекта из переменных сессии
        private void PopGame() {
            trump = (string)HttpContext.Current.Session["trump"];
            turn = (bool)HttpContext.Current.Session["turn"];
            adoptmode = (bool)HttpContext.Current.Session["adoptmode"];

            cards = new List<string>();
            cardsgone = new List<string>();
            cardsgame = new List<string>();
            lastcardsgame = new List<string>();

            char[] separator = { ',' };
            if ((string)HttpContext.Current.Session["cards"] != "") { cards.AddRange(((string)HttpContext.Current.Session["cards"]).Split(separator)); }
            if ((string)HttpContext.Current.Session["cardsgone"] != "") { cardsgone.AddRange(((string)HttpContext.Current.Session["cardsgone"]).Split(separator)); }
            if ((string)HttpContext.Current.Session["cardsgame"] != "") { cardsgame.AddRange(((string)HttpContext.Current.Session["cardsgame"]).Split(separator)); }
            if ((string)HttpContext.Current.Session["lastcardsgame"] != "") { lastcardsgame.AddRange(((string)HttpContext.Current.Session["lastcardsgame"]).Split(separator)); }
            }
        }

    public partial class index : System.Web.UI.Page {
        public const string xml = "<?xml version=\"1.0\"?><response><status>@1</status><card>@2</card><error>@3</error></response>";
        public game TheGame;
        public string answer;
        public bool endcondition = false;

        protected void Page_Load(object sender, EventArgs e) {
        TheGame = new game();

        if (Request.QueryString["command"] == null) {
            answer = xml.Replace("@1", "50").Replace("@2", "").Replace("@3", "No command provided"); 
            Session.Abandon();
            }
        else {
            if ((Request.QueryString["command"].ToLower()) != "add") { TheGame.ResetAdoptMode(); }

            switch (Request.QueryString["command"].ToLower()) {
                case "ready":                                                                   // Подтверждаем, что готовы. ЕСЛИ НЕ ГОТОВЫ - КОД 51!!!
                    answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                    TheGame.ClearGame();                                                        // Чистим поля на всякий случай (если прошлая игра ошибочно завершилась заранее, без обнуления сессии)
                    break;

                case "init":                                                                    // Проверяем розданные карты и наполняем внутренние структуры
                    if (Request.QueryString["cards"] == null) {
                        answer = xml.Replace("@1", "61").Replace("@2", "").Replace("@3", "Parameter 'cards' not provided");
                        endcondition = true;
                        break;
                    }

                    if (Request.QueryString["trump"] == null) {
                        answer = xml.Replace("@1", "62").Replace("@2", "").Replace("@3", "Parameter 'trump' not provided");
                        endcondition = true;
                        break;
                    }

                    if (!CheckCards(Request.QueryString["cards"].ToLower(), Request.QueryString["trump"].ToLower())) {           
                        answer = xml.Replace("@1", "67").Replace("@2", "").Replace("@3", "Incorrect initial giveout");           
                        endcondition = true;
                        break;
                    }

                    if (Request.QueryString["turn"] == null) {
                        answer = xml.Replace("@1", "63").Replace("@2", "").Replace("@3", "Parameter 'turn' not provided");
                        endcondition = true;
                        break;
                    }

                    if ((Request.QueryString["turn"] == "0") || (Request.QueryString["turn"] == "1")) {
                        TheGame.SetTurn(Request.QueryString["turn"] == "1");
                        answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                    }
                    else {
                        answer = xml.Replace("@1", "64").Replace("@2", "").Replace("@3", "Parameter 'turn' - incorrect value " + Request.QueryString["turn"] + ". Should be 0 or 1");
                        endcondition = true;
                    }   
                    break;

                case "move":
                    // Если нам предлагают ходить, а был не наш ход -  значит, мы на предыдущем шаге отбились!
                    if (!TheGame.OurTurn()) {
                        TheGame.DumpCards();
                        TheGame.ToggleTurn();
                    }

                    answer = xml.Replace("@1", "0").Replace("@2", TheGame.MakeMove()).Replace("@3", "");
                    break;

                case "info":                                                                    // Информация о том, чем побился противник в ответ на наш код
                    if (Request.QueryString["card"] == "NA") {                                  // Принял противник карты ВАЖНО!!! Мы знаем часть его карт тут!!!
                        TheGame.AcceptEnemyCards();
                        answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                        break;
                    }

                    answer = CheckCardParameter();
                    if (answer !="") {
                        endcondition = true;
                        break;  }

                    if (!TheGame.CheckBeatence(Request.QueryString["card"])){
                        answer = xml.Replace("@1", "73").Replace("@2", "").Replace("@3", "This card will not beat - " + Request.QueryString["card"] );
                        endcondition = true;
                        break;
                    }

                    TheGame.AddCardToGame(Request.QueryString["card"]);
                    answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                    break;

                case "beat":
                    // Если нам предлагают отбиться, а был наш ход -  значит, на предыдущем шаге противник отбился!
                    if (TheGame.OurTurn()) {
                        TheGame.DumpCards();
                        TheGame.ToggleTurn();
                    }

                    answer = CheckCardParameter();
                    if (answer != "") {
                        endcondition = true;
                        break; }

                    // Проверка, что нам кинули не больше 6 карт (или сколько у нас на руках было, если меньше)
                    if ((TheGame.Length() == 0) || (TheGame.GameLength() == 12)) {
                        answer = xml.Replace("@1", "83").Replace("@2", "").Replace("@3", "Too many cards for me!!");
                        endcondition = true;
                        break;
                    }

                    // Проверка, что на поле были карты такого номинала и их можно подбрасывать (если карта первая - то всегда Ок)
                    if (!TheGame.CheckPresence(Request.QueryString["card"])) {
                        answer = xml.Replace("@1", "72").Replace("@2", "").Replace("@3", "This card will not do! - " + Request.QueryString["card"]);             
                        endcondition = true;
                        break;
                    }

                    answer = xml.Replace("@1", "0").Replace("@2", TheGame.MakeBeat(Request.QueryString["card"])).Replace("@3", "");
                    break;

                // Добрасывание карт НАМ после того, как МЫ приняли
                case "add":
                    if (!TheGame.GetAdoptMode()) {
                        answer = xml.Replace("@1", "81").Replace("@2", "").Replace("@3", "I don't adopt any cards now!");                                        
                        endcondition = true;
                        break;
                    }

                    answer = CheckCardParameter();
                    if (answer != "") {
                        endcondition = true;
                        break; }

                    // Проверка, что в только что взятых картах есть такой номинал
                    if (!TheGame.CheckAdoptPresence(Request.QueryString["card"])) {
                        answer = xml.Replace("@1", "72").Replace("@2", "").Replace("@3", "This card will not do! - " + Request.QueryString["card"]);             
                        endcondition = true;
                        break;
                    }

                    TheGame.AddCard(Request.QueryString["card"]);
                    answer = xml.Replace("@1", "0").Replace("@2", "NA").Replace("@3", "");
                    break;

                // Предложение добросить карты после того, как противник взял
                case "dump":
                    answer = xml.Replace("@1", "0").Replace("@2", TheGame.MakeDump()).Replace("@3", "");
                    break;

                case "card":                                                                    // Проверяем дорозданную карту и наполняем внутренние структуры
                    if (TheGame.Length() == 6) {                                                // Попытка сдать 7ю карту. ВАЖНО! ПРОВЕРИТЬ, ЧТО РАЗДАЕТСЯ ПОРОВНУ В КОНЦЕ ИГРЫ!!!
                        answer = xml.Replace("@1", "82").Replace("@2", "").Replace("@3", "7th card will not fit");
                        endcondition = true;
                        break;
                    }
                    answer = CheckCardParameter();
                    if (answer != "") {
                        endcondition = true;
                        break; }

                    TheGame.AddCard(Request.QueryString["card"]);
                    answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                    break;

                case "done":                                                                    // Соглашаемся с результатом (ИЛИ НЕТ)
                    answer = xml.Replace("@1", "0").Replace("@2", "").Replace("@3", "");
                    endcondition = true;
                    break;

                default:
                    answer = xml.Replace("@1", "49").Replace("@2", "").Replace("@3", "Unknown command");
                    endcondition = true;
                    break;
            }
        }
        if (endcondition) { Session.Abandon(); }
        else { TheGame.PushGame(); }
        }

        //      Проверка первоначальной раздачи
        public bool CheckCards(string cards, string trump) {
            if (cards.Length != 12) {
                return false;
            }

            for (int i = 0; i < 12; i = i + 2) {
                if (CheckCard(cards.Substring(i, 2))) {
                    TheGame.AddCard(cards.Substring(i, 2));
                }
                else {
                    return false;
                }
            }

            if (!CheckCard(trump)) {
                return false;
            }

            TheGame.SetTrump(trump);
            return true;
            }

        //      Проверка правильности карты
        public bool CheckCard(string card) {
            if (card.Length == 2) {
                return ("scdh".IndexOf(card[1]) >= 0) && ("123456789".IndexOf(card[0]) >= 0);
                }
            else {
                return false;
            }
        }

        //      Проверка параметра card 
        public string CheckCardParameter() {
            if (Request.QueryString["card"] == null) {
                return xml.Replace("@1", "61").Replace("@2", "").Replace("@3", "Parameter 'card' not provided");
            }   

            if (CheckCard(Request.QueryString["card"])) {                               // Проверка карты на правильность и недублирование
                if (!TheGame.CheckAbsence(Request.QueryString["card"])) {               // Проверка карты на остутствие на руках
                    return xml.Replace("@1", "71").Replace("@2", "").Replace("@3", "Card " + Request.QueryString["card"] + " already was in game");
                }
            }
            else {
                return xml.Replace("@1", "64").Replace("@2", "").Replace("@3", "Incorrect Card " + Request.QueryString["card"] + " tried to beat");
            }
            return "";
        }
    }
}