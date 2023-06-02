using ControlPositionTerminal.Util;
using System;

namespace ControlPositionTerminal.Exceptions;

public class ExecuteWithExceptionHandling
{
    /// <summary>
    /// Выполнить задачу типа Func с обработкой исключений. Если задача выбрасывает исключение,
    /// исключение регистрируется в логе, и возвращается значение по умолчанию.
    /// </summary>
    /// <typeparam name="T">Тип результата, который возвращает функция.</typeparam>
    /// <param name="func">Функция, которую нужно выполнить.</param>
    /// <returns>Результат, возвращенный функцией, или значение по умолчанию, если произошло исключение.</returns>
    public static T Execute<T>(Func<T> func)
    {
        try
        {
            T execute = func();
            return execute;
        }
        catch (Exception e)
        {
            ExceptionLog.SetLogMessage(e);
            return default(T); // return the default value for type T in case of an exception
        }
    }
}