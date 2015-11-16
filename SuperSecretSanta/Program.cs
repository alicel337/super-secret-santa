using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace SuperSecretSanta
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WriteIntroText();

            var details = GetCoupleDetails();

            PairUp(details);

            SendEmails(details);
        }

        private static void WriteIntroText()
        {
            Console.WriteLine("* Welcome to Super Secret Santa v1.0 *");
            Console.WriteLine("======================================");
            Console.WriteLine("      _                               ");
            Console.WriteLine("     | |           by                 ");
            Console.WriteLine("    -----          L337 Al            ");
            Console.WriteLine("    ( ''>          &                  ");
            Console.WriteLine("    (  :)          5up3r l337 r0b     ");
            Console.WriteLine("    (  :)                             ");
            Console.WriteLine();
        }

        private static IList<Person> GetCoupleDetails()
        {
            Console.WriteLine("** Type DONE when you're finished entering details of all the couples **");

            var people = new List<Person>();

            while (true)
            {
                var partnerA = GetPersonDetails("first");

                if (partnerA == null) break;

                var partnerB = GetPersonDetails("second");

                if (partnerB == null) break;

                partnerA.Partner = partnerB;
                partnerB.Partner = partnerA;

                people.Add(partnerA);
                people.Add(partnerB);

                Console.WriteLine("\nCouple details added.\n");
            }

            return people;
        }

        private static Person GetPersonDetails(string order)
        {
            var person = new Person();

            Console.WriteLine($"Enter name of {order} partner in couple:");
            person.Name = Console.ReadLine();

            if (person.Name == "DONE") return null;

            Console.WriteLine($"Enter email address of {order} partner in couple:");
            person.EmailAddress = Console.ReadLine();

            return person;
        }

        private static void PairUp(IList<Person> people)
        {
            WriteBusyText("RANDOMISING");

            while (true)
            {
                var shuffledPartners = people.OrderBy(p => Guid.NewGuid()).ToList();

                for (var personCount = 0; personCount < people.Count; personCount++)
                {
                    if (people[personCount] == shuffledPartners[personCount]
                        || people[personCount].Partner == shuffledPartners[personCount])
                        break;

                    people[personCount].Buyer = shuffledPartners[personCount];

                    if (personCount == people.Count - 1) return;
                }
            }
        }

        private static void SendEmails(IList<Person> people)
        {
            WriteBusyText("SENDING EMAILS");

            using (var client = new SmtpClient("removed", 999)
            {
                Credentials = new NetworkCredential("removed", "removed"),
                EnableSsl = true
            })
            {
                foreach (var person in people)
                {
                    var body =
                        $"Hi {person.Buyer.Name},\n\nYou have been chosen by Super Secret Santa v1.0 to buy for {person.Name}.";

                    var message = new MailMessage("removed", person.Buyer.EmailAddress,
                        "Super Secret Santa", body);

                    client.Send(message);
                }
            }
        }

        private static void WriteBusyText(string text)
        {
            Console.WriteLine($"{text} ");

            for (var count = 0; count < 200; count++)
            {
                Console.Write(". ");
                Thread.Sleep(5);
            }
        }
    }

    public class Person
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public Person Buyer { get; set; }
        public Person Partner { get; set; }
    }
}
