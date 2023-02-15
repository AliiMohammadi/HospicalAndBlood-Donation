using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
/// <summary>
/// پروژه بیمارستان و بیمارانی که میخواهد خون دریافت کنن یا خون اهدا کند.
/// مسئله تمرینی درس برنامه نویسی اقای پوریا علیان نژاد از دانشگاه فردوسی مشهد
/// علی محمدی
/// </summary>
namespace HospitalIllsAndAlongsProblem
{
    class Program
    {
        /// <summary>
        /// این عدد ظرفیت کل مشتری هاست
        /// یعنی تعداد جفت ها 
        /// بیمار و همرا ه بیمار یک مشتری حساب میشوند
        /// </summary>
        const int Capacity = 10;
        /// <summary>
        /// جدول گروه خونی ها البته با عدد . صفر نشون دهنده اینه که گروه خونی نمیخوره
        /// </summary>
        static int[,] Matrix = new int[,]
        {
                 //|O-|O+|A-|A+|B-|B+|AB-|AB+
            /*O-*/{ 1, 0, 0, 0, 0, 0, 0,  0 },
            /*O+*/{ 1, 1, 0, 0, 0, 0, 0,  0 },
            /*A-*/{ 1, 0, 1, 0, 0, 0, 0,  0 },
            /*B-*/{ 1, 0, 0, 0, 1, 0, 0,  0 },
            /*A+*/{ 1, 1, 1, 1, 0, 0, 0,  0 },
            /*B+*/{ 1, 1, 0, 0, 1, 1, 0,  0 },
           /*AB-*/{ 1, 0, 1, 0, 1, 0, 1,  0 },
           /*AB+*/{ 1, 1, 1, 1, 1, 1, 1,  1 }
        };
        // جدول بالا به ترتیب کم چیده شده. یعنی دقیقا مثل عکس نشده شده. اگه دقت کنید بی مفنی بالاتره از ایه

        static void Main(string[] args)
        {
            print("Hello. wellcome to this program.");

            while (true)
            {
                Console.WriteLine();
                print("*********************************.");
                Console.WriteLine();

                List<string> ills = new List<string>();
                List<string> alongs = new List<string>();
                List<int> ScoreBoard = new List<int>();

                Stopwatch timer = new Stopwatch();
                print("Generate automatilcy or do you want to insert your list here?");
                print("(gen for generate . ins for inserting)");
                string input = Console.ReadLine();
                if (input.Equals("ins"))
                    GetInput(ref ills, ref alongs);
                else
                if (input.Equals("gen"))
                    GenerateRandomPeopl(ref ills, ref alongs);
                else
                {
                    print("Invalid selection. Generating as default.");
                    GenerateRandomPeopl(ref ills, ref alongs);
                }
                //این تایمر برای محاسبه زمان اجرای برنامه هستش که میفهمیم برنامه چقدر طول کشیده تا اجرا بشه
                timer.Start();

                List<string> best = FindBestFit(ills, alongs, ref ScoreBoard);

                timer.Stop();

                //اینجا نمایش نتیجس
                print("Best founded fits is : ");
                if (best.Count == 0)
                    ShowList(ills, alongs);
                else
                    ShowList(ills, best);

                Console.WriteLine();

                //این قسمتو اضافی قرار دادم که یک سری اطلاعات بده بعد از تموم شدن برنامه 
                print("Log informations: ");
                //اطلاعت زمان محاسبه 
                print("Elapsed time(Calculation time in milisecond): " + timer.ElapsedMilliseconds + "ms.");
                //تعداد حالت های ممکن 
                print("Possible sampel sates : Fact(" + ills.Count + ").");
                // بهترین امتیازی که هست
                print("Best possible score: " + ScoreBoard.Max());
                // الباقی حالت های هم تراز با اونی که پیدا شده
                print("Other best possible states: " + (ScoreBoard.Where(x => x == ScoreBoard.Max()).Count() - 1));
                Console.WriteLine();

                print("Type ex for exiting or Enter for restaring.");
                if (Console.ReadKey().Equals("ex")) break;
            }

            Environment.Exit(0);
            //Ali Mohammadi
        }
        /// <summary>
        /// تابع ساده گرفتن ورودی از کاربر
        /// </summary>
        static void GetInput(ref List<string> ills, ref List<string> alongs)
        {
            int counter = 1;

            while (counter <= Capacity)
            {
                print("*" + counter);
                print("Enter Ill blood order:");
                ills.Add(Console.ReadLine());
                print("Enter ALong blood order:");
                alongs.Add(Console.ReadLine());

                print("Added.");
                print("type ok to stop adding. or Enter to add another.");

                string ex = Console.ReadLine();

                if (ex.Equals("ok"))
                {
                    break;
                }
                else
                {
                    counter++;
                }
            }
        }
        /// <summary>
        /// جمعو جور کننده تابع های اصلی توی یک تابع
        /// </summary>
        /// <param name="Ills"></param>
        /// <param name="Alongs"></param>
        /// <returns></returns>
        static List<string> FindBestFit(List<string> Ills, List<string> Alongs, ref List<int> scores)
        {
            int count = Ills.Count;
            int max = 0;
            int score = 0;

            List<string> Generateds = PrimaryStatesOf(Alongs, Ills, count);
            List<string> CurrentState = new List<string>();
            List<string> BestState = new List<string>();

            //حلقه پیدا کردن بهرتین حالت بین حالت های محاسبه شده
            //توی این حلقه حالت ها امتیازشون محاسبه میشه و در اخر بهترین حالت برگردانده میشه
            foreach (var item in Generateds)
            {
                CurrentState = item.Split(',').ToList();
                score = CountFits(Ills, CurrentState);

                scores.Add(score);

                if (score >= count) return CurrentState;

                if (score > max)
                {
                    max = score;
                    BestState = new List<string>(CurrentState);
                }

            }

            return BestState;
        }

