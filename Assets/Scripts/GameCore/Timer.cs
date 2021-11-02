using System;
using System.Collections;
using System.Collections.Generic;
using CookingPrototype.Utilities;
using UniRx;
using UnityEngine;

namespace CookingPrototype.GameCore {
public class Timer : IDisposable
{
    private Action _started;
    private Action<TimeSpan> _ticked;
    private Action _completed;

    private CompositeDisposable _disposable;

    private TimeSpan _tickInterval;
    private TimeSpan _tickTime;
    private bool _callTickOnStarted;
    private bool _repeatable;
    
    private readonly TimeSpan _tickFrequency;
    
    public TimeSpan TimeLeft { get; private set; }

    public TimeSpan InitialTime { get; private set; }
    public bool IsOn => _disposable != null;
    public Timer(TimeSpan initialTime) : this(initialTime, TimeSpan.FromMilliseconds(500))
    {
        
    }
    public Timer(TimeSpan initialTime, TimeSpan tickFrequency)
    {
        InitialTime = initialTime;
        _tickInterval = 1f.ToTimeSpanSeconds();
        _tickFrequency = tickFrequency;
    }
    public Timer(DateTime timeNow, DateTime dueTime)
        : this(dueTime - timeNow) { }
    
    public Timer OnStarted(params Action[] callbacks)
    {
        foreach (var callback in callbacks)
        {
            _started += callback;
        }
        return this;
    }
    public Timer OnTick(params Action<TimeSpan>[] callbacks)
    {
        foreach (var callback in callbacks)
        {
            _ticked += callback;
        }

        return this;
    }
    public Timer OnCompleted(params Action[] callbacks)
    {
        foreach (var callback in callbacks)
        {
            _completed += callback;
        }

        return this;
    }
    public Timer SetTickInterval(TimeSpan interval)
    {
        _tickInterval = interval;
        return this;
    }

    public Timer SetTickCallbackOnStarted(bool value = true)
    {
        _callTickOnStarted = value;
        return this;
    }
    public Timer SetRepeatable(bool value = true)
    {
        _repeatable = true;
        return this;
    }

    public Timer Start()
    {
        TimeLeft = InitialTime;
        if(_callTickOnStarted)
            _ticked?.Invoke(TimeLeft);
        
        _started?.Invoke();
        Resume();
        return this;
    }

    public void Reset()
    {
        Reset(InitialTime);
    }
    public void Reset(TimeSpan newTime)
    {
        InitialTime = newTime;
        TimeLeft = newTime;
    }
   
    public void Stop()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
    public void Resume()
    {
        if (_disposable != null)
        {
            Stop();
        }

        _disposable = new CompositeDisposable();
        Observable.Timer(_tickFrequency)
            .Repeat()
            .Subscribe(_ => { UpdateTimer(); })
            .AddTo(_disposable);
    }

    private void UpdateTimer()
    {
        _tickTime += _tickFrequency;
        TimeLeft -= _tickFrequency;

        if (_tickTime >= _tickInterval)
        {
            _ticked?.Invoke(TimeLeft);
            _tickTime = 0f.ToTimeSpanSeconds();
        }
        
        CheckCompleted();
    }

    private void CheckCompleted()
    {
        if (TimeLeft > TimeSpan.Zero) return;
        
        Stop();
        _completed?.Invoke();

        if (_repeatable)
        {
            Reset();
            Resume();
        }
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    } 
}

}
