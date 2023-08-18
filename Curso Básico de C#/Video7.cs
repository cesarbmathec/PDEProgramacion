using System;

namespace PracticasCurso
{
    public class Program
    {
        public static void Main(string[] args)
        {
            float a, b, c;
            float det;
            float x1, x2;

            Console.WriteLine("Introduzca coeficiente a: ");
            a = Convert.ToSingle(Console.ReadLine());
            Console.WriteLine("Introduzca coeficiente b: ");
            b = Convert.ToSingle(Console.ReadLine());
            Console.WriteLine("Introduzca coeficiente c: ");
            c = Convert.ToSingle(Console.ReadLine());

            det = MathF.Pow(b, 2) - 4 * a * c;

            if (det > 0)
            {
                x1 = (-b + MathF.Sqrt(det)) / (2 * a);
                x2 = (-b - MathF.Sqrt(det)) / (2 * a);

                Console.WriteLine("Raices reales x1: {0} , x2: {1}", x1, x2);
            }
            else
            {
                Console.WriteLine("No posee raices reales");
            }
        }
    }
}
