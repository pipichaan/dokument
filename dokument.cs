using System;
using System.Threading;

/// <summary>
/// Класс <see cref="Account"/> представляет банковский счет, 
/// позволяя осуществлять пополнение и снятие средств с обеспечением потокобезопасности.
/// </summary>
class Account
{
    /// <summary>
    /// Переменная для хранения текущего баланса счета.
    /// </summary>
    private decimal balance;

    /// <summary>
    /// Объект для синхронизации доступа к счету между потоками.
    /// </summary>
    private readonly object sinh = new object();

    /// <summary>
    /// Получает текущий баланс счета.
    /// </summary>
    /// <value>Текущий баланс счета.</value>
    public decimal Balance
    {
        get
        {
            lock (sinh) // Блокировка доступа, чтобы избежать гонки потоков.
            {
                return balance;
            }
        }
    }

    /// <summary>
    /// Метод для пополнения счета на указанную сумму.
    /// </summary>
    /// <param name="sum">Сумма, на которую нужно пополнить счет. 
    /// Должна быть положительной.</param>
    /// <exception cref="ArgumentException">Вызывается, если сумма меньше или равна нулю.</exception>
    public void popolnenie(decimal sum)
    {
        if (sum <= 0) // Проверка, чтобы сумма была положительной.
            throw new ArgumentException("Сумма должна быть положительной");

        lock (sinh) // Блокирует доступ для безопасного изменения баланса.
        {
            balance += sum; // Увеличивает баланс на сумму пополнения.
            Console.WriteLine($"Пополнение: {sum}. Баланс: {balance}");
            Monitor.Pulse(sinh); // Сигнализирует, что баланс пополнен.
        }
    }

    /// <summary>
    /// Метод для снятия средств со счета.
    /// </summary>
    /// <param name="suum">Сумма, которую необходимо снять.
    /// Должна быть положительной.</param>
    /// <exception cref="ArgumentException">Вызывается, если сумма снятия меньше или равна нулю.</exception>
    public void snyatie(decimal suum)
    {
        if (suum <= 0) // Проверка положительной суммы снятия.
            throw new ArgumentException("Сумма снятия должна быть положительной");

        lock (sinh) // Блокирует доступ для безопасного изменения баланса.
        {
            while (balance < suum) // Проверка, достаточно ли средств на счете.
            {
                Console.WriteLine($"Недостаточно средств для снятия {suum}. Баланс: {balance}");
                Monitor.Wait(sinh); // Ожидание, пока не будет пополнен счет.
            }
            balance -= suum; // Уменьшает баланс на сумму снятия.
            Console.WriteLine($"Снятие: {suum}. Остаток: {balance}");
        }
    }
}