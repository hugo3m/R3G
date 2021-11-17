using UnityEngine;
using System.Collections;

/// <summary>
/// Place this object on any game object to deffer code execution from other threads to unity's thread
/// <example>
/// <code>
/// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
/// UnityThreadExecute.InvokeNextUpdate(() =>
/// {
///     *Unity dependent code *
///     handle.Set();
/// });
/// handle.WaitOne();
/// </code>
/// the handle variable ensures that the thread waits for the code to be executed in unity's thread
/// </example>
/// </summary>
public class UnityThreadExecute : UnitySingleton<UnityThreadExecute> {

    protected UnityThreadExecute () {}

    System.Action _invoking;

    private static object _lock = new object();

    // This object needs to exist from the start otherwise we will attempt to create it through the first threaded call and it will fail
    static UnityThreadExecute _instance;

    void Awake()
    {
        _instance = UnityThreadExecute.Instance;
    }

    void Update()
    {
        if (_invoking != null)
        {
            System.Action act;
            lock (_lock)
            {
                act = _invoking;
                _invoking = null;
            }
            act();
        }

    }

    /// <summary>
    /// Deffer code execution from other threads to unity's thread
    /// <example>
    /// <code>
    /// var handle = new EventWaitHandle(false, EventResetMode.AutoReset);
    /// UnityThreadExecute.InvokeNextUpdate(() =>
    /// {
    ///     *Unity dependent code *
    ///     handle.Set();
    /// });
    /// handle.WaitOne();
    /// </code>
    /// the handle variable ensures that the thread waits for the code to be executed in unity's thread
    /// </example>
    /// </summary>
    public static void InvokeNextUpdate(System.Action action)
    {
        if (action == null) throw new System.ArgumentNullException("action");
        var h = _instance;
        lock (_lock)
        {
            h._invoking += action;
        }
    }
}
