//Det här programmet är en gästbok. Det går att skapa inlägg och ta bort inlägg. Inläggen sparas lokalt på datorn i samma mapp som program.cs ligger. Inmatningar kontrolleras så att inte fel värde går att ange.
// Skrivet av Markus Vickman 2024-09-19
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

//Programmet GuestBook
namespace GuestBook
{
    //Klassen Guest där objekt för inlägg skapas i en konstruktor. Klassen innehåller även metoder
    public class Guest
    {
        public string User { get; set; }
        public string Post { get; set; }

        //Konstruktorn som skapar gästinläggs-objekt
        public Guest(string user, string post)
        {
            User = user;
            Post = post;
        }


        //Main uppgiften i programmet. Här startar programmet. 
        public static Task Main()
        {
            //variabel med namnet på filen för inlägg som sparas på samma plats som program.cs
            string fileName = "guestbook_mavi.json"/*@"c:\windows\Temp\guestbook_mavi.json"*/;
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            //Consolen rensas och programmets namn skrivs ut.
            Console.Clear();
            Console.WriteLine("M A R K U S  G Ä S T B O K");

            //Metoden för programmets val körs
            Guest.ProgramCoices(fileName);

            return Task.CompletedTask;
        }


        //En metod där programmets val skriv ut på skärmen och det går att välja utifrån alternativ. Metoden tar emot filsökvägen som parameter.
        public static void ProgramCoices(string fileName)
        {
            Console.WriteLine("\r\n[0] Läsa alla inlägg \r\n[1] Skriv nytt inlägg \r\n[2] Ta bort ett inlägg \r\n[3] Avsluta programmet");
            string coice = Console.ReadLine()!;

            //4 val där valet sparas i strängen coice och jämförs nedan för att se att svaret verkligen var 0, 1, 2 eller 3.
            if (coice == "0" || coice == "1" || coice == "2" || coice == "3")
            {
                //Rensar consolen och skriver ut programnamnet igen
                Console.Clear();
                Console.WriteLine("M A R K U S  G Ä S T B O K");

                //Vid val 0 initieras en metod för att skriva ut alla inlägg på skärmen. Coice tas med som argument för att metoden ska veta vilket val som initierade den.
                if (coice == "0")
                {
                    Guest.ReadPosts(fileName, coice);
                }

                //Vid val 1 initieras en metod för att skriva ett nytt inlägg 
                else if (coice == "1")
                {
                    Guest.WritePost(fileName);
                }

                //Vid val 2 körs först en metod för att skriva ut alla indexerade poster på skärmen, coice är nu 2 och därför händer inget mer i den metoden. Sedan körs en metod för att tabort inlägg. 
                else if (coice == "2")
                {
                    Guest.ReadPosts(fileName, coice);
                    Guest.RemovePost(fileName);
                }

                //Vid val 3 rensas consolen och sedan avslutas programmet.
                else if (coice == "3")
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
            }
            //Om valet inte är 0, 1, 2, eller 3 så skrivs ett felmeddelande och valen skrivs ut igen.
            else
            {
                Console.WriteLine("\r\nFelinmatat värde! Svara 0, 1, 2 eller 3.");
                Guest.ProgramCoices(fileName);
            }
        }


        //Metoden där inlägg skapas
        public static void WritePost(string fileName)
        {
            //åäö fungerade inte och därför skapades en inställning för JsonScriptEncodern där alla Unicodes är acceptabla.
            var unicodesJson = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = false
            };

            //Tillfrågas att skriva in namn och inlägg
            Console.WriteLine("Skriv Gästnamnet:");
            string guestName = Console.ReadLine()!;

            Console.WriteLine("Skriv gästbookinlägget:");
            string guestPost = Console.ReadLine()!;

            //Check för att se att inget av fälten är tomma. Om något är tomt skrivs ett felmeddelande ut och metoden körs om från början.
            if (guestName.Length == 0 || guestPost.Length == 0)
            {
                Console.WriteLine("\r\nBåde gästnamn och inlägg måste skrivas!");
                Guest.WritePost(fileName);
            }
            //annars skapas ett nytt object för inlägg med namn och meddelande med hjälp av Guest konstruktorn.
            else
            {
                Guest test1 = new Guest(guestName, guestPost);

                //Objektet Json serializeras med hjälp av inställningarna i början av metoden.
                string test2 = JsonSerializer.Serialize(test1, unicodesJson);

                //En rad skrivs in i filen där objektet nu har json-format och avslutas på en tom rad.
                File.AppendAllText(fileName, test2 + Environment.NewLine);

                //Main metoden körs igen, alltså programmet körs helt från början
                Guest.Main();
            }
        }


