namespace E
{
    public interface ICallback { }

    public interface IAction : ICallback
    { public void Invoke(); }

    public interface IAction<in T0> : ICallback
    { public void Invoke(T0 t0); }

    public interface IAction<in T0, in T1> : ICallback
    { public void Invoke(T0 t0, T1 t1); }

    public interface IAction<in T0, in T1, in T2> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2); }

    public interface IAction<in T0, in T1, in T2, in T3> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14); }

    public interface IAction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15> : ICallback
    { public void Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15); }

    public interface IFunction<out TResult> : ICallback
    { public TResult Invoke(); }

    public interface IFunction<in T0, out TResult> : ICallback
    { public TResult Invoke(T0 t0); }

    public interface IFunction<in T0, in T1, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1); }

    public interface IFunction<in T0, in T1, in T2, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2); }

    public interface IFunction<in T0, in T1, in T2, in T3, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14); }

    public interface IFunction<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, out TResult> : ICallback
    { public TResult Invoke(T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15); }
}