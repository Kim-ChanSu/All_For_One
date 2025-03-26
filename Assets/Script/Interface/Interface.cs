using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StatusSubject
{
    void AddObserver(StatusObserver observer);
    void RemoveObserver(StatusObserver observer);
    void SetObservers();
}

public interface StatusObserver
{
    void InitObserver(StatusSubject subject);
    void SetObserver(DungeonCharacter status);
}