        /// <summary>
        /// لیست رو میگیره و تعداد حالت های ممکن توی چینش بدون تکرار عناصر رو برمیگردونه
        /// هسته اصلی برنامه این تابع هست.
        /// </summary>
        /// <param name="AlongList"></param>
        /// <param name="IllList"></param>
        /// <param name="L"></param>
        /// <returns></returns>
        static List<string> PrimaryStatesOf(List<string> AlongList, List<string> IllList, int L)
        {
            if (L <= 0)
                return null;
            else
            {
                List<string> Result = new List<string>();
                List<string> Axu = new List<string>();
                List<string> Axu2;


                for (int i = 0; i < AlongList.Count; i++)
                {
                    Axu2 = new List<string>(AlongList);
                    Axu2.Remove(AlongList[i]);
                    Axu = PrimaryStatesOf(Axu2, IllList, L - 1);

                    if (Axu == null)
                    {
                        Result.Add(AlongList[i]);
                    }
                    else
                        for (int j = 0; j < Axu.Count; j++)
                        {
                            Result.Add(AlongList[i] + "," + Axu[j]);
                        }

                }

                return Result;
            }
        }

        /// <summary>
        /// تعداد جور بودن گروه خونی مریض و همراه داده شده رو میشماره 
        /// </summary>
        /// <param name="Ills"></param>
        /// <param name="Alongs"></param>
        /// <returns></returns>
        static int CountFits(List<string> Ills, List<string> Alongs)
        {
            int count = 0;

            for (int i = 0; i < Ills.Count; i++)
                if (IsPossibleMerg(Ills[i], Alongs[i].ToString())) count++;

            return count;

        }

        /// <summary>
        /// چک میکنه که ایا دوتا گروه خونی بهم میخورن یا نه
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static bool IsPossibleMerg(string x, string y)
        {
            return (IsPossibleMerg(ConvertToIndex(x), ConvertToIndex(y)));
        }
        /// <summary>
        /// چک میکنه که ایا دوتا گروه خونی بهم میخورن یا نه
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static bool IsPossibleMerg(int x, int y)
        {
            return (Matrix[x, y] == 1);
        }

        /// <summary>
        /// این تابه ایندکس گروه خونی رو میگیره و اسم اون رو برمیگردونه
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static string ConvertToName(int x)
        {
            switch (x)
            {
                case 0:
                    return "O-";
                case 1:
                    return "O+";
                case 2:
                    return "A-";
                case 3:
                    return "B-";
                case 4:
                    return "A+";
                case 5:
                    return "B+";
                case 6:
                    return "AB-";
                case 7:
                    return "AB+";

                default:
                    Console.WriteLine("-<ERORR: .>-");
                    printError("index out of the range (0 to 7).");
                    return " ";
                    break;
            }
        }
        /// <summary>
        /// این تابع اسم گروه خونی رو میگیره بعدش ایندکسش رو با توجه به جدول ماتریس بالا برمیگردونه
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static int ConvertToIndex(string x)
        {
            switch (x)
            {
                case "O-":
                    return 0;
                case "O+":
                    return 1;
                case "A-":
                    return 2;
                case "B-":
                    return 3;
                case "A+":
                    return 4;
                case "B+":
                    return 5;
                case "AB-":
                    return 6;
                case "AB+":
                    return 7;

                default:
                    printError("Unrocidnized name. " + x);
                    return 0;
            }
        }
        /// <summary>
        /// نمایش دو لیست مریض ها و همراهان
        /// </summary>
        /// <param name="ills"></param>
        /// <param name="alongs"></param>
        static void ShowList(List<string> ills, List<string> alongs)
        {
            print("Ills Alongs");

            for (int i = 0; i < ills.Count; i++)
            {
                print(ills[i] + "  " + alongs[i]);
            }
        }
        /// <summary>
        /// تابع جنریت کردن سریع تصادفی بیماران و همراهان.
        /// برای اسون شدن وارد کردن وردوی نوشته شده
        /// </summary>
        /// <param name="Numberofpeople"></param>
        /// <param name="ills"></param>
        /// <param name="alongs"></param>
        static void GenerateRandomPeopl(byte Numberofpeople, ref List<string> ills, ref List<string> alongs)
        {
            if (Numberofpeople > Capacity)
                return;

            Random randomnum = new Random();

            for (int i = 0; i < Numberofpeople; i++)
            {
                ills.Add(ConvertToName(randomnum.Next(7)));
                alongs.Add(ConvertToName(randomnum.Next(7)));
            }

            print("GENERATED RANDOM PEOPL:");
            ShowList(ills, alongs);
        }
        /// <summary>
        /// تابع جنریت کردن سریع تصادفی بیماران و همراهان.
        /// برای اسون شدن وارد کردن وردوی نوشته شده
        /// </summary>
        /// <param name="ills"></param>
        /// <param name="alongs"></param>
        static void GenerateRandomPeopl(ref List<string> ills, ref List<string> alongs)
        {
            Random randomnum = new Random();

            for (int i = 0; i < randomnum.Next(3, Capacity); i++)
            {
                ills.Add(ConvertToName(randomnum.Next(7)));
                alongs.Add(ConvertToName(randomnum.Next(7)));
            }

            print("GENERATED RANDOM PEOPL:");
            ShowList(ills, alongs);
        }

        static void print(object ms)
        {
            Console.WriteLine(ms);
        }
        static void printError(object ms)
        {
            Console.WriteLine("-<ERORR: " + ms + " >-");
        }
    }
}
