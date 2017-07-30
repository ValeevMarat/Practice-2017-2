using System;

namespace Practice_2017_2
{
    // Задание №2 практики 2017г.
    // Создать алгоритм декодирования
    // https://acmp.ru/index.asp?main=task&id_task=655

    class Program
    {
        static int[] LongToIntArr(long code, int length)
        {                        // Число для перевода, длина массива
            long p = 1;
            int[] londGigits = new int[length];       // Хранение чисел code

            for (int i = 0; i < length; i++, p *= 10) // Запись цифр с конца
                londGigits[i] = (int)(code / p % 10);

            return londGigits;
        }                                           // Превращает число типа long в массив цифр (читает цифры с конца)

        static bool FoundNDigits(int start, int[] nDigits, int k, int transfComb, int[] sumDigits)
        {                       // Начальный индекс, цифры N, кол-во сдвигов, комбинация переносов и цифры code
            int cur = start;                                                                    // Текущий индекс
            int nLen = nDigits.Length;

            while (true)                                                                        // Ищем значения пока не дойдём до первого или пока не найдётся ошибка
            {
                int next = (cur + nLen - k) % nLen,                                             // Следующий индекс
                    carryFrom = (transfComb >> cur) & 1,                                        // Есть ли перенос из этого разряда (бинарный сдвиг быстрее возведения в степень)
                    carryTo = cur > 0 ? (transfComb >> (cur - 1)) & 1 : 0,                      // Есть ли перенос в текущий разряд, у последних цифр перенос в этот разряд невозможен
                    nextDigitValue = carryFrom * 10 + sumDigits[cur] - carryTo - nDigits[cur];  // Следующее значение

                if (nextDigitValue < 0 || nextDigitValue > 9)                                   // Если следуещее значение некорректно, значит и начальное тоже => требуется продолжить перебор
                    return false;

                if (next == nLen - 1 && nextDigitValue == 0)                                    // Если следующий элемент первый и равен нулю, выходим
                    return true;

                if (next == start && nextDigitValue != nDigits[start])                          // Если следующий индекс равен начальному и следующее значение не равно начальному, то начальная цифра не подходит
                    return false;

                nDigits[next] = nextDigitValue;                                                 // Присваиваем следующему индексу следующее значение
                cur = next;                                                                     // Текущее становится следующим

                if (cur == start)                                                               // Если пришло к началу, то перебор останавливается
                    return true;
            }
        } // Найдены ли цифры числа N

        static long IntArrToLong(int[] nDigits)
        {                       // Массив с цифрами N (перевёрнутый)
            int nLen = nDigits.Length;          // Длина N
            long n = 0;

            for (int i = nLen - 1; i >= 0; i--) // Записываем в N найденное значение
                n = n * 10 + nDigits[i];

            return n;
        }                                                    // Превращает цифры из массива в число типа long (читает цифры с конца)

        static bool IsNCorrect(long n, int[] nDigits, long code, int k)
        {                      // N,   цифры N,       начальное число и кол-во сдвигов
            int nLen = nDigits.Length;      // Длина N

            long shifted = 0;               // cyclic(N,k)
            for (int i = 0; i < nLen; i++)  // Записываем cyclic(N,k)
                shifted = shifted * 10 + nDigits[(nLen - 1 - k - i + nLen) % nLen];

            return n + shifted == code;     // N верно, если соблюдается это равенство
        }                            // Проверяет, верно ли N

        static long Decode(long code, int k)
        {                // Число после кодирования и кол-во сдвигов
            for (int nLen = code.ToString().Length - 1; nLen <= code.ToString().Length; nLen++) // Длина N может быть меньше на единицу либо равно cyclic(N, K), поэтому перебираются оба варианта
            {
                int[] sumDigits = LongToIntArr(code, nLen);                                     // Массив с цифрами числа code
                int[] nDigits = new int[nLen];                                                  // Цифры N

                for (int transfComb = 0; transfComb < Math.Pow(2, nLen); transfComb++)          // Перебор 2^nLen (перебор комбинаций переносов в разряды)
                {
                    for (int i = 0; i < nLen; i++)                                              // Отмечаем каждый элемент как непроверенный
                        nDigits[i] = -1;

                    for (int start = 0; start < nLen; start++)                                  // Перебираем элементы, от которых отталкиваемся
                        if (nDigits[start] == -1)                                               // Если элемент ещё не посещён, то требуется подобрать ему нужное значение
                        {
                            bool digitsFound = false;                                           // Найдены ли цифры
                            for (int i = 0; i <= 9; i++)                                        // Перебор цифр
                            {
                                if (start == nLen - 1 && i == 0)                                // Первая цифра N не может быть равна нулю
                                    continue;

                                nDigits[start] = i;
                                if (digitsFound = FoundNDigits(start, nDigits, k, transfComb, sumDigits)) break;
                            }
                            if (!digitsFound)                                                   // Если цифры не найдены, разрываем цикл
                                break;
                        }
                    long n = IntArrToLong(nDigits);                                             // Переводит цифры в число типа long
                    if (IsNCorrect(n, nDigits, code, k))                                        // Если N+cyclic(N,k)==code, значит N найдено
                        return n;
                }
            }
            return -1;                                                                          // Недостижимо, если данные корректны
        }                                                       // Декодориует code

        static void Main(string[] args)
        {
            Console.WriteLine(Decode(long.Parse(Console.ReadLine()), int.Parse(Console.ReadLine())));
        }                                                            // Читает code и k, выводит N
    }
}