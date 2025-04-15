using System.Collections.Generic;
using UnityEngine;

namespace ph.Achievements {
    public static class AchievementDatabase {
        public static readonly List<Achievement> achievements;

        static AchievementDatabase() {
            Sprite bronzeLaurel = Resources.Load<Sprite>("Achievements/bronze");
            Sprite bronzeCup = Resources.Load<Sprite>("Achievements/bronzeCup");
            Sprite silverLaurel = Resources.Load<Sprite>("Achievements/silver");
            Sprite silverCup = Resources.Load<Sprite>("Achievements/silverCup");
            Sprite goldLaurel = Resources.Load<Sprite>("Achievements/golden");
            Sprite goldCup = Resources.Load<Sprite>("Achievements/goldenCup");

            achievements = new List<Achievement> {
                // Quiz Achievements
                new Achievement {
    id = "quiz10",
    type = AchievementType.Quiz,
    threshold = 10,
    icon = bronzeCup,
    titles = new Dictionary<string, string> {
        { "en", "Quiz Rookie" },
        { "pl", "Quizowy Żółtodziób" },
        { "es", "Novato del Quiz" },
        { "fr", "Bleu du Quiz" },
        { "de", "Quiz-Anfänger" },
        { "it", "Principiante del Quiz" },
        { "ru", "Новичок викторин" },
        { "pt-BR", "Iniciante do Quiz" },
        { "ko", "퀴즈 초보자" },
        { "zh-Hans", "测验新手" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Correctly answer 10 quiz questions." },
        { "pl", "Poprawnie odpowiedz na 10 pytań quizowych." },
        { "es", "Responde correctamente a 10 preguntas del quiz." },
        { "fr", "Répondez correctement à 10 questions du quiz." },
        { "de", "Beantworte 10 Quizfragen richtig." },
        { "it", "Rispondi correttamente a 10 domande del quiz." },
        { "ru", "Правильно ответьте на 10 вопросов викторины." },
        { "pt-BR", "Responda corretamente a 10 perguntas do quiz." },
        { "ko", "퀴즈 질문 10개를 정확히 답하세요." },
        { "zh-Hans", "正确回答10个测验问题。" }
    }
},
                new Achievement {
    id = "quiz100",
    type = AchievementType.Quiz,
    threshold = 100,
    icon = silverCup,
    titles = new Dictionary<string, string> {
        { "en", "Quiz Veteran" },
        { "pl", "Weteran quizów" },
        { "es", "Veterano del Quiz" },
        { "fr", "Vétéran du Quiz" },
        { "de", "Quiz-Veteran" },
        { "it", "Veterano del Quiz" },
        { "ru", "Ветеран викторин" },
        { "pt-BR", "Veterano do Quiz" },
        { "ko", "퀴즈 베테랑" },
        { "zh-Hans", "测验老手" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Correctly answer 100 quiz questions. You clearly like pain." },
        { "pl", "Poprawnie odpowiedz na 100 pytań quizowych. Lubisz się męczyć, co?" },
        { "es", "Responde correctamente a 100 preguntas. ¿Te gusta sufrir?" },
        { "fr", "Répondez correctement à 100 questions. Vous aimez souffrir, non ?" },
        { "de", "Beantworte 100 Fragen korrekt. Du magst wohl den Schmerz." },
        { "it", "Rispondi correttamente a 100 domande. Ti piace soffrire?" },
        { "ru", "Ответьте правильно на 100 вопросов. Любите страдать, да?" },
        { "pt-BR", "Responda corretamente a 100 perguntas. Gosta de sofrer, né?" },
        { "ko", "퀴즈 100문제를 맞히세요. 고통이 좋으신가요?" },
        { "zh-Hans", "正确回答100个问题。你这是自找苦吃吧？" }
    }
},
                new Achievement {
    id = "quiz500",
    type = AchievementType.Quiz,
    threshold = 500,
    icon = goldCup,
    titles = new Dictionary<string, string> {
        { "en", "The Chosen Nerd" },
        { "pl", "Wybrany nerd" },
        { "es", "El Nerd Elegido" },
        { "fr", "Le Nerd Élu" },
        { "de", "Der Auserwählte Nerd" },
        { "it", "Il Nerd Prescelto" },
        { "ru", "Избранный ботан" },
        { "pt-BR", "O Nerd Escolhido" },
        { "ko", "선택받은 너드" },
        { "zh-Hans", "被选中的书呆子" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Answer 500 quiz questions correctly. There is no meme. Take your diploma." },
        { "pl", "Odpowiedz poprawnie na 500 pytań. Nie ma mema. Bierz dyplom." },
        { "es", "Responde correctamente 500 preguntas. No hay meme. Toma tu diploma." },
        { "fr", "Répondez correctement à 500 questions. Pas de mème. Prenez votre diplôme." },
        { "de", "Beantworte 500 Fragen richtig. Kein Meme. Nimm dein Diplom." },
        { "it", "Rispondi correttamente a 500 domande. Niente meme. Prendi il diploma." },
        { "ru", "Ответь на 500 вопросов. Без мемов. Забирай диплом." },
        { "pt-BR", "Responda corretamente 500 perguntas. Sem meme. Pegue seu diploma." },
        { "ko", "500문제를 맞히세요. 밈은 없습니다. 졸업장을 받으세요." },
        { "zh-Hans", "正确回答500个问题。没有表情包，拿好你的文凭。" }
    }
},

                // Mail Achievements
                new Achievement {
    id = "mail10",
    type = AchievementType.Mail,
    threshold = 10,
    icon = bronzeCup,
    titles = new Dictionary<string, string> {
        { "en", "Spamurai Jack" },
        { "pl", "Spamuraj Jack" },
        { "es", "Spamurái Jack" },
        { "fr", "Spamouraï Jack" },
        { "de", "Spamurai Jack" },
        { "it", "Spamurai Jack" },
        { "ru", "Спамурай Джек" },
        { "pt-BR", "Spamurai Jack" },
        { "ko", "스팸무사 잭" },
        { "zh-Hans", "垃圾武士杰克" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Correctly mark 10 emails. Just a warm-up, samurai." },
        { "pl", "Poprawnie oznacz 10 maili. To tylko rozgrzewka, samuraju." },
        { "es", "Marca correctamente 10 correos. Solo un calentamiento, samurái." },
        { "fr", "Marquez correctement 10 e-mails. Un simple échauffement, samouraï." },
        { "de", "Markiere 10 E-Mails richtig. Nur ein Aufwärmen, Samurai." },
        { "it", "Segna correttamente 10 email. Solo un riscaldamento, samurai." },
        { "ru", "Отметь 10 писем правильно. Просто разминка, самурай." },
        { "pt-BR", "Marque corretamente 10 e-mails. Só um aquecimento, samurai." },
        { "ko", "이메일 10개를 정확히 표시하세요. 준비운동이에요, 사무라이." },
        { "zh-Hans", "正确标记10封邮件。只是热身，武士。" }
    }
},
                new Achievement {
    id = "mail100",
    type = AchievementType.Mail,
    threshold = 100,
    icon = silverCup,
    titles = new Dictionary<string, string> {
        { "en", "Click Like a Pro" },
        { "pl", "Klikacz Profesjonalista" },
        { "es", "Haz clic como un profesional" },
        { "fr", "Clique comme un pro" },
        { "de", "Klick wie ein Profi" },
        { "it", "Clicca come un professionista" },
        { "ru", "Кликай как профи" },
        { "pt-BR", "Clique como um profissional" },
        { "ko", "프로처럼 클릭하세요" },
        { "zh-Hans", "像专业人士一样点击" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Correctly mark 100 emails. Your mouse fears you now." },
        { "pl", "Poprawnie oznacz 100 maili. Twoja myszka zaczyna się Ciebie bać." },
        { "es", "Marca correctamente 100 correos. Tu ratón ya te teme." },
        { "fr", "Marque correctement 100 e-mails. Ta souris commence à avoir peur." },
        { "de", "Markiere 100 E-Mails richtig. Deine Maus fürchtet dich jetzt." },
        { "it", "Segna correttamente 100 email. Il tuo mouse ora ti teme." },
        { "ru", "Отметь 100 писем правильно. Твоя мышка боится тебя." },
        { "pt-BR", "Marque corretamente 100 e-mails. Seu mouse já te respeita." },
        { "ko", "이메일 100개를 정확히 표시하세요. 마우스가 당신을 두려워합니다." },
        { "zh-Hans", "正确标记100封邮件。你的鼠标开始害怕你了。" }
    }
},
                new Achievement {
    id = "mail300",
    type = AchievementType.Mail,
    threshold = 300,
    icon = goldCup,
    titles = new Dictionary<string, string> {
        { "en", "The Inboxinator" },
        { "pl", "Skrzynkokinator" },
        { "es", "El Buzoneador" },
        { "fr", "L’Inboxinator" },
        { "de", "Der Posteinganginator" },
        { "it", "L’Inboxinatore" },
        { "ru", "Инбоксинатор" },
        { "pt-BR", "O Caixa-destruidor" },
        { "ko", "인박스 종결자" },
        { "zh-Hans", "收件箱终结者" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Correctly mark 300 emails. You don’t check the inbox – the inbox checks you." },
        { "pl", "Poprawnie oznacz 300 maili. To nie Ty sprawdzasz skrzynkę – to ona sprawdza Ciebie." },
        { "es", "Marca correctamente 300 correos. Tú no revisas el buzón, él te revisa a ti." },
        { "fr", "Marque correctement 300 e-mails. Ce n’est plus toi qui consultes ta boîte, c’est elle qui te consulte." },
        { "de", "Markiere 300 E-Mails korrekt. Du checkst nicht den Posteingang – der Posteingang checkt dich." },
        { "it", "Segna correttamente 300 email. Non sei tu a controllare la casella, è lei che controlla te." },
        { "ru", "Отметь 300 писем. Это не ты проверяешь почту — она проверяет тебя." },
        { "pt-BR", "Marque corretamente 300 e-mails. Você não confere a caixa de entrada – ela confere você." },
        { "ko", "이메일 300개를 정확히 표시하세요. 당신이 인박스를 확인하는 게 아닙니다 – 인박스가 당신을 확인하죠." },
        { "zh-Hans", "正确标记300封邮件。不是你在查看收件箱，而是它在盯着你。" }
    }
},

                // Level Achievements
                new Achievement {
    id = "level5",
    type = AchievementType.Level,
    threshold = 5,
    icon = bronzeLaurel,
    titles = new Dictionary<string, string> {
        { "en", "Coffee-Powered Intern" },
        { "pl", "Stażyściarz na kofeinie" },
        { "es", "Becario con cafeína" },
        { "fr", "Stagiaire caféiné" },
        { "de", "Kaffeepraktikant" },
        { "it", "Stagista caffeinomane" },
        { "ru", "Кофейный стажёр" },
        { "pt-BR", "Estagiário movido a café" },
        { "ko", "커피로 구동되는 인턴" },
        { "zh-Hans", "咖啡驱动的实习生" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Reach level 5. You’ve officially earned your first mug of company coffee." },
        { "pl", "Osiągnij poziom 5. Oficjalnie zasługujesz na pierwszy firmowy kubek z kawą." },
        { "es", "Alcanza el nivel 5. Oficialmente mereces tu primera taza de café de oficina." },
        { "fr", "Atteins le niveau 5. Tu mérites officiellement ta première tasse de café d’entreprise." },
        { "de", "Erreiche Level 5. Du hast dir offiziell den ersten Firmenkaffee verdient." },
        { "it", "Raggiungi il livello 5. Hai ufficialmente guadagnato la tua prima tazza di caffè aziendale." },
        { "ru", "Достигни уровня 5. Ты официально заслужил первую кружку офисного кофе." },
        { "pt-BR", "Chegue ao nível 5. Você oficialmente ganhou sua primeira caneca de café da firma." },
        { "ko", "레벨 5에 도달하세요. 이제 공식적으로 회사 커피 한 잔 받을 자격이 있습니다." },
        { "zh-Hans", "达到等级5。你终于配得上你的第一杯公司咖啡了。" }
    }
},
                new Achievement {
    id = "level10",
    type = AchievementType.Level,
    threshold = 10,
    icon = silverLaurel,
    titles = new Dictionary<string, string> {
        { "en", "Certified Email Clicker" },
        { "pl", "Certyfikowany klikacz maili" },
        { "es", "Clickeador de correos certificado" },
        { "fr", "Cliqueur d’emails certifié" },
        { "de", "Zertifizierter Mail-Klicker" },
        { "it", "Cliccatore di email certificato" },
        { "ru", "Сертифицированный кликатель писем" },
        { "pt-BR", "Clicador de e-mails certificado" },
        { "ko", "인증된 이메일 클릭러" },
        { "zh-Hans", "认证邮件点击师" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Reach level 10. HR is still unsure what your job actually is." },
        { "pl", "Osiągnij poziom 10. HR wciąż nie wie, czym właściwie się zajmujesz." },
        { "es", "Alcanza el nivel 10. Recursos Humanos aún no sabe qué haces exactamente." },
        { "fr", "Atteins le niveau 10. Les RH ne savent toujours pas ce que tu fais." },
        { "de", "Erreiche Level 10. Die Personalabteilung weiß immer noch nicht, was du genau machst." },
        { "it", "Raggiungi il livello 10. Le risorse umane non hanno ancora capito cosa fai." },
        { "ru", "Достигни уровня 10. Отдел кадров до сих пор не понял, чем ты занимаешься." },
        { "pt-BR", "Chegue ao nível 10. O RH ainda não sabe qual é exatamente o seu trabalho." },
        { "ko", "레벨 10에 도달하세요. 인사팀은 여전히 당신의 업무를 이해하지 못함." },
        { "zh-Hans", "达到等级10。人资部门仍然搞不清你到底是干啥的。" }
    }
},
                new Achievement {
    id = "level40",
    type = AchievementType.Level,
    threshold = 40,
    icon = goldLaurel,
    titles = new Dictionary<string, string> {
        { "en", "The Firewall Whisperer" },
        { "pl", "Szeptacz zapory sieciowej" },
        { "es", "El susurrador del cortafuegos" },
        { "fr", "Le chuchoteur de pare-feu" },
        { "de", "Der Firewalls-Flüsterer" },
        { "it", "Il sussurratore di firewall" },
        { "ru", "Шептун файрволлов" },
        { "pt-BR", "O sussurrador de firewalls" },
        { "ko", "방화벽의 속삭임자" },
        { "zh-Hans", "防火墙低语者" }
    },
    descriptions = new Dictionary<string, string> {
        { "en", "Reach level 40. The server obeys your voice now." },
        { "pl", "Osiągnij poziom 40. Serwer teraz słucha twojego głosu." },
        { "es", "Alcanza el nivel 40. El servidor ahora obedece tu voz." },
        { "fr", "Atteins le niveau 40. Le serveur t’obéit désormais." },
        { "de", "Erreiche Level 40. Der Server gehorcht jetzt deinem Befehl." },
        { "it", "Raggiungi il livello 40. Il server obbedisce solo a te." },
        { "ru", "Достигни 40 уровня. Сервер теперь подчиняется твоему голосу." },
        { "pt-BR", "Chegue ao nível 40. O servidor agora obedece à sua voz." },
        { "ko", "레벨 40에 도달하세요. 서버가 이제 당신의 목소리에 따릅니다." },
        { "zh-Hans", "达到等级40。服务器如今唯你马首是瞻。" }
    }
}
            };
        }
    }
}
