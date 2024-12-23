using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhoneCallApp
{
    enum SubscriberType { Regular, Business, VIP }

    class CallRecord
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CallDate { get; set; }
        public double RatePerMinute { get; set; }
        public int DurationMinutes { get; set; }
        public double Discount { get; set; }
        public SubscriberType Type { get; set; }

        public CallRecord(string fullName, string phoneNumber, DateTime callDate, double rate, int duration, double discount, SubscriberType type)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            CallDate = callDate;
            RatePerMinute = rate;
            DurationMinutes = duration;
            Discount = discount;
            Type = type;
        }

        public double CalculateCost() => RatePerMinute * DurationMinutes * (1 - Discount / 100);

        public override string ToString() =>
            string.Join(";", FullName, PhoneNumber, CallDate.ToString("yyyy-MM-dd"), RatePerMinute, DurationMinutes, Discount, Type);

        public string ToDisplayString() =>
            $"{FullName} | {PhoneNumber} | {CallDate:yyyy-MM-dd} | {RatePerMinute} грн/хв | {DurationMinutes} хв | Знижка: {Discount}% | Вартість: {CalculateCost():F2} грн | Тип: {Type}";

        public static CallRecord FromString(string line)
        {
            var p = line.Split(';');
            return new CallRecord(p[0], p[1], DateTime.Parse(p[2]), double.Parse(p[3]), int.Parse(p[4]), double.Parse(p[5]), Enum.Parse<SubscriberType>(p[6]));
        }
    }

    class Program
    {
        const string FilePath = "calls.txt";

        static void Main()
        {
            var records = File.Exists(FilePath) ? File.ReadAllLines(FilePath).Select(CallRecord.FromString).ToList() : new List<CallRecord>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n===== Управління Дзвінками =====");
                Console.WriteLine("1. Додати запис");
                Console.WriteLine("2. Вивести всі записи");
                Console.WriteLine("3. Пошук за прізвищем");
                Console.WriteLine("4. Пошук за номером телефону");
                Console.WriteLine("5. Пошук за датою");
                Console.WriteLine("0. Вихід");
                Console.WriteLine("=================================");
                Console.Write("Ваш вибір: ");

                if (!int.TryParse(Console.ReadLine(), out var choice))
                {
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    continue;
                }
                if (choice == 0) break;
                switch (choice)
                {
                    case 1:
                        AddRecord(records);
                        break;
                    case 2:
                        ShowAllRecords(records);
                        break;
                    case 3:
                    case 4:
                    case 5:
                        SearchRecords(records, choice);
                        break;
                    default:
                        Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                        break;
                }
                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void AddRecord(List<CallRecord> records)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===== Додавання запису =====");
                var record = new CallRecord(
                    Prompt("ПІП: "),
                    Prompt("Телефон: "),
                    DateTime.Parse(Prompt("Дата (yyyy-MM-dd): ")),
                    double.Parse(Prompt("Тариф: ")),
                    int.Parse(Prompt("Хвилини: ")),
                    double.Parse(Prompt("Знижка %: ")),
                    Enum.Parse<SubscriberType>(Prompt("Тип (Regular/Business/VIP): "), true)
                );
                records.Add(record);
                File.AppendAllText(FilePath, record + "\n");
                Console.WriteLine("Запис успішно додано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowAllRecords(List<CallRecord> records)
        {
            Console.Clear();
            Console.WriteLine("===== Всі записи =====");
            if (records.Any())
                records.ForEach(r => Console.WriteLine(r.ToDisplayString()));
            else
                Console.WriteLine("Записів не знайдено.");
        }

        static void SearchRecords(List<CallRecord> records, int choice)
        {
            Console.Clear();
            Console.WriteLine("===== Пошук записів =====");
            var query = Prompt(choice == 3 ? "Введіть прізвище: " : choice == 4 ? "Введіть телефон: " : "Введіть дату (yyyy-MM-dd): ");
            var results = choice == 3 ? records.Where(r => r.FullName.Contains(query, StringComparison.OrdinalIgnoreCase)) :
                          choice == 4 ? records.Where(r => r.PhoneNumber == query) :
                          records.Where(r => r.CallDate.Date == DateTime.Parse(query));
            if (results.Any())
                results.ToList().ForEach(r => Console.WriteLine(r.ToDisplayString()));
            else
                Console.WriteLine("Записів не знайдено.");
        }

        static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
    }
}
