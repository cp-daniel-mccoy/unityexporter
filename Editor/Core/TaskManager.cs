﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TaskManager
{
    List<IEnumerator> _tasks;
    IEnumerator _current = null;

    public TaskManager()
    {
        _tasks = new List<IEnumerator>();
    }

    public void addTask(IEnumerator task)
    {
        _tasks.Add(task);
    }

    public void clear()
    {
        _tasks.Clear();
    }

    public bool play()
    {
        if (_tasks.Count > 0)
        {
            if (_current == null || !_current.MoveNext())
            {
                _current = _tasks[0];
                _tasks.RemoveAt(0);
            }
        }

        if (_current != null)
            _current.MoveNext();

        if (_current != null && !_current.MoveNext() && _tasks.Count == 0)
            return false;

        return true;
    }
}