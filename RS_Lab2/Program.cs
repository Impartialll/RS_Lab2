using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace RS_Lab2
{
    class Program
    {
        static double x;
        static double y;
        static double z;
        static readonly object lockX = new object();
        static readonly object lockY = new object();
        static readonly object lockZ = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Варіант 7\n");
            Console.WriteLine("Завдання 1: Фільтрація (Т0) та квадрати (Т1) масиву");
            Task1();

            Console.WriteLine("\nЗавдання 2: Обчислення виразів А (Т0) та В (Т1)");
            Task2();

            Console.WriteLine("\nЗавдання 3: Обчислення за графом залежностей (Т1-Т5)");
            Task3();

            Console.ReadLine();
        }

        static void Task1()
        {
            int[] array = new int[10];
            Random rand = new Random();

            Console.Write("Початковий масив: ");
            for (int i = 0; i < 10; i++)
            {
                array[i] = rand.Next(0, 26);
                Console.Write($"{array[i]} ");
            }
            Console.WriteLine("\n");

            Thread t0 = new Thread(() =>
            {
                foreach (int val in array)
                {
                    if (val > 10 && val < 20)
                    {
                        Console.WriteLine($"T0 (10 < x < 20): {val}");
                    }
                }
            });

            Thread t1 = new Thread(() =>
            {
                foreach (int val in array)
                {
                    Console.WriteLine($"T1 (квадрат x): {val * val}");
                }
            });

            t0.Start();
            t1.Start();
            t0.Join();
            t1.Join();
        }

        static void Task2()
        {
            int nx = 100;
            int ny = 150;
            double[] X = new double[nx];
            double[] Y = new double[ny];
            Random rand = new Random();

            for (int i = 0; i < nx; i++)
            {
                X[i] = rand.NextDouble() * 25.0;
            }

            for (int i = 0; i < ny; i++)
            {
                Y[i] = rand.NextDouble() * 20.0 - 10.0;
            }

            double A = 0;
            double B = 0;

            Thread t0 = new Thread(() =>
            {
                double sum = 0;
                for (int i = 0; i < nx; i++)
                {
                    sum += X[i];
                }
                A = nx * Math.E + sum;
                Console.WriteLine($"T0: A = N*e + sum(X) = {A:F2}");
            });
            t0.Name = "T0";
            t0.Priority = ThreadPriority.Normal;

            Thread t1 = new Thread(() =>
            {
                double prod = 1;
                for (int i = 0; i < ny; i++)
                {
                    prod *= Y[i];
                }
                B = 5 + prod;
                Console.WriteLine($"T1: B = 5 + prod(Y) = {B:E2}");
            });
            t1.Name = "T1";
            t1.Priority = ThreadPriority.AboveNormal;

            t0.Start();
            t1.Start();
            t0.Join();
            t1.Join();
        }

        static void Task3()
        {
            Random rand = new Random();
            x = rand.Next(1, 10);
            y = rand.Next(1, 10);
            z = rand.Next(1, 10);

            Console.WriteLine($"Початкові значення: x={x}, y={y}, z={z}");

            Task t1 = new Task(() =>
            {
                lock (lockX)
                {
                    x = x + 3;
                }
            });

            Task t2 = new Task(() =>
            {
                lock (lockZ)
                {
                    z = z + 2;
                }
            });

            Task t3 = new Task(() =>
            {
                lock (lockY)
                {
                    y = y * 2;
                }
            });

            t1.Start();
            t2.Start();
            t3.Start();

            Task t4 = Task.WhenAll(t1, t2).ContinueWith(_ =>
            {
                lock (lockX)
                {
                    lock (lockZ)
                    {
                        x = z * x;
                    }
                }
            });

            Task t5 = Task.WhenAll(t2, t3, t4).ContinueWith(_ =>
            {
                lock (lockX)
                {
                    lock (lockY)
                    {
                        y = x * y;
                    }
                }
            });

            t5.Wait();
            Console.WriteLine($"Граф виконано. Кінцеві значення: x={x}, y={y}, z={z}");
        }
    }
}