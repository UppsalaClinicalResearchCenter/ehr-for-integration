
#Detta dokument

##Generellt

Vi pratade en del om de egenskaper vi ser hos lösningen och som vi ser som strategiskt viktiga. Vi ser båda att det faktum att registren fortfarande får presentera ett formulär hela vägen till användaren som den strategiskt viktigaste aspekten eftersom den är en förutsättning, dels för att veta vad det är användaren egentligen svarat på och dels för att bibehålla möjligheten att ställa krav på inmatad data. Andra viktiga aspekter är att det finns ett utrymme för att partiellt mappa registreringar utifrån den data man har, men lämna resterande uppgifter för manuell registrering, 

Viktigt att berätta att detta i första hand inte är en lösning som går i polemik mot andra intitativ för att överföra data.

##Samarbete ur ett tekniskt perspektiv

* Strukturerad data behövs för meningsfull integration, oavsett teknisk lösning - Viktigt att synliggöra detta för slutanvändaren 
* Prova konceptet - Beställa ett förifyllt formulär utifrån journaldata fungerar väl med UCRs och Incas tekniska förutsättningar, med samma teknisk på journalsystemsidan
 


##Införande
* Manuellt arbete att koppla begrepp från journal mot registrets behov 
* Kvalitetsregister är olika både i innehåll och process - svåra att generalisera, men lätta i ett konkret fall, kommer att kräva en teknisk insats hos båda parter i varje integration 
* Kvalitetsregister har en specifikation på data som efterfrågas, det saknas motsvarande dokumentation för de olika journalsystemens innehåll. Därmed är det lättare för journalsystemen att förstå kvalitetsregistrens specifikationer än vice versa

##Change Management
Då vi endast gör förifyllnad så behövs ingen samordning av releaser (journal och kvalitetsregister). En ny variabel i kvalitetsregister innebär bara att den ej blir förifylld (tills mappning i journal finnes)

###Uppdaterade versioner av journal
* Om journalsystemet sköter mappningen så kan de förändras hur som helst så länge de uppfyller specifikationen. Om man inte längre uppfyller specifikationen så kommer data inte längre förifyllas, men integrationen kommer fungera i övrigt

###Uppdaterade versioner av registrets formulär
* Innebär ett manuellt förvaltningssteg att konfigurera mappning för eventuella nya/ändrade variabler 

##Återanvändbarhet
* Möjlighet till återanvändbarhet som i tjänstekontrakt finns inte med denna lösning. Däremot så tror vi att arbetet som behövs för en ny integration är begränsat att återanvändbarhet är mindre viktigt
* 

##Förslag på nästa steg i samarbete
* Dela detta proof-of-concept med fler registercentrum (Stratum)
* Bygga en exempelapplikation som kan fylla i flera register på olika plattformar (påbörjat)


##Tekniska förbättringar på PoC
* POST istf för GET för att skicka "data" och meta-data till ett register från journalsystem  - [Schema](schema/registration-schema.json)
* Ej skapa ärenden vid varje nytt anrop (Inca)
* PoC på senaste version av plattform (UCR) 
* Hantera att det redan finns en registrering som ska uppdateras 

  
  