        //Metod för att ta bort inlägg
        public static void RemovePost(string fileName)
        {

            //Läser in index för vilket inlägg som ska tas bort.
            Console.WriteLine("\r\nVilket inlägg vill du radera?");
            string coice = Console.ReadLine()!;

            //Om valet som gjordes var en siffra skapas en integer med namn j.
            if (int.TryParse(coice, out int j))
            {
                //Används för att checka om indexet finns.
                bool testIfExists = false;

                //En lista skapas med en sträng för varje rad i gästboksfilen. Det skrivs fortfarande som Json-format.
                List<string> linesFromFile = File.ReadAllLines(fileName).ToList();

                //längden på listan räknas med index 0 ända tills listan är slut.
                for (int i = 0; i <= linesFromFile.Count; i++)
                {
                    //Om valet som gjordes (coice/int j) matchar med något index i listan från filen så ändras testIfExists till true.
                    if (j == (i - 1))
                    {
                        testIfExists = true;
                    }
                }

                //Om testIfExists blev true i föregående loop tas den strängen bort från listan och sedan skrivs hela listan tillbaka till filen igen.
                if (testIfExists)
                {
                    linesFromFile.RemoveAt(j);
                    File.WriteAllLines(fileName, linesFromFile.ToArray());
                }

                //Om ett inlägg med det indexet inte hittades skrivs ett felmeddelande ut. Metoden körs om från början
                else
                {
                    Console.WriteLine("\r\nDet finns inget inlägg att radera på detta index: " + coice);
                    Guest.RemovePost(fileName);
                }
            }

            //Om valet inte går att konvertera till integer är det ett ogiltigt svar och ett felmeddelande skrivs ut. Metoden körs om från början
            else
            {
                Console.WriteLine("\r\nIndex som ska raderas måste vara en siffra!");
                Guest.RemovePost(fileName);
            }
            //Programmet körs om från början
            Guest.Main();
        }


        //Metod för att läsa upp alla inlägg. Coice tas in som parameter för att om valet var 0 så ska menyn köras igen efter utskrift.
        public static void ReadPosts(string fileName, string coice)
        {
            //åäö fungerade inte och därför skapades en inställning för JsonScriptEncodern där alla Unicodes är acceptabla.
            var unicodesJson = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = false
            };

            //Läser om hela filen till en sträng för att testa så att inte filen är tom.
            string jsonString = File.ReadAllText(fileName);

            //Om filen inte är tom skrivs alla inlägg ut på skärmen
            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                Console.WriteLine("\r\nAlla dagboksinlägg:\r\n");

                //Används för att skriva ut ett index till varje inlägg (kanske en for-loop hade varit bättre men det här fungerar bra.)
                int counter = 0;

                //Alla rader i filen skrivs ut en efter en
                foreach (var line in File.ReadAllLines(fileName))
                {
                    //Check så att inga tomma rader tas med (har dock begränsat det i metoden för skriva nya inlägg)
                    if (line.Length > 0)
                    {
                        //Varje rad deserializeras och objekt görs i Guest konstruktorn
                        Guest guest = JsonSerializer.Deserialize<Guest>(line, unicodesJson)!;
                        
                        //Index, namn och inlägget skrivs ut på skärmen
                        if (coice == "2")
                        {
                        Console.WriteLine($"[{counter}] {guest.User}: {guest.Post}");
                        }

                        else 
                        {
                        Console.WriteLine($"{guest.User}: {guest.Post}");
                        }

                        //Räknar upp index
                        counter++;
                    }
                }

                //Om metoden kallades på för att bara läsa upp inläggen så återgår vi till menyn annars görs inget mer. Används vid val 2 för att metoden bara ska skriva upp inlägg och sedan gå vidare för att ge alternativ för att ta bort.
                if (coice == "0")
                {
                    Guest.ProgramCoices(fileName);
                }
                else{
                }
            }

            //Om det inte finns några inlägg i gästboken meddelas dessa och val ges för att skriva nytt eller avsluta.
            else
            {
                Console.WriteLine("\r\nDet finns inga inlägg i gästboken. \r\n[j] Skriva nytt inlägg \r\n[n] För att avsluta programmet");
                string answer = Console.ReadLine()!;
                if (answer == "j" || answer == "n")
                {
                    if (answer == "j")
                    {
                        Guest.WritePost(fileName);
                    }
                    else if (answer == "n")
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Du måste välja mellan j eller n.");
                    Guest.ReadPosts(fileName, coice);
                }
            }
        }
    }
